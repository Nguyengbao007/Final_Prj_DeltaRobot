# delta_plc.py
# --- Robot Delta: Äá»™ng há»c thuáº­n/nghá»‹ch + gá»­i dá»¯ liá»‡u gÃ³c tá»›i PLC Siemens ---
# ÄÆ¡n vá»‹: hÃ¬nh há»c mm, gÃ³c Ä‘á»™. Trá»¥c Z Ã¢m (hÆ°á»›ng xuá»‘ng dÆ°á»›i).

import math
import time
from dataclasses import dataclass
from typing import Tuple, List, Iterable, Optional

# ========== Náº¿u muá»‘n cháº¡y offline (khÃ´ng cÃ³ PLC) thÃ¬ cÃ³ thá»ƒ comment pháº§n snap7 ==========
try:
    import snap7
    from snap7.util import set_real
except Exception:
    snap7 = None
# =======================================================================================

# -----------------------------
# Khai bÃ¡o tham sá»‘ hÃ¬nh há»c robot Delta
# -----------------------------
@dataclass
class DeltaParams:
    e: float   # cáº¡nh tam giÃ¡c bÃ n Ä‘á»™ng (mm)
    f: float   # cáº¡nh tam giÃ¡c Ä‘áº¿ (mm)
    rf: float  # chiá»u dÃ i tay trÃªn (upper arm)
    re: float  # chiá»u dÃ i tay dÆ°á»›i (parallelogram)
    deg_limit: float = 75.0  # giá»›i háº¡n gÃ³c tá»‘i Ä‘a cho má»—i khá»›p (Ä‘á»™)



class DeltaKinematics:
    # Háº±ng sá»‘ toÃ¡n há»c
    sqrt3 = math.sqrt(3.0)
    sin120 = sqrt3 / 2.0
    cos120 = -0.5
    tan30 = 1.0 / sqrt3
    sin30 = 0.5
    tan60 = sqrt3

    def __init__(self, p: DeltaParams):
        self.p = p
        self.base_radius = 0.5 * self.tan60 * p.f
        self.platform_radius = 0.5 * self.tan60 * p.e
        self.t = (p.f - p.e) * self.tan30 / 2.0

    # ---------- Äá»™ng há»c nghá»‹ch (IK) ----------
    def _angle_yz(self, x0: float, y0: float, z0: float) -> Tuple[float, bool]:
        """Solve single-arm inverse kinematics in the YZ plane."""
        if abs(z0) < 1e-9:
            return 0.0, False

        y1 = -self.base_radius
        y0_shift = y0 - self.platform_radius

        a = (x0 * x0 + y0_shift * y0_shift + z0 * z0 + self.p.rf * self.p.rf - self.p.re * self.p.re - y1 * y1) / (2 * z0)
        b = (y1 - y0_shift) / z0

        d = self.p.rf * (b * b * self.p.rf + self.p.rf) - (a + b * y1) * (a + b * y1)
        if d < 0:
            return 0.0, False

        yj = (y1 - a * b - math.sqrt(d)) / (b * b + 1)
        zj = a + b * yj

        theta = math.degrees(math.atan2(-zj, y1 - yj))
        return theta, True

    def inverse(self, x: float, y: float, z: float) -> Tuple[float, float, float]:
        """
        Inverse kinematics using the standard delta robot formulation.
        """
        # Arm 1
        t1, ok1 = self._angle_yz(x, y, z)
        if not ok1:
            raise ValueError('IK no real solution (arm 1)')

        # Arm 2 (rotate -120? around Z)
        x2 = x * self.cos120 + y * self.sin120
        y2 = -x * self.sin120 + y * self.cos120
        t2, ok2 = self._angle_yz(x2, y2, z)
        if not ok2:
            raise ValueError('IK no real solution (arm 2)')

        # Arm 3 (rotate +120? around Z)
        x3 = x * self.cos120 - y * self.sin120
        y3 = x * self.sin120 + y * self.cos120
        t3, ok3 = self._angle_yz(x3, y3, z)
        if not ok3:
            raise ValueError('IK no real solution (arm 3)')

        for t in (t1, t2, t3):
            if abs(t) > self.p.deg_limit:
                raise ValueError(f'Angle exceeds limit ?{self.p.deg_limit}?')

        return (float(t1), float(t2), float(t3))

    # ---------- Äá»™ng há»c thuáº­n (FK) ----------
    def forward(self, t1: float, t2: float, t3: float) -> Tuple[float, float, float]:
        """
        Forward kinematics using the standard delta robot formulation.
        """
        rf, re = self.p.rf, self.p.re
        t = self.t

        theta1 = math.radians(t1)
        theta2 = math.radians(t2)
        theta3 = math.radians(t3)

        y1 = -(t + rf * math.cos(theta1))
        z1 = -rf * math.sin(theta1)

        y2 = (t + rf * math.cos(theta2)) * self.sin30
        x2 = y2 * self.tan60
        z2 = -rf * math.sin(theta2)

        y3 = (t + rf * math.cos(theta3)) * self.sin30
        x3 = -y3 * self.tan60
        z3 = -rf * math.sin(theta3)

        dnm = (y2 - y1) * x3 - (y3 - y1) * x2
        if abs(dnm) < 1e-12:
            raise ValueError('FK degenerate configuration')

        w1 = y1 * y1 + z1 * z1
        w2 = x2 * x2 + y2 * y2 + z2 * z2
        w3 = x3 * x3 + y3 * y3 + z3 * z3

        a1 = (z2 - z1) * (y3 - y1) - (z3 - z1) * (y2 - y1)
        b1 = (w2 - w1) * (y3 - y1) - (w3 - w1) * (y2 - y1)
        a2 = (z2 - z1) * x3 - (z3 - z1) * x2
        b2 = (w2 - w1) * x3 - (w3 - w1) * x2

        a = a1 * a1 + a2 * a2 + dnm * dnm
        b = 2 * (a1 * b1 + a2 * (b2 - y1 * dnm) - z1 * dnm * dnm)
        c = (b2 - y1 * dnm) ** 2 + b1 * b1 + dnm * dnm * (z1 * z1 - re * re)

        disc = b * b - 4 * a * c
        if disc < 0:
            raise ValueError('FK no real solution')

        z0 = -0.5 * (b + math.sqrt(disc)) / a
        x0 = (a1 * z0 + b1) / dnm
        y0 = (a2 * z0 + b2) / dnm
        return (float(x0), float(y0), float(z0))


