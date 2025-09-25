"""
Delta robot kinematics, trajectory planning, and Siemens S7-1200 PLC communication.

References:
- Rotary Delta Robot Forward/Inverse Kinematics Calculations (used for validation and cross-checking):
  https://www.marginallyclever.com/other/samples/fk-ik-test.html

This module provides:
- DeltaDimensions: robot geometry parameters
- DeltaRobot: FK and IK for a standard rotary delta robot
- TrajectoryPlanner: simple linear path with trapezoidal velocity profile
- S7PLCClient: minimal wrapper over snap7 to communicate with Siemens S7-1200

Notes:
- Angles are in degrees at the public API. Internally some helpers use radians.
- Coordinate system: X right, Y forward, Z up. Typical delta robots have working area at negative Z
  if you define Z=0 at base plane. Adjust to your machine as needed.
- The equations follow the commonly used formulation for rotary delta robots with parallel forearms.
"""
from __future__ import annotations

from dataclasses import dataclass
from math import sqrt, sin, cos, tan, atan2, radians, degrees, acos
from typing import List, Tuple, Optional, Iterable

try:
    import snap7  # type: ignore
    from snap7.util import get_bool, get_real, set_bool, set_real  # type: ignore
    SNAP7_AVAILABLE = True
except Exception:  # pragma: no cover - optional dep
    SNAP7_AVAILABLE = False


@dataclass
class DeltaDimensions:
    base_radius_f: float  # distance from center of base to motor shaft (f)
    end_effector_radius_e: float  # distance from wrist joint to tool center (e)
    bicep_length_rf: float  # length from motor shaft to elbow (rf)
    forearm_length_re: float  # length from elbow to wrist (re)
    base_to_floor_b: float = 0.0  # optional, not used in math here


class DeltaKinematicsError(Exception):
    pass


