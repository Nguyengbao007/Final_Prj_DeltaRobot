# pip install python-snap7
import math
import snap7
from snap7.util import set_real
from dataclasses import dataclass

@dataclass
class DeltaParams:
    e: float   # mm, side of end-effector triangle
    f: float   # mm, side of base triangle
    rf: float  # mm, upper arm length
    re: float  # mm, lower arm length
    deg_limit: float = 75.0  # giới hạn |theta| (độ), tùy cơ khí

class DeltaIK:
    # Các hằng phụ trợ hình học
    sqrt3 = math.sqrt(3.0)
    pi = math.pi
    sin120 = sqrt3 / 2.0
    cos120 = -0.5
    tan60 = sqrt3
    sin30 = 0.5
    tan30 = 1.0 / sqrt3

    def __init__(self, p: DeltaParams):
        self.p = p
        # chuyển sang bán kính tương đương (khoảng cách từ tâm -> cạnh)
        self.e = p.e
        self.f = p.f
        self.rf = p.rf
        self.re = p.re
        self.E = self.e * self.tan30 / 2.0
        self.F = self.f * self.tan30 / 2.0

    def _angle_yz(self, x0, y0, z0):
        """
        Tính góc cho 1 tay (project vào mặt phẳng YZ) – phần cốt lõi của IK.
        Trả về (theta_deg, ok)
        """
        # dịch trục X do khoảng cách giữa base và platform
        y1 = -self.F
        y0 = y0 - self.E

        a = (x0**2 + y0**2 + z0**2 + self.rf**2 - self.re**2 - y1**2) / (2*z0)
        b = (y1 - y0) / z0

        # phương trình bậc 2 theo y: (a + b*y)^2 + y^2 + x0^2 = rf^2
        # -> (1 + b^2) y^2 + 2ab y + (a^2 + x0^2 - rf^2) = 0
        A = 1 + b*b
        B = 2*a*b
        C = a*a + x0*x0 - self.rf*self.rf

        disc = B*B - 4*A*C
        if disc < 0:
            return None, False  # ngoài không gian làm việc

        yj = (-B - math.sqrt(disc)) / (2*A)  # nhánh vật lý
        zj = a + b*yj

        # góc khớp: theta = atan2(-zj, y1 - yj)
        theta = math.degrees(math.atan2(-(zj), (y1 - yj)))
        return theta, True

    def inverse(self, x, y, z):
        """
        Trả về (theta1, theta2, theta3) theo độ.
        Ném ValueError nếu không giải được hoặc vượt giới hạn.
        """
        # Tay 1 (trục tại góc 0°)
        t1, ok1 = self._angle_yz(x, y, z)
        if not ok1:
            raise ValueError("Vị trí ngoài không gian làm việc (tay 1)")

        # Tay 2 (quay -120° quanh Z)
        x2 = x * self.cos120 + y * self.sin120
        y2 = -x * self.sin120 + y * self.cos120
        t2, ok2 = self._angle_yz(x2, y2, z)
        if not ok2:
            raise ValueError("Vị trí ngoài không gian làm việc (tay 2)")

        # Tay 3 (quay +120° quanh Z)
        x3 = x * self.cos120 - y * self.sin120
        y3 = x * self.sin120 + y * self.cos120
        t3, ok3 = self._angle_yz(x3, y3, z)
        if not ok3:
            raise ValueError("Vị trí ngoài không gian làm việc (tay 3)")

        # kiểm tra giới hạn góc
        for t in (t1, t2, t3):
            if abs(t) > self.p.deg_limit:
                raise ValueError(f"Góc vượt giới hạn |θ| ≤ {self.p.deg_limit}°")

        return (t1, t2, t3)

# ---------------------------
# Ví dụ sử dụng + gửi PLC
# ---------------------------
def send_angles_to_plc(ip: str, rack: int, slot: int, db_number: int, angles_deg):
    """
    angles_deg: tuple/list 3 phần tử (θ1, θ2, θ3) – độ
    Ghi vào DB theo dạng REAL liên tiếp: offset 0, 4, 8.
    """
    if len(angles_deg) != 3:
        raise ValueError("Cần đúng 3 góc")

    data = bytearray(12)  # 3 * REAL (4 byte)
    set_real(data, 0, float(angles_deg[0]))
    set_real(data, 4, float(angles_deg[1]))
    set_real(data, 8, float(angles_deg[2]))

    plc = snap7.client.Client()
    plc.connect(ip, rack, slot)
    if not plc.get_connected():
        raise ConnectionError("Không kết nối được PLC")

    try:
        plc.write_area(snap7.types.Areas.DB, db_number, 0, data)
    finally:
        plc.disconnect()

if __name__ == "__main__":
    # ====== 1) Khai báo tham số Delta (ví dụ phổ biến) ======
    params = DeltaParams(
        e=115.0,   # mm, cạnh platform
        f=457.3,   # mm, cạnh base
        rf=224.0,  # mm, upper arm
        re=457.3,  # mm, lower arm
        deg_limit=75.0
    )
    ik = DeltaIK(params)

    # ====== 2) Tính IK cho điểm mong muốn ======
    # Chú ý: z âm (đi xuống). Điều chỉnh (x,y,z) theo thực tế.
    target = (0.0, 0.0, -400.0)  # mm
    try:
        angles = ik.inverse(*target)
        print("Góc (deg): θ1, θ2, θ3 =", angles)
    except ValueError as e:
        print("Lỗi IK:", e)
        raise

    # ====== 3) Gửi về PLC (S7-1200/1500) ======
    # PLC cần có DB1 với REAL Angle[0..2] tại offset 0.
    PLC_IP = "192.168.0.1"
    RACK = 0
    SLOT = 1       # S7-1200/1500 thường là 1
    DB_NUMBER = 1  # ví dụ DB1

    send_angles_to_plc(PLC_IP, RACK, SLOT, DB_NUMBER, angles)
    print("Đã ghi góc vào PLC.")
