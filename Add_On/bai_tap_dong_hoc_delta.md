# Bài tập động học robot Delta – Trình bày từng bước

## Thông số cơ khí dùng trong ví dụ
- e = f = r_f = r_e = 100 mm  
- E = e·tan(30°)/2 = 28.867513 mm  
- F = f·tan(30°)/2 = 28.867513 mm

Quy ước: trục Z dương hướng lên, nên toạ độ làm việc thường có z âm (bên dưới đế).

## Bài 1 – Động học nghịch (IK)
**Đề:** Cho vị trí bàn động (tâm end-effector)
$$P(x,y,z)=(30,-15,-130)\,\text{mm}$$
Hãy tính các góc khớp quay $\theta_1,\theta_2,\theta_3$.

### Bước 1. Chuẩn hoá hình học từng tay
Tính toạ độ điểm cần nối trong mặt phẳng local của mỗi tay (quay hệ $(x,y)$ lần lượt 0°, -120°, +120°):
- Tay 1: $(x_1,y_1,z)=(30,-15,-130)$
- Tay 2 (quay -120°): $(x_2,y_2,z)=(-12.990381,9.495190,-130)$
- Tay 3 (quay +120°): $(x_3,y_3,z)=(13.495190,-24.505810,-130)$

### Bước 2. Dịch điểm bám do offset $E$
Với mỗi tay, đặt $y' = y - E$, $y_1=-F$.
- Tay 1: $y'_1 = -43.867513$, $y_1 = -28.867513$
- Tay 2: $y'_2 = -19.372323$, $y_1 = -28.867513$
- Tay 3: $y'_3 = 4.613249$, $y_1 = -28.867513$