class DeltaRobot:
    """Rotary delta robot FK/IK.

    The math is adapted from widely used delta formulations (e.g., Trossen/Marginally Clever variants).
    The three upper arms are separated by 120 degrees around the Z axis.
    """

    # arm offset angles around Z (degrees)
    ARM_ANGLES_DEG: Tuple[float, float, float] = (0.0, 120.0, -120.0)

    def __init__(
        self,
        dimensions: DeltaDimensions,
        min_angle_deg: float = -70.0,
        max_angle_deg: float = 70.0,
    ) -> None:
        self.dim = dimensions
        self.min_angle_deg = min_angle_deg
        self.max_angle_deg = max_angle_deg

        # Precompute constants used by IK/FK
        self._sqrt3 = sqrt(3.0)
        self._pi = 3.141592653589793
        self._sin120 = self._sqrt3 / 2.0
        self._cos120 = -0.5
        self._tan60 = self._sqrt3
        self._sin30 = 0.5
        self._tan30 = 1.0 / self._sqrt3

        self.f = self.dim.base_radius_f
        self.e = self.dim.end_effector_radius_e
        self.rf = self.dim.bicep_length_rf
        self.re = self.dim.forearm_length_re

        # Derived geometry (using common notation)
        self._t = (self.f - self.e) * self._tan30 / 2.0

    # ---------- Public API ----------
    def inverse_kinematics(self, x: float, y: float, z: float) -> Tuple[float, float, float]:
        """Compute joint angles (theta1, theta2, theta3) in degrees for desired TCP at (x,y,z).

        Raises DeltaKinematicsError if the position is unreachable.
        """
        theta1 = self._angle_yz(x, y, z)
        theta2 = self._angle_yz(
            x * self._cos120 + y * self._sin120,
            y * self._cos120 - x * self._sin120,
            z,
        )
        theta3 = self._angle_yz(
            x * self._cos120 - y * self._sin120,
            y * self._cos120 + x * self._sin120,
            z,
        )

        self._validate_joint_limits((theta1, theta2, theta3))
        return (theta1, theta2, theta3)

    def forward_kinematics(self, theta1: float, theta2: float, theta3: float) -> Tuple[float, float, float]:
        """Compute TCP (x,y,z) in mm from joint angles (degrees).

        Raises DeltaKinematicsError if no valid solution exists.
        """
        # Convert to radians
        t1 = radians(theta1)
        t2 = radians(theta2)
        t3 = radians(theta3)

        # Upper arm joint coordinates (elbow pivots) relative to base
        y1 = -(self._t + self.rf * cos(t1))
        z1 = -self.rf * sin(t1)

        y2 = (self._t + self.rf * cos(t2)) * self._sin30
        x2 = y2 * self._tan60
        z2 = -self.rf * sin(t2)

        y3 = (self._t + self.rf * cos(t3)) * self._sin30
        x3 = -y3 * self._tan60
        z3 = -self.rf * sin(t3)

        # Wrist joint spheres: center = elbow pivot; radius = re
        # Find intersection of three spheres projected in YZ/XZ planes; analytic solution
        x1 = 0.0
        dnm = (y2 - y1) * (x3 - x1) - (y3 - y1) * (x2 - x1)
        if abs(dnm) < 1e-9:
            raise DeltaKinematicsError("Singular configuration in FK (dnm≈0)")

        w1 = y1 * y1 + z1 * z1
        w2 = x2 * x2 + y2 * y2 + z2 * z2
        w3 = x3 * x3 + y3 * y3 + z3 * z3

        # x = (a1*z + b1)/dnm ; y = (a2*z + b2)/dnm
        a1 = (z2 - z1) * (y3 - y1) - (z3 - z1) * (y2 - y1)
        b1 = -((w2 - w1) * (y3 - y1) - (w3 - w1) * (y2 - y1)) / 2.0
        a2 = (z2 - z1) * (x3 - x1) - (z3 - z1) * (x2 - x1)
        b2 = ((w2 - w1) * (x3 - x1) - (w3 - w1) * (x2 - x1)) / 2.0

        # Solve for z from sphere equation x^2 + y^2 + z^2 + Ax*z + Bz + C = 0
        a = a1 * a1 + a2 * a2 + dnm * dnm
        b = 2.0 * (a1 * b1 + a2 * b2 + dnm * dnm * z1)
        c = (b1 * b1 + b2 * b2 + dnm * dnm * (w1 - self.re * self.re))

        disc = b * b - 4.0 * a * c
        if disc < 0.0:
            raise DeltaKinematicsError("No real FK solution (discriminant<0)")

        z = -0.5 * (b + sqrt(disc)) / a  # choose lower Z (typical)
        x = (a1 * z + b1) / dnm
        y = (a2 * z + b2) / dnm
        return (x, y, z)

    # ---------- Trajectory Planning ----------
    def plan_linear_trapezoid(
        self,
        start_xyz: Tuple[float, float, float],
        end_xyz: Tuple[float, float, float],
        max_vel: float = 200.0,  # mm/s
        max_acc: float = 1000.0,  # mm/s^2
        dt: float = 0.01,  # s
    ) -> List[Tuple[float, float, float, float]]:
        """Plan a time-parameterized linear XYZ path with trapezoidal speed profile.

        Returns a list of (t, x, y, z) samples.
        """
        sx, sy, sz = start_xyz
        ex, ey, ez = end_xyz

        dx, dy, dz = (ex - sx), (ey - sy), (ez - sz)
        dist = sqrt(dx * dx + dy * dy + dz * dz)
        if dist < 1e-9:
            return [(0.0, sx, sy, sz)]

        ux, uy, uz = dx / dist, dy / dist, dz / dist

        # Trapezoid timings
        t_acc = max_vel / max_acc
        d_acc = 0.5 * max_acc * t_acc * t_acc
        if 2 * d_acc > dist:
            # Triangle profile
            t_acc = sqrt(dist / max_acc)
            t_cruise = 0.0
        else:
            # Trapezoid
            t_cruise = (dist - 2 * d_acc) / max_vel
        t_total = 2 * t_acc + t_cruise

        samples: List[Tuple[float, float, float, float]] = []
        t = 0.0
        while t < t_total - 1e-12:
            if t < t_acc:
                s = 0.5 * max_acc * t * t
            elif t < t_acc + t_cruise:
                s = d_acc + max_vel * (t - t_acc)
            else:
                t2 = t - t_acc - t_cruise
                s = d_acc + (dist - 2 * d_acc) + (max_vel * t2 - 0.5 * max_acc * t2 * t2)
            x = sx + ux * s
            y = sy + uy * s
            z = sz + uz * s
            samples.append((t, x, y, z))
            t += dt
        samples.append((t_total, ex, ey, ez))
        return samples

    def plan_linear_trapezoid_to_joints(
        self,
        start_xyz: Tuple[float, float, float],
        end_xyz: Tuple[float, float, float],
        max_vel: float = 200.0,
        max_acc: float = 1000.0,
        dt: float = 0.01,
    ) -> List[Tuple[float, float, float, float, float, float, float]]:
        """Plan linear trapezoid in XYZ and convert to joints via IK.
        Returns list of (t, x, y, z, th1, th2, th3).
        """
        path = self.plan_linear_trapezoid(start_xyz, end_xyz, max_vel, max_acc, dt)
        result: List[Tuple[float, float, float, float, float, float, float]] = []
        for t, x, y, z in path:
            th1, th2, th3 = self.inverse_kinematics(x, y, z)
            result.append((t, x, y, z, th1, th2, th3))
        return result

    # ---------- PLC Communication ----------
    def to_plc_payload(self, angles_deg: Tuple[float, float, float]) -> bytes:
        """Pack three float angles (deg) as IEEE754 floats (little-endian) suitable for a PLC DB."""
        import struct
        return struct.pack("<fff", *angles_deg)

    @staticmethod
    def from_plc_payload(payload: bytes) -> Tuple[float, float, float]:
        import struct
        th1, th2, th3 = struct.unpack("<fff", payload[:12])
        return (th1, th2, th3)

    # ---------- Internal helpers ----------
    def _validate_joint_limits(self, thetas_deg: Tuple[float, float, float]) -> None:
        lo, hi = self.min_angle_deg, self.max_angle_deg
        for th in thetas_deg:
            if th < lo - 1e-6 or th > hi + 1e-6:
                raise DeltaKinematicsError(
                    f"Joint angle {th:.3f} deg out of limits [{lo:.3f}, {hi:.3f}]"
                )

    def _angle_yz(self, x: float, y: float, z: float) -> float:
        """IK helper that finds the angle for one arm projected into the YZ plane.

        Raises DeltaKinematicsError for unreachable positions.
        """
        y1 = -self._t
        y0 = y
        z0 = z

        # Shift to elbow circle center at (0, y1)
        a = (x * x + y0 * y0 + z0 * z0 + self.rf * self.rf - self.re * self.re - y1 * y1) / (2 * z0) if abs(z0) > 1e-12 else float('inf')
        b = (y1 - y0) / z0 if abs(z0) > 1e-12 else float('inf')

        # (a + b*y)^2 + y^2 + x^2 = rf^2 -> quadratic in y
        d = -(a + b * y1) * (a + b * y1) + self.rf * (self.rf * (1 + b * b))
        if d < 0:
            raise DeltaKinematicsError("Unreachable IK point (d<0)")
        yj = (y1 - a * b - sqrt(d)) / (1 + b * b)  # choose elbow-down solution
        zj = a + b * yj
        theta = degrees(atan2(-(zj), (y1 - yj)))  # motor angle in degrees
        return theta


