# 🏗️ Delta Robot  -- README

## 📝 Tóm tắt đồ án (Abstract)

Đồ án "**Robot Delta phân loại bánh ngọt**" trình bày quá trình nghiên
cứu, thiết kế và xây dựng một hệ thống robot Delta 3 bậc tự do phục vụ
cho việc phân loại sản phẩm trên băng tải. Hệ thống tích hợp cơ khí,
điều khiển PLC, truyền động servo, thị giác máy tính và trí tuệ nhân tạo
YOLOv8. Robot sử dụng camera để nhận dạng sản phẩm, chuyển đổi toạ độ và
điều khiển di chuyển chính xác đến vị trí gắp. Kết quả đạt được là một
mô hình hoàn chỉnh có khả năng vận hành thực tế, minh họa rõ ràng quy
trình tự động hóa trong công nghiệp.

------------------------------------------------------------------------

## 👨‍🎓 Thông tin sinh viên & Giảng viên hướng dẫn

-   **Họ và tên:** Nguyễn Gia Bảo -- MSSV: 21151073
-   **Họ và tên:** Nguyễn Xuân Hoàng -- MSSV: 21151459
-   **Ngành:** Kỹ thuật Điều khiển & Tự động hoá
-   **Khoa:** Điện -- Điện Tử (Khoá K21)
-   **Giảng viên hướng dẫn:** TS. Trần Mạnh Sơn
-   **Trường:** Đại học Sư Phạm Kỹ Thuật TP.HCM (HCMUTE)

------------------------------------------------------------------------

## 📌 Khái quát về dự án

Đề tài **Robot Delta phân loại bánh ngọt** tập trung nghiên cứu, thiết
kế và chế tạo mô hình **Robot Delta 3 bậc tự do (3-DOF)** phục vụ cho
việc gắp -- phân loại sản phẩm nhẹ trên băng tải. Robot Delta được lựa
chọn nhờ khả năng làm việc tốc độ cao, gia tốc lớn, độ lặp lại tốt và
phù hợp với dây chuyền đóng gói thực phẩm.

Hệ thống được xây dựng theo hướng mô phỏng ứng dụng thực tế, gồm các
khối chính: - **Cụm cơ khí Robot Delta** 
- **Bộ điều khiển:** PLC **Siemens S7-1200 (CPU 1214C DC/DC/DC)**.
- **Truyền động:** 3 **servo Mitsubishi HF-KP13B** kết hợp **driver
MR-J3-10A**, điều khiển bằng xung PTO từ PLC.
- **Thị giác máy:** camera **Logitech C615** thu ảnh băng tải.
- **AI nhận dạng:** mô hình **YOLOv8** (ONNX) phát hiện bánh, lấy tâm
vật thể và quy đổi toạ độ phục vụ gắp.
- **Phần mềm điều khiển -- giám sát:** ứng dụng **C# WinForms** để hiển
thị camera, kết nối PLC (Snap7), điều khiển jog và gửi toạ độ XYZ.

Mục tiêu của dự án là xây dựng một mô hình hoàn chỉnh thể hiện chuỗi xử
lý: **"Nhận dạng bằng AI → Xử lý toạ độ → Điều khiển robot Delta gắp và
phân loại sản phẩm"**, ứng dụng thực tiễn trong dây chuyền đóng gói bánh
kẹo.

------------------------------------------------------------------------

## 🤖 Hình ảnh mô hình Robot Delta

*(Thêm ảnh robot_delta.png nếu có)*

------------------------------------------------------------------------

## 🙏 Lời cảm ơn

Nhóm sinh viên xin chân thành cảm ơn **TS. Trần Mạnh Sơn** đã tận tình
hướng dẫn, hỗ trợ và định hướng trong suốt quá trình thực hiện đề tài.
Những góp ý chuyên môn của thầy đã giúp nhóm hoàn thiện hơn cả về lý
thuyết lẫn kỹ năng thực nghiệm.

Nhóm cũng xin gửi lời cảm ơn đến khoa **Điện -- Điện Tử**, Đại học Sư
Phạm Kỹ Thuật TP.HCM, đã tạo điều kiện về cơ sở vật chất và môi trường
học tập để nhóm thực hiện đồ án này.

------------------------------------------------------------------------

*README sẽ tiếp tục được cập nhật khi dự án hoàn thiện.*
