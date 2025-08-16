# delta_plc.py
# --- Robot Delta: Động học thuận/nghịch + gửi dữ liệu góc tới PLC Siemens ---
# Đơn vị: hình học mm, góc độ. Trục Z âm (hướng xuống dưới).

import math
import time
from dataclasses import dataclass
from typing import Tuple, List, Iterable, Optional

# ========== Nếu muốn chạy offline (không có PLC) thì có thể comment phần snap7 ==========
try:
    import snap7
    from snap7.util import set_real
except Exception:
    snap7 = None
# =======================================================================================

# -----------------------------
# Khai báo tham số hình học robot Delta
# -----------------------------
@dataclass
class DeltaParams:
    e: float   # cạnh tam giác bàn động (mm)
    f: float   # cạnh tam giác đế (mm)
    rf: float  # chiều dài tay trên (upper arm)
    re: float  # chiều dài tay dưới (parallelogram)
    deg_limit: float = 75.0  # giới hạn góc tối đa cho mỗi khớp (độ)


class DeltaKinematics:
    # Hằng số toán học
    sqrt3 = math.sqrt(3.0)
    sin120 = sqrt3 / 2.0
    cos120 = -0.5
    tan30 = 1.0 / sqrt3

    def __init__(self, p: DeltaParams):
        self.p = p
        # khoảng cách từ tâm -> cạnh cho base và platform
        self.E = p.e * self.tan30 / 2.0  # platform offset
        self.F = p.f * self.tan30 / 2.0  # base offset

    # ---------- Động học nghịch (IK) ----------
    def _angle_yz(self, x0: float, y0: float, z0: float) -> Tuple[float, bool]:
        """
        Giải cho một tay trong mặt phẳng YZ (x=0).
        Trả về (theta_deg, ok)
        """
        y1 = -self.F
        y0 = y0 - self.E

        # phương trình giải góc từ hình học
        a = (x0*x0 + y0*y0 + z0*z0 + self.p.rf*self.p.rf - self.p.re*self.p.re - y1*y1) / (2*z0)
        b = (y1 - y0) / z0

        A = 1 + b*b
        B = 2*a*b
        C = a*a + x0*x0 - self.p.rf*self.p.rf

        disc = B*B - 4*A*C
        if disc < 0:
            return 0.0, False  # không có nghiệm

        yj = (-B - math.sqrt(disc)) / (2*A)  # nghiệm vật lý
        zj = a + b*yj

        # góc khớp
        theta = math.degrees(math.atan2(-(zj), (y1 - yj)))
        return theta, True

    def inverse(self, x: float, y: float, z: float) -> Tuple[float, float, float]:
        """
        Động học nghịch: (x,y,z) [mm] -> (theta1, theta2, theta3) [deg]
        Nếu không tính được hoặc vượt giới hạn -> ValueError
        """
        # Tay 1
        t1, ok1 = self._angle_yz(x, y, z)
        if not ok1:
            raise ValueError("Ngoài workspace (tay 1)")

        # Tay 2 (quay -120° quanh Z)
        x2 = x * self.cos120 + y * self.sin120
        y2 = -x * self.sin120 + y * self.cos120
        t2, ok2 = self._angle_yz(x2, y2, z)
        if not ok2:
            raise ValueError("Ngoài workspace (tay 2)")

        # Tay 3 (quay +120° quanh Z)
        x3 = x * self.cos120 - y * self.sin120
        y3 = x * self.sin120 + y * self.cos120
        t3, ok3 = self._angle_yz(x3, y3, z)
        if not ok3:
            raise ValueError("Ngoài workspace (tay 3)")

        # Kiểm tra giới hạn góc
        for t in (t1, t2, t3):
            if abs(t) > self.p.deg_limit:
                raise ValueError(f"Góc vượt giới hạn |θ| ≤ {self.p.deg_limit}°")

        return (t1, t2, t3)

    # ---------- Động học thuận (FK) ----------
    def forward(self, t1: float, t2: float, t3: float) -> Tuple[float, float, float]:
        """
        Động học thuận: (θ1,θ2,θ3) [deg] -> (x,y,z) [mm]
        Cách: tính tọa độ elbow của 3 tay rồi giải giao điểm 3 mặt cầu bán kính re.
        """
        rf, re = self.p.rf, self.p.re
        F = self.F  # base offset

        # Hàm phụ: elbow trong hệ tọa độ local YZ
        def elbow_local(theta_deg: float) -> Tuple[float, float, float]:
            theta = math.radians(theta_deg)
            y_l = -(F + rf * math.cos(theta))
            z = -rf * math.sin(theta)
            return 0.0, y_l, z

        # Hàm xoay quanh trục Z
        def rotz(xl: float, yl: float, phi_deg: float) -> Tuple[float, float]:
            phi = math.radians(phi_deg)
            s, c = math.sin(phi), math.cos(phi)
            xg = xl * c + yl * s
            yg = -xl * s + yl * c
            return xg, yg

        # Tính 3 elbow
        e1 = elbow_local(t1)
        e2 = elbow_local(t2)
        e3 = elbow_local(t3)

        x1, y1 = rotz(e1[0], e1[1], 0.0)
        z1 = e1[2]

        x2, y2 = rotz(e2[0], e2[1], 120.0)
        z2 = e2[2]

        x3, y3 = rotz(e3[0], e3[1], -120.0)
        z3 = e3[2]

        # Giải giao điểm 3 mặt cầu
        def lin_from_pair(xi, yi, zi, xj, yj, zj):
            A = 2*(xi - xj)
            B = 2*(yi - yj)
            C = 2*(zi - zj)
            D = (xi*xi + yi*yi + zi*zi) - (xj*xj + yj*yj + zj*zj)
            return A, B, C, D

        A1, B1, C1, D1 = lin_from_pair(x1, y1, z1, x2, y2, z2)
        A2, B2, C2, D2 = lin_from_pair(x1, y1, z1, x3, y3, z3)

        detM = A1*B2 - A2*B1
        if abs(detM) < 1e-9:
            raise ValueError("FK bị suy biến (phương trình song song)")

        dx_dz_num = -(C1*B2 - C2*B1)
        dy_dz_num = -(A1*C2 - A2*C1)
        x0_num = (D1*B2 - D2*B1)
        y0_num = (A1*D2 - A2*D1)

        x_of_z = lambda z: (x0_num + dx_dz_num * z) / detM
        y_of_z = lambda z: (y0_num + dy_dz_num * z) / detM

        # Thay vào mặt cầu 1 -> phương trình bậc 2 theo z
        ax = dx_dz_num / detM
        ay = dy_dz_num / detM
        bx = x0_num / detM - x1
        by = y0_num / detM - y1

        a = ax*ax + ay*ay + 1.0
        b = 2*(ax*bx + ay*by - z1)
        c = bx*bx + by*by + z1*z1 - re*re

        disc = b*b - 4*a*c
        if disc < 0:
            raise ValueError("FK không có nghiệm thực")

        z_sol1 = (-b - math.sqrt(disc)) / (2*a)
        z_sol2 = (-b + math.sqrt(disc)) / (2*a)

        # chọn nghiệm z âm (robot delta nằm dưới base)
        z = z_sol1 if z_sol1 < z_sol2 else z_sol2
        if z > 0 and z_sol1 < 0: z = z_sol1
        if z > 0 and z_sol2 < 0: z = z_sol2

        x = x_of_z(z)
        y = y_of_z(z)
        return (float(x), float(y), float(z))