class TrajectoryPlanner:
    def __init__(self, robot: DeltaRobot) -> None:
        self.robot = robot

    def linear_xyz(self, start_xyz: Tuple[float, float, float], end_xyz: Tuple[float, float, float], max_vel: float, max_acc: float, dt: float) -> List[Tuple[float, float, float, float]]:
        return self.robot.plan_linear_trapezoid(start_xyz, end_xyz, max_vel, max_acc, dt)

    def linear_to_joints(self, start_xyz: Tuple[float, float, float], end_xyz: Tuple[float, float, float], max_vel: float, max_acc: float, dt: float) -> List[Tuple[float, float, float, float, float, float, float]]:
        return self.robot.plan_linear_trapezoid_to_joints(start_xyz, end_xyz, max_vel, max_acc, dt)


class S7PLCClient:
    """Minimal Siemens S7 client using snap7."""

    def __init__(self, address: str, rack: int = 0, slot: int = 1) -> None:
        self.address = address
        self.rack = rack
        self.slot = slot
        self._client: Optional["snap7.client.Client"] = None

    def connect(self) -> None:
        if not SNAP7_AVAILABLE:
            raise RuntimeError("snap7 is not installed. Please `pip install snap7-python`.")
        if self._client is None:
            self._client = snap7.client.Client()
        self._client.connect(self.address, self.rack, self.slot)

    def disconnect(self) -> None:
        if self._client is not None:
            try:
                self._client.disconnect()
            finally:
                self._client.destroy()
                self._client = None

    def read_db(self, db_number: int, start: int, size: int) -> bytes:
        if self._client is None:
            raise RuntimeError("PLC not connected")
        return self._client.db_read(db_number, start, size)

    def write_db(self, db_number: int, start: int, data: bytes) -> None:
        if self._client is None:
            raise RuntimeError("PLC not connected")
        self._client.db_write(db_number, start, data)


# ------------------------- Self-test / CLI -------------------------
if __name__ == "__main__":
    # Example dimensions (mm). Replace with your actual robot data.
    dims = DeltaDimensions(
        base_radius_f=200.0,
        end_effector_radius_e=50.0,
        bicep_length_rf=150.0,
        forearm_length_re=350.0,
    )
    robot = DeltaRobot(dims, min_angle_deg=-70.0, max_angle_deg=70.0)

    # Inverse -> Forward roundtrip
    xyz = (0.0, 0.0, -300.0)
    th = robot.inverse_kinematics(*xyz)
    xyz_fk = robot.forward_kinematics(*th)
    err = sqrt((xyz[0]-xyz_fk[0])**2 + (xyz[1]-xyz_fk[1])**2 + (xyz[2]-xyz_fk[2])**2)
    print("IK angles (deg):", th)
    print("FK back to XYZ:", xyz_fk, " error(mm)=", err)

    # Plan a short move
    planner = TrajectoryPlanner(robot)
    path = planner.linear_to_joints(xyz, (50.0, -30.0, -320.0), max_vel=200.0, max_acc=800.0, dt=0.02)
    print("Planned samples:", len(path))
    print("First sample:", path[0])
    print("Last sample:", path[-1])

    # PLC payload demo (no actual PLC IO here)
    payload = robot.to_plc_payload(th)
    print("PLC payload length:", len(payload))
    print("Unpacked angles:", DeltaRobot.from_plc_payload(payload))
