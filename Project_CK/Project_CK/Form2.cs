using Emgu.CV;
using Emgu.CV.CvEnum;
using Sharp7;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace Project_CK
{
    public partial class Form2 : Form
    {
        public double ee = 65;     // end effector
        public double ff = 35;// base
        public double re = 230;
        public double rf = 150;
        const Double sqrt3 = 1.732;
        const Double pi = 3.1415;    // PI
        const Double sin120 = 0.866;
        const Double cos120 = -0.5;
        const Double tan60 = 1.732;
        const Double sin30 = 0.5;
        const Double tan30 = 0.577;
        public S7Client plc;

        private VideoCapture _cap;
        private CancellationTokenSource _cts;
        private Task _captureTask;
        private System.Windows.Forms.Timer _uiTimer;

        // shared khung hình mới nhất để UI lấy
        private Bitmap _latestBmp; // dùng Interlocked/lock để đổi

        const int WIDTH = 640, HEIGHT = 480, TARGET_FPS = 30, CAM_INDEX = 1;

        public Form2()
        {
            InitializeComponent();
            plc = new S7Client();

            // cấu hình combobox PLC
            combox_plc.Items.Add("192.168.0.1");   // ví dụ IP S7-1200
            combox_plc.Text = "192.168.0.1";       // giá trị mặc định
            combox_plc.DropDownStyle = ComboBoxStyle.DropDown;
            UpdateStatus(false);
            trackBar_dosang.AutoSize = false;
            trackBar_saturation.AutoSize = false;
            trackBar_zoom.AutoSize = false;
            trackBar_dosang.Height = 30;   // nhỏ gọn
            trackBar_dosang.Minimum = 10;  // fps thấp nhất
            trackBar_dosang.Maximum = 60;  // fps cao nhất
            trackBar_dosang.Value = 25;
            trackBar_saturation.Height = 30;   // nhỏ gọn
            trackBar_saturation.Minimum = -100;  // fps thấp nhất
            trackBar_saturation.Maximum = 100;  // fps cao nhất
            trackBar_saturation.Value = 25;
            trackBar_zoom.Height = 30;   // nhỏ gọn
            trackBar_zoom.Minimum = 0;  // fps thấp nhất
            trackBar_zoom.Maximum = 10;  // fps cao nhất
            trackBar_zoom.Value = 0;


        }
        ////////////////////////////////
        int delta_calcAngleYZ(Double x0, Double y0, Double z0, ref Double theta)
        {


            Double y1 = -0.5 * 0.57735 * ff; // f/2 * tg 30     A
            y0 -= 0.5 * 0.57735 * ee;    // shift center to edge    B
                                         // z = a + b*y
            Double a = (x0 * x0 + y0 * y0 + z0 * z0 + rf * rf - re * re - y1 * y1) / (2 * z0);  //C
            Double b = (y1 - y0) / z0;      //D
                                            // discriminant
            Double d = -(a + b * y1) * (a + b * y1) + rf * (b * b * rf + rf);//G
            if (d < 0) return -1; // non-existing point
            Double yj = (y1 - a * b - Math.Sqrt(d)) / (b * b + 1); // choosing outer point
            Double zj = a + b * yj;
            theta = 180.0 * Math.Atan(-zj / (y1 - yj)) / pi + ((yj > y1) ? 180.0 : 0.0);
            return 0;
        }
        int delta_calcInverse(Double x0, Double y0, Double z0, ref Double theta1, ref Double theta2, ref Double theta3)
        {
            theta1 = theta2 = theta3 = 0;
            int status = delta_calcAngleYZ(x0, y0, z0, ref theta1);
            if (status == 0) status = delta_calcAngleYZ(x0 * cos120 + y0 * sin120, y0 * cos120 - x0 * sin120, z0, ref theta2);  // rotate coords to +120 deg
            if (status == 0) status = delta_calcAngleYZ(x0 * cos120 - y0 * sin120, y0 * cos120 + x0 * sin120, z0, ref theta3);  // rotate coords to -120 deg
            return status;
        }
        int delta_calcForward(Double theta1, Double theta2, Double theta3, ref Double x0, ref Double y0, ref Double z0)
        {
            Double t = (ff - ee) * tan30 / 2;
            Double dtr = pi / 180.0;

            theta1 *= dtr;
            theta2 *= dtr;
            theta3 *= dtr;

            Double y1 = -(t + rf * Math.Cos(theta1));
            Double z1 = -rf * Math.Sin(theta1);

            Double y2 = (t + rf * Math.Cos(theta2)) * sin30;
            Double x2 = y2 * tan60;
            Double z2 = -rf * Math.Sin(theta2);

            Double y3 = (t + rf * Math.Cos(theta3)) * sin30;
            Double x3 = -y3 * tan60;
            Double z3 = -rf * Math.Sin(theta3);

            Double dnm = (y2 - y1) * x3 - (y3 - y1) * x2;

            Double w1 = y1 * y1 + z1 * z1;
            Double w2 = x2 * x2 + y2 * y2 + z2 * z2;
            Double w3 = x3 * x3 + y3 * y3 + z3 * z3;

            // x = (a1*z + b1)/dnm
            Double a1 = (z2 - z1) * (y3 - y1) - (z3 - z1) * (y2 - y1);
            Double b1 = -((w2 - w1) * (y3 - y1) - (w3 - w1) * (y2 - y1)) / 2.0;

            // y = (a2*z + b2)/dnm;
            Double a2 = -(z2 - z1) * x3 + (z3 - z1) * x2;
            Double b2 = ((w2 - w1) * x3 - (w3 - w1) * x2) / 2.0;

            // a*z^2 + b*z + c = 0
            Double a = a1 * a1 + a2 * a2 + dnm * dnm;
            Double b = 2 * (a1 * b1 + a2 * (b2 - y1 * dnm) - z1 * dnm * dnm);
            Double c = (b2 - y1 * dnm) * (b2 - y1 * dnm) + b1 * b1 + dnm * dnm * (z1 * z1 - re * re);

            // discriminant
            Double d = b * b - (Double)4.0 * a * c;
            if (d < 0)
            {
                return -1; // non-existing point
            }

            z0 = -0.5 * (b + Math.Sqrt(d)) / a;
            x0 = (a1 * z0 + b1) / dnm;
            y0 = (a2 * z0 + b2) / dnm;
            return 0;
        }
        ////////////////////////////////
        private void label4_Click(object sender, EventArgs e)
        {

        }
        ////////////////////////////////
        private void UpdateStatus(bool connected)
        {
            if (connected)
            {
                label_status_connection.Text = "Đã kết nối";
                label_status_connection.ForeColor = Color.Green;
            }
            else
            {
                label_status_connection.Text = "Chưa kết nối";
                label_status_connection.ForeColor = Color.Red;
            }
        }
        private void btn_connect_Click(object sender, EventArgs e)
        {
            string ip = combox_plc.Text.Trim();
            if (string.IsNullOrEmpty(ip))
            {
                MessageBox.Show("Vui lòng nhập IP PLC!");
                return;
            }

            Task.Run(() =>
            {
                int result = plc.ConnectTo(ip, 0, 1);  // Rack=0, Slot=1
                if (result == 0)
                {
                    this.Invoke((MethodInvoker)(() =>
                    {
                        UpdateStatus(true);
                    }));
                }
                else
                {
                    this.Invoke((MethodInvoker)(() =>
                    {
                        UpdateStatus(false);
                        MessageBox.Show("Kết nối thất bại. Mã lỗi: " + result);
                    }));
                }
            });
        }

        private void btn_disconnect_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                if (plc != null && plc.Connected)
                {
                    plc.Disconnect();
                    this.Invoke((MethodInvoker)(() =>
                    {
                        UpdateStatus(false);
                    }));
                }
                else
                {
                    this.Invoke((MethodInvoker)(() =>
                    {
                        UpdateStatus(false);
                        MessageBox.Show("PLC chưa được kết nối!");
                    }));
                }
            });
        }
        ////////////////////////////////
        private void trackBar1_Scroll(object sender, EventArgs e)
        {

        }
        ////////////////////////////////
        private void btn_startvideo_Click(object sender, EventArgs e)
        {
            if (_cap != null) return;

            _cap = new VideoCapture(CAM_INDEX, VideoCapture.API.DShow);
            try { _cap.Set(CapProp.FrameWidth, WIDTH); } catch { }
            try { _cap.Set(CapProp.FrameHeight, HEIGHT); } catch { }
            try { _cap.Set(CapProp.Fps, TARGET_FPS); } catch { }
            try { _cap.Set(CapProp.FourCC, VideoWriter.Fourcc('M', 'J', 'P', 'G')); } catch { } // giúp giữ FPS với webcam UVC

            _cts = new CancellationTokenSource();

            // Luồng capture: đọc liên tục để không dồn buffer
            _captureTask = Task.Run(() =>
            {
                using (var frame = new Mat())
                {
                    while (!_cts.IsCancellationRequested)
                    {
                        if (!_cap.Read(frame) || frame.IsEmpty) { Thread.Sleep(1); continue; }
                        var bmp = frame.ToBitmap();

                        // thay thế bitmap cũ bằng cái mới nhất
                        Bitmap old = Interlocked.Exchange(ref _latestBmp, bmp);
                        old?.Dispose();
                    }
                }
            });

            // UI vẽ ~30 fps (tách khỏi capture)
            _uiTimer = new System.Windows.Forms.Timer { Interval = 1000 / TARGET_FPS };
            _uiTimer.Tick += (s, ev) =>
            {
                var bmp = Interlocked.Exchange(ref _latestBmp, null);
                if (bmp != null)
                {
                    var old = pictureBox.Image;
                    pictureBox.Image = bmp;
                    old?.Dispose();
                }
            };
            _uiTimer.Start();
        }

        private void btn_stopvideo_click(object sender, EventArgs e)
        {
            StopCamera();
        }
        private void StopCamera()
        {
            _uiTimer?.Stop();
            _uiTimer = null;

            try { _cts?.Cancel(); } catch { }
            try { _captureTask?.Wait(100); } catch { }
            _captureTask = null;

            try { _cap?.Dispose(); } catch { }
            _cap = null;

            var last = Interlocked.Exchange(ref _latestBmp, null);
            last?.Dispose();

            var old = pictureBox.Image;
            pictureBox.Image = null;
            old?.Dispose();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            StopCamera();
            base.OnFormClosing(e);
        }
        ////////////////////////////////
        private string FormatValue(double v)
        {
            // Bước 1: làm tròn 2 số thập phân
            double rounded = Math.Round(v, 2);

            // Bước 2: nếu gần 0 thì coi là 0
            if (Math.Abs(rounded) < 0.001)
                return "0";

            // Bước 3: nếu sau khi làm tròn là số nguyên → hiển thị nguyên
            if (Math.Abs(rounded % 1) < 0.001)
                return ((int)rounded).ToString();

            // Bước 4: còn lại hiển thị 2 số thập phân
            return rounded.ToString("0.##"); // bỏ .00 nếu không cần
        }
        ////////////////////////////////
        private void btn_FWD_click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox_theta1.Text) ||
                string.IsNullOrWhiteSpace(textBox_theta2.Text) ||
                string.IsNullOrWhiteSpace(textBox_theta3.Text))
            {
                MessageBox.Show("Vui lòng nhập đủ giá trị Theta1, Theta2, Theta3!",
                                "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // dừng luôn, không chạy tiếp
            }
            // Thử parse dữ liệu (đề phòng nhập chữ, ký tự lạ)
            if (!double.TryParse(textBox_theta1.Text, out double Theta1_tb) ||
                !double.TryParse(textBox_theta2.Text, out double Theta2_tb) ||
                !double.TryParse(textBox_theta3.Text, out double Theta3_tb))
            {
                MessageBox.Show("Giá trị nhập vào không hợp lệ! Vui lòng nhập số.",
                                "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Double X_view = 0;
            Double Y_view = 0;
            Double Z_view = 0;
            int Status1 = delta_calcForward(Theta1_tb, Theta2_tb, Theta3_tb, ref X_view, ref Y_view, ref Z_view);
            if (Status1 == 0)
            {
                textBox_view_x.Text = FormatValue(X_view);
                textBox_view_y.Text = FormatValue(Y_view);
                textBox_view_z.Text = FormatValue(Z_view);
            }
            else
            {
                textBox_view_x.Text = " Ø ";
                textBox_view_y.Text = " Ø ";
                textBox_view_z.Text = " Ø ";

            }
        }

        private void btn_INV_click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox_x.Text) ||
                string.IsNullOrWhiteSpace(textBox_y.Text) ||
                string.IsNullOrWhiteSpace(textBox_z.Text))
            {
                MessageBox.Show("Vui lòng nhập đủ giá trị X, Y, Z!",
                                "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // dừng luôn, không chạy tiếp
            }
            // Thử parse dữ liệu (đề phòng nhập chữ, ký tự lạ)
            if (!double.TryParse(textBox_x.Text, out double x_tb) ||
                !double.TryParse(textBox_y.Text, out double y_tb) ||
                !double.TryParse(textBox_z.Text, out double z_tb))
            {
                MessageBox.Show("Giá trị nhập vào không hợp lệ! Vui lòng nhập số.",
                                "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Double theta1_view = 0;
            Double theta2_view = 0;
            Double theta3_view = 0;
            int Status2 = delta_calcInverse(x_tb, y_tb, z_tb, ref theta1_view, ref theta2_view, ref theta3_view);
            if (Status2 == 0)
            {
                textBox_view_theta1.Text = FormatValue(theta1_view);
                textBox_view_theta2.Text = FormatValue(theta2_view);
                textBox_view_theta3.Text = FormatValue(theta3_view);
            }
            else
            {
                textBox_view_theta1.Text = " Ø ";
                textBox_view_theta2.Text = " Ø ";
                textBox_view_theta3.Text = " Ø ";
            }

        }

        private void btn_push_click(object sender, EventArgs e)
        {
            if (!plc.Connected)
            {
                MessageBox.Show("PLC chưa kết nối.");
                return;
            }
            Double X_push = Convert.ToDouble(textBox_push_x.Text);
            Double Y_push = Convert.ToDouble(textBox_push_y.Text);
            Double Z_push = Convert.ToDouble(textBox_push_z.Text);
            byte[] buf = new byte[4];
        }
        ////////////////////////////////
    }
}