# -----------------------------
# Gá»­i dá»¯ liá»‡u gÃ³c sang PLC (snap7)
# -----------------------------
def send_angles_to_plc(ip: str, rack: int, slot: int, db_number: int, angles_deg: Tuple[float, float, float]):
    """
    angles_deg: tuple 3 gÃ³c (deg)
    Ghi vÃ o DB PLC dáº¡ng REAL liÃªn tiáº¿p offset 0,4,8
    """
    if snap7 is None:
        raise RuntimeError("ChÆ°a cÃ i python-snap7")

    data = bytearray(12)  # 3 REAL = 12 byte
    set_real(data, 0, float(angles_deg[0]))
    set_real(data, 4, float(angles_deg[1]))
    set_real(data, 8, float(angles_deg[2]))

    plc = snap7.client.Client()
    plc.connect(ip, rack, slot)
    try:
        if not plc.get_connected():
            raise ConnectionError("KhÃ´ng káº¿t ná»‘i Ä‘Æ°á»£c PLC")
        plc.write_area(snap7.types.Areas.DB, db_number, 0, data)
    finally:
        plc.disconnect()


# -----------------------------
# Táº¡o vÃ  cháº¡y quá»¹ Ä‘áº¡o tuyáº¿n tÃ­nh
# -----------------------------
def linspace3(p0, p1, n):
    """Ná»™i suy tuyáº¿n tÃ­nh tá»« p0 -> p1, chia n Ä‘iá»ƒm"""
    if n < 2:
        yield p1
        return
    dx = (p1[0] - p0[0]) / (n - 1)
    dy = (p1[1] - p0[1]) / (n - 1)
    dz = (p1[2] - p0[2]) / (n - 1)
    for i in range(n):
        yield (p0[0] + i*dx, p0[1] + i*dy, p0[2] + i*dz)

def stream_linear_trajectory_to_plc(
    kin: DeltaKinematics,
    p0, p1, n_points,
    plc_ip, rack, slot, db,
    period_s=0.02,
    dry_run=True
):
    """
    Sinh quá»¹ Ä‘áº¡o tuyáº¿n tÃ­nh tá»« p0 -> p1 gá»“m n_points.
    Vá»›i má»—i Ä‘iá»ƒm: tÃ­nh IK -> gá»­i sang PLC.
    period_s: chu ká»³ gá»­i (giÃ¢y), vÃ­ dá»¥ 0.02s = 50Hz
    dry_run=True: chá»‰ in káº¿t quáº£, khÃ´ng gá»­i PLC
    """
    angles_all = []
    for idx, p in enumerate(linspace3(p0, p1, n_points)):
        ang = kin.inverse(*p)
        angles_all.append(ang)

        print(f"Point {idx}: {p} -> gÃ³c {ang}")

        if not dry_run:
            send_angles_to_plc(plc_ip, rack, slot, db, ang)

        if period_s > 0:
            time.sleep(period_s)

    return angles_all


# -----------------------------
# Demo cháº¡y thá»­
# -----------------------------


if __name__ == "__main__":
    # 1) Default mechanical parameters (per spec: 100,100,100,100)
    params = DeltaParams(e=24.0, f=75.0, rf=100.0, re=300.0, deg_limit=120.0)
    kin = DeltaKinematics(params)

    # 2) Test IK and FK
    target = (0.0, 0.0, -120.0)
    angles = kin.inverse(*target)
    xyz_back = kin.forward(*angles)
    print("[IK] angles (deg):", angles)
    print("[FK] position (mm):", xyz_back)

    # 3) Generate a linear trajectory and dry-run
    PLC_IP = "192.168.0.1"
    RACK, SLOT, DB = 0, 1, 1
    P0 = (0.0, 0.0, -110.0)
    P1 = (30.0, 0.0, -130.0)

    traj = stream_linear_trajectory_to_plc(
        kin, P0, P1, n_points=10,
        plc_ip=PLC_IP, rack=RACK, slot=SLOT, db=DB,
        period_s=0.05,
        dry_run=True  # change to False to send to PLC
    )