# -----------------------------
# Gửi dữ liệu góc sang PLC (snap7)
# -----------------------------
def send_angles_to_plc(ip: str, rack: int, slot: int, db_number: int, angles_deg: Tuple[float, float, float]):
    """
    angles_deg: tuple 3 góc (deg)
    Ghi vào DB PLC dạng REAL liên tiếp offset 0,4,8
    """
    if snap7 is None:
        raise RuntimeError("Chưa cài python-snap7")

    data = bytearray(12)  # 3 REAL = 12 byte
    set_real(data, 0, float(angles_deg[0]))
    set_real(data, 4, float(angles_deg[1]))
    set_real(data, 8, float(angles_deg[2]))

    plc = snap7.client.Client()
    plc.connect(ip, rack, slot)
    try:
        if not plc.get_connected():
            raise ConnectionError("Không kết nối được PLC")
        plc.write_area(snap7.types.Areas.DB, db_number, 0, data)
    finally:
        plc.disconnect()


# -----------------------------
# Tạo và chạy quỹ đạo tuyến tính
# -----------------------------
def linspace3(p0, p1, n):
    """Nội suy tuyến tính từ p0 -> p1, chia n điểm"""
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
    Sinh quỹ đạo tuyến tính từ p0 -> p1 gồm n_points.
    Với mỗi điểm: tính IK -> gửi sang PLC.
    period_s: chu kỳ gửi (giây), ví dụ 0.02s = 50Hz
    dry_run=True: chỉ in kết quả, không gửi PLC
    """
    angles_all = []
    for idx, p in enumerate(linspace3(p0, p1, n_points)):
        ang = kin.inverse(*p)
        angles_all.append(ang)

        print(f"Point {idx}: {p} -> góc {ang}")

        if not dry_run:
            send_angles_to_plc(plc_ip, rack, slot, db, ang)

        if period_s > 0:
            time.sleep(period_s)

    return angles_all


# -----------------------------
# Demo chạy thử
# -----------------------------
if __name__ == "__main__":
    # 1) Tham số cơ khí (theo yêu cầu: 100,100,100,100)
    params = DeltaParams(e=100.0, f=100.0, rf=100.0, re=100.0, deg_limit=75.0)
    kin = DeltaKinematics(params)

    # 2) Test IK và FK
    target = (0.0, 0.0, -120.0)
    angles = kin.inverse(*target)
    xyz_back = kin.forward(*angles)
    print("[IK] góc (deg):", angles)
    print("[FK] tính lại tọa độ (mm):", xyz_back)

    # 3) Tạo quỹ đạo tuyến tính và chạy thử
    PLC_IP = "192.168.0.1"
    RACK, SLOT, DB = 0, 1, 1
    P0 = (0.0, 0.0, -110.0)
    P1 = (30.0, 0.0, -130.0)

    traj = stream_linear_trajectory_to_plc(
        kin, P0, P1, n_points=10,
        plc_ip=PLC_IP, rack=RACK, slot=SLOT, db=DB,
        period_s=0.05,
        dry_run=True  # Đổi thành False để gửi PLC
    )