### Bước 3. Lập phương trình khoảng cách cho một tay
Với tay bất kỳ (đã đưa về local YZ):
$$ (y' + F - r_f\cos\theta)^2 + (z + r_f\sin\theta)^2 = r_e^2 $$
Biến đổi đại số đặt:
$$ a = \frac{x^2 + y'^2 + z^2 + r_f^2 - r_e^2 - F^2}{2z},\quad b=\frac{F - y'}{z} $$
Suy ra phương trình bậc hai theo $y_j$ (toạ độ khớp nối):
$$ (1+b^2)\,y_j^2 + 2ab\,y_j + (a^2 + x^2 - r_f^2) = 0 $$
Chọn nghiệm vật lý (tay "gập xuống"):  
$y_j = \dfrac{-B - \sqrt{\Delta}}{2A}$ với $A=1+b^2, B=2ab, C=a^2+x^2-r_f^2, \; \Delta=B^2-4AC$.  
Sau đó $z_j=a+b\,y_j$ và $\theta=\operatorname{atan2}(-z_j, F-y_j)$.

### Bước 4. Tính số cho từng tay
**Tay 1:**
- $a=-72.657790$, $b=-0.115385$  
- $A=1.013314$, $B=16.767182$, $C=-3820.845550$, $\Delta=15767.997585$  
- $y_j = -70.233869$ mm, $z_j = -64.553882$ mm  
- $\theta_{1} = 57.348108^\circ$

**Tay 2:**
- $a=-82.278717$, $b=-0.323250$  
- $A=1.104491$, $B=53.165394$, $C=-5740.436628$, $\Delta=28149.310158$  
- $y_j = -70.636400$ mm, $z_j = -49.331504$ mm  
- $\theta_{2} = 55.476836^\circ$

**Tay 3:**
- $a=-61.892259$, $b=0.257544$  
- $A=1.066329$, $B=-31.879999$, $C=-6165.309728$, $\Delta=27313.330564$  
- $y_j = -62.545163$ mm, $z_j = -78.000411$ mm  
- $\theta_{3} = 66.647126^\circ$

**Kết quả IK:**
$$(\theta_1,\theta_2,\theta_3) \approx (57.348^\circ,\;55.477^\circ,\;66.647^\circ) $$

### Bước 5. Kiểm tra điều kiện hình học/giới hạn
- $\Delta \ge 0$ cho cả 3 tay ⇒ điểm nằm trong workspace hình học.  
- $|\theta_i| \le \theta_{\max}$ (ví dụ 75°) ⇒ thoả giới hạn khớp.

## Bài 2 – Phát hiện điểm không với tới (counter‑example)
**Đề:** Với cùng tham số trên, thử $P=(70,70,-100)$ mm.  
Tính $\Delta$ cho 3 tay; nếu có tay nào $\Delta<0$ ⇒ không có nghiệm thực.
Kết quả $\Delta$ (tay 1→3): 5562.816129, 7433.279605, None.  
Vì tay 3 không có nghiệm thực ⇒ điểm không với tới.

## Bài 3 – Động học thuận (FK) – Cách giải từng bước
**Đề:** Cho trước bộ góc** thu được ở Bài 1**:  
$$(\theta_1,\theta_2,\theta_3)=(57.348^\circ,55.477^\circ,66.647^\circ)$$
Hãy tìm $(x,y,z)$.

### Bước 1. Tính toạ độ các khớp khuỷu $J_i$
Trong local của từng tay:  
$J=(0, -(F+r_f\cos\theta), -r_f\sin\theta)$,  rồi quay về global lần lượt 0°, +120°, −120°.

Ví dụ số (mm):  
- $J_1 \approx (0.000, -87.380, -84.577)$  
- $J_2 \approx (74.472, 43.055, -83.355)$  
- $J_3 \approx (-75.432, 44.326, -91.985)$

### Bước 2. Viết phương trình khoảng cách tới **ba điểm bám** trên bàn động
Điểm bám (midpoint cạnh) của từng tay là  
$W_i = O + R_i$, với $O=(x,y,z)$ là tâm bàn động, $R_i$ là vector offset từ tâm tới midpoint cạnh:
$$R_1=(0,-E,0),\quad R_2=\operatorname{Rot}_z(+120^\circ)R_1,\quad R_3=\operatorname{Rot}_z(-120^\circ)R_1$$
Ràng buộc độ dài thanh song song:
$$ \|W_i - J_i\|^2 = r_e^2,\quad i=1,2,3. $$

### Bước 3. Trừ cặp phương trình để khử bậc hai
Lấy (2)−(1) và (3)−(1) ta được hai phương trình **tuyến tính** theo $(x,y,z)$:
$$ A_1 x + B_1 y + C_1 z = D_1,\qquad A_2 x + B_2 y + C_2 z = D_2, $$
trong đó
$$\begin{aligned}
A_1&=2\big[(r_{2x}-r_{1x})-(x_2-x_1)\big],&\quad D_1&=\|(R_1-J_1)\|^2-\|(R_2-J_2)\|^2,\\
B_1&=2\big[(r_{2y}-r_{1y})-(y_2-y_1)\big],&\quad A_2&=2\big[(r_{3x}-r_{1x})-(x_3-x_1)\big],\\
C_1&=2(z_1-z_2),&\quad B_2&=2\big[(r_{3y}-r_{1y})-(y_3-y_1)\big],\\
&&\quad C_2&=2(z_1-z_3),\end{aligned}$$
với $R_i=(r_{ix},r_{iy},0)$, $J_i=(x_i,y_i,z_i)$.

Giải hai phương trình trên cho $x(z)$, $y(z)$ (dạng **tuyến tính** theo $z$).

### Bước 4. Thế lại vào một phương trình cầu (ví dụ của tay 1)
Ta thu được phương trình **bậc hai theo $z$**:  
$$ a z^2 + b z + c = 0. $$
Giải ra hai nghiệm $z$ (chọn nghiệm vật lý, thường $z<0$), rồi suy ra $x$ và $y$ bằng các hàm $x(z), y(z)$ đã có.

> **Gợi ý kiểm chứng:** thay $(x,y,z)$ tìm được vào hàm IK để kiểm tra có thu lại đúng các $\theta_i$.

---
## Ghi chú thực hành
- Khi lập trình điều khiển, ưu tiên dùng **IK** để sinh góc từ quỹ đạo $(x,y,z)$ vì nhanh và ổn định.  
- Luôn kiểm tra $\Delta\ge0$ và giới hạn $|\theta_i|\le\theta_{\max}$.  
- Với PLC Siemens, có thể truyền chuỗi góc qua Snap7/OPC UA; mỗi điểm quỹ đạo là một bản ghi $(\theta_1,\theta_2,\theta_3)$.
