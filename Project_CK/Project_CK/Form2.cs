using Emgu.CV;
using Emgu.CV.CvEnum;
using Sharp7;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using WinTimer = System.Windows.Forms.Timer;


namespace Project_CK
{
    public partial class Form2 : Form
    {
        public double ee = 65;     // end effector
        public double ff = 35;// base
        public double re = 230;
        public double rf = 150;
        const Double sqrt3 = 1.732;
        const Double pi = 3.141593;    // PI
        const Double sin120 = 0.8660254;
        const Double cos120 = -0.5;
        const Double tan60 = 1.732;
        const Double sin30 = 0.5;
        const Double tan30 = 0.57735;
        public S7Client plc;

        const int DB_CMD = 33; // DB điều khiển
        const int DB_FB = 18; // DB phản hồi
        private int _seq = 0 ;
        private System.Windows.Forms.Timer fbTimer;


        private VideoCapture _cap;
        private CancellationTokenSource _cts;
        private Task _captureTask;
        private System.Windows.Forms.Timer _uiTimer;

        private YoloOnnxSafe _yolo;
        private readonly string[] _coco = new string[] {
     "person","bicycle","car","motorcycle","airplane","bus","train","truck","boat","traffic light",
     "fire hydrant","stop sign","parking meter","bench","bird","cat","dog","horse","sheep","cow",
    "elephant","bear","zebra","giraffe","backpack","umbrella","handbag","tie","suitcase","frisbee",
     "skis","snowboard","sports ball","kite","baseball bat","baseball glove","skateboard","surfboard","tennis racket",
     "bottle","wine glass","cup","fork","knife","spoon","bowl","banana","apple","sandwich","orange","broccoli","carrot",
     "hot dog","pizza","donut","cake","chair","couch","potted plant","bed","dining table","toilet","tv","laptop","mouse",
     "remote","keyboard","cell phone","microwave","oven","toaster","sink","refrigerator","book","clock","vase",
     "scissors","teddy bear","hair drier","toothbrush"
        };

        private WinTimer plcTimer;

        // shared khung hình mới nhất để UI lấy
        private Bitmap _latestBmp; // dùng Interlocked/lock để đổi

        const int WIDTH = 640, HEIGHT = 640, TARGET_FPS = 30, CAM_INDEX = 1;

        public Form2()
        {
            InitializeComponent();
            plc = new S7Client();

            _yolo = new YoloOnnxSafe("yolov8n.onnx", _coco, useDirectML: false, inputW: 640, inputH: 640)
            {
                ScoreThresh = 0.25f,
                NmsThresh = 0.45f
            };

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
            numericUpDown_z.Minimum = -400;   // giới hạn nhỏ nhất
            numericUpDown_z.Maximum = -200;
            numericUpDown_z.Increment = 0.1M;
            numericUpDown_x.Minimum = -100;   // giới hạn nhỏ nhất
            numericUpDown_x.Maximum = 100;
            numericUpDown_x.Increment = 0.1M;
            numericUpDown_y.Minimum = -100;   // giới hạn nhỏ nhất
            numericUpDown_y.Maximum = 100;
            numericUpDown_y.Increment = 0.1M;
            numericUpDown_vel.Minimum = 0;
            numericUpDown_vel.Maximum = 500;
            numericUpDown_y.Increment = 1;

            System.Windows.Forms.Timer fbTimer = new System.Windows.Forms.Timer();
            fbTimer = new System.Windows.Forms.Timer();
            fbTimer.Interval = 200;
            fbTimer.Tick += (s, e) => RefreshFeedbackUi();
            fbTimer.Start();

            plcTimer = new WinTimer();
            plcTimer.Interval = 500;
            plcTimer.Tick += viewplc;


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
            Double yj = (y1 - a * b - Math.Sqrt(d)) / (b * b + 1); // choosing outer point H
            Double zj = a + b * yj; /// I
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
            plcTimer.Start(); 
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
                    plcTimer.Stop();
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
            try { _cap.Set(CapProp.FourCC, VideoWriter.Fourcc('M', 'J', 'P', 'G')); } catch { } // giúp giữ FPS với UVC

            _cts = new CancellationTokenSource();

            // Luồng capture: đọc liên tục, YOLO mỗi n khung để nhẹ máy
            _captureTask = Task.Run(() =>
            {
                using (var frame = new Mat())
                {
                    int frameCount = 0;
                    while (!_cts.IsCancellationRequested)
                    {
                        if (!_cap.Read(frame) || frame.IsEmpty) { Thread.Sleep(1); continue; }

                        using var srcBmp = frame.ToBitmap();
                        var drawBmp = (Bitmap)srcBmp.Clone();

                        // Để nhẹ máy: detect mỗi 2 khung (bạn có thể đổi %1 để detect mọi khung)
                        if ((++frameCount % 1) == 0)
                        {
                            var dets = _yolo.Infer(srcBmp);

                            using var g = Graphics.FromImage(drawBmp);
                            using var pen = new Pen(Color.Lime, 2);
                            using var font = new Font("Segoe UI", 9);

                            foreach (var d in dets)
                            {
                                g.DrawRectangle(pen, d.Rect.X, d.Rect.Y, d.Rect.Width, d.Rect.Height);
                                var text = $"{d.Label} {d.Score:0.00}";
                                var sz = g.MeasureString(text, font);
                                g.FillRectangle(Brushes.Black, d.Rect.X, d.Rect.Y - sz.Height, sz.Width, sz.Height);
                                g.DrawString(text, font, Brushes.Yellow, d.Rect.X, d.Rect.Y - sz.Height);
                            }
                        }

                        var old = Interlocked.Exchange(ref _latestBmp, drawBmp);
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
        ////////////////////////////////
        public class YoloOnnxSafe
        {
            public record Det(RectangleF Rect, float Score, int ClassId, string Label);

            private readonly InferenceSession _sess;
            private readonly int _inpW, _inpH;
            private readonly string _inputName;
            private readonly string _outputName;
            private readonly string[] _names;

            public float ScoreThresh { get; set; } = 0.1f;
            public float NmsThresh { get; set; } = 0.45f;

            public YoloOnnxSafe(string onnxPath, string[] classNames, bool useDirectML = false, int inputW = 640, int inputH = 640)
            {
                var opt = new SessionOptions();
                if (useDirectML)
                {
                    // Cần Microsoft.ML.OnnxRuntime.DirectML
                    opt.AppendExecutionProvider_DML();
                }
                else
                {
                    opt.AppendExecutionProvider_CPU();
                }

                _sess = new InferenceSession(onnxPath, opt);
                _inpW = inputW; _inpH = inputH;
                _inputName = _sess.InputMetadata.Keys.First();
                _outputName = PickOutputName(_sess);
                _names = classNames ?? Array.Empty<string>();
            }

            public List<Det> Infer(Bitmap bgr)
            {
                var (resized, scale, padX, padY) = Letterbox(bgr, _inpW, _inpH);

                var input = new DenseTensor<float>(new[] { 1, 3, _inpH, _inpW });
                var data = BitmapToBytes24(resized); // BGR bytes
                int idx = 0;
                for (int y = 0; y < _inpH; y++)
                {
                    for (int x = 0; x < _inpW; x++)
                    {
                        byte B = data[idx++], G = data[idx++], R = data[idx++];
                        input[0, 0, y, x] = R / 255f;
                        input[0, 1, y, x] = G / 255f;
                        input[0, 2, y, x] = B / 255f;
                    }
                }
                resized.Dispose();

                // ✅ Không dùng using var ở đây nữa
                var inputs = new List<NamedOnnxValue>
    {
        NamedOnnxValue.CreateFromTensor(_inputName, input)
    };

                // ✅ Chỉ cần using cho results thôi
                using var results = _sess.Run(inputs);

                var t = results.First(o => o.Name == _outputName).AsTensor<float>();

                var dets = ParseDetections(t, scale, padX, padY, bgr.Width, bgr.Height);
                return Nms(dets, NmsThresh);
            }

            // ---------- Helpers ----------
            private static (Bitmap bmp, float scale, int padX, int padY) Letterbox(Bitmap src, int dstW, int dstH)
            {
                float r = Math.Min(dstW / (float)src.Width, dstH / (float)src.Height);
                int nw = (int)Math.Round(src.Width * r);
                int nh = (int)Math.Round(src.Height * r);
                int padX = (dstW - nw) / 2;
                int padY = (dstH - nh) / 2;

                var canvas = new Bitmap(dstW, dstH, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                using var g = Graphics.FromImage(canvas);
                g.Clear(Color.Black);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
                g.DrawImage(src, new Rectangle(padX, padY, nw, nh));
                return (canvas, r, padX, padY);
            }

            private static byte[] BitmapToBytes24(Bitmap bmp)
            {
                var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                var bd = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly,
                                      System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                int bytes = Math.Abs(bd.Stride) * bd.Height;
                byte[] buffer = new byte[bytes];
                Marshal.Copy(bd.Scan0, buffer, 0, bytes);
                bmp.UnlockBits(bd);
                return buffer;
            }

            private static string PickOutputName(InferenceSession s)
                => s.OutputMetadata.Keys.First(); // đa số model Ultralytics có 1 output; nếu nhiều, chọn cái đầu

            private System.Collections.Generic.List<Det> ParseDetections(Tensor<float> t, float scale, int padX, int padY, int origW, int origH)
            {
                var list = new System.Collections.Generic.List<Det>();

                if (t.Dimensions.Length == 3 && t.Dimensions[1] == 84) // [1,84,8400]
                {
                    int num = t.Dimensions[2];
                    for (int i = 0; i < num; i++)
                    {
                        float x = t[0, 0, i], y = t[0, 1, i], w = t[0, 2, i], h = t[0, 3, i];
                        int best = -1; float conf = 0f;
                        for (int c = 4; c < 84; c++)
                        {
                            float s = t[0, c, i];
                            if (s > conf) { conf = s; best = c - 4; }
                        }
                        if (conf < ScoreThresh) continue;

                        var rect = UnletterBox(x, y, w, h, scale, padX, padY, origW, origH);
                        string label = (best >= 0 && best < _names.Length) ? _names[best] : $"id{best}";
                        list.Add(new Det(rect, conf, best, label));
                    }
                }
                else // [1,8400,84]
                {
                    int num = t.Dimensions[1];
                    for (int i = 0; i < num; i++)
                    {
                        float x = t[0, i, 0], y = t[0, i, 1], w = t[0, i, 2], h = t[0, i, 3];
                        int best = -1; float conf = 0f;
                        int ch = t.Dimensions[2];
                        for (int c = 4; c < ch; c++)
                        {
                            float s = t[0, i, c];
                            if (s > conf) { conf = s; best = c - 4; }
                        }
                        if (conf < ScoreThresh) continue;

                        var rect = UnletterBox(x, y, w, h, scale, padX, padY, origW, origH);
                        string label = (best >= 0 && best < _names.Length) ? _names[best] : $"id{best}";
                        list.Add(new Det(rect, conf, best, label));
                    }
                }
                return list;
            }

            private static RectangleF UnletterBox(float cx, float cy, float w, float h, float scale, int padX, int padY, int ow, int oh)
            {
                float x1 = cx - w / 2f, y1 = cy - h / 2f;
                float x2 = cx + w / 2f, y2 = cy + h / 2f;
                float gx1 = (x1 - padX) / scale;
                float gy1 = (y1 - padY) / scale;
                float gx2 = (x2 - padX) / scale;
                float gy2 = (y2 - padY) / scale;
                return RectangleF.FromLTRB(
                    Clamp(gx1, 0, ow), Clamp(gy1, 0, oh),
                    Clamp(gx2, 0, ow), Clamp(gy2, 0, oh));
            }

            private static float IoU(RectangleF a, RectangleF b)
            {
                float x1 = Math.Max(a.Left, b.Left);
                float y1 = Math.Max(a.Top, b.Top);
                float x2 = Math.Min(a.Right, b.Right);
                float y2 = Math.Min(a.Bottom, b.Bottom);
                float inter = Math.Max(0, x2 - x1) * Math.Max(0, y2 - y1);
                float uni = a.Width * a.Height + b.Width * b.Height - inter + 1e-6f;
                return inter / uni;
            }

            private static System.Collections.Generic.List<Det> Nms(System.Collections.Generic.List<Det> boxes, float thr)
            {
                var res = new System.Collections.Generic.List<Det>();
                foreach (var grp in boxes.GroupBy(b => b.ClassId))
                {
                    var s = grp.OrderByDescending(b => b.Score).ToList();
                    while (s.Count > 0)
                    {
                        var m = s[0]; res.Add(m); s.RemoveAt(0);
                        s = s.Where(b => IoU(b.Rect, m.Rect) < thr).ToList();
                    }
                }
                return res;
            }

            // .NET Framework 4.x không có Math.Clamp
            private static float Clamp(float v, float min, float max) => (v < min) ? min : (v > max ? max : v);
        }

        private void viewplc(object sender, EventArgs e)
        {
            byte[] buf = new byte[12];
            int result = plc.DBRead(111, 0, buf.Length, buf);
            if (result == 0)
            {
                float x = S7.GetRealAt(buf, 0);
                float y = S7.GetRealAt(buf, 4);
                float z = S7.GetRealAt(buf, 8);

                double X_viewplc = 0, Y_viewplc = 0, Z_viewplc = 0;
                int Status1 = delta_calcForward(x * (-1) / 10, y * (-1) / 10, z / 10, ref X_viewplc, ref Y_viewplc, ref Z_viewplc);

                if (Status1 == 0)
                {
                    textBox_viewplc_x.Text = FormatValue(X_viewplc);
                    textBox_viewplc_y.Text = FormatValue(Y_viewplc);
                    textBox_viewplc_z.Text = FormatValue(Z_viewplc);
                }
                else
                {
                    textBox_viewplc_x.Text = "Ø";
                    textBox_viewplc_y.Text = "Ø";
                    textBox_viewplc_z.Text = "Ø";
                }
            }
            else
            {
                // trường hợp mất kết nối
                textBox_viewplc_x.Text = "---";
                textBox_viewplc_y.Text = "---";
                textBox_viewplc_z.Text = "---";
            }
        }
        ////////////////////////////////
        void WriteBoolBit(int db, int byteOffset, int bit, bool value)
        {
            byte[] b = new byte[1];
            int rc = plc.DBRead(db, byteOffset, 1, b);
            if (rc != 0) throw new Exception(plc.ErrorText(rc));

            S7.SetBitAt(b, 0, bit, value);  // bit = 0..7 trong byte
            rc = plc.DBWrite(db, byteOffset, 1, b);
            if (rc != 0) throw new Exception(plc.ErrorText(rc));
        }
        public bool ReadBoolBit(int db, int byteOffset, int bit)
        {
            byte[] b = new byte[1];
            int rc = plc.DBRead(db, byteOffset, 1, b);
            if (rc != 0) throw new Exception(plc.ErrorText(rc));

            return (b[0] & (1 << bit)) != 0;
        }
        public void WriteReal(int db, int byteOffset, float value)
        {
            byte[] buf = new byte[4];
            S7.SetRealAt(buf, 0, value);
            int rc = plc.DBWrite(db, byteOffset, buf.Length, buf);
            if (rc != 0) throw new Exception(plc.ErrorText(rc));
        }
        public float ReadReal(int db, int byteOffset)
        {
            byte[] buf = new byte[4];
            int rc = plc.DBRead(db, byteOffset, buf.Length, buf);
            if (rc != 0) throw new Exception(plc.ErrorText(rc));

            return S7.GetRealAt(buf, 0);
        }
        public void WriteDInt(int db, int byteOffset, int value)
        {
            byte[] buf = new byte[4];
            S7.SetDIntAt(buf, 0, value);
            int rc = plc.DBWrite(db, byteOffset, buf.Length, buf);
            if (rc != 0) throw new Exception(plc.ErrorText(rc));
        }
        public int ReadDInt(int db, int byteOffset)
        {
            byte[] buf = new byte[4];
            int rc = plc.DBRead(db, byteOffset, buf.Length, buf);
            if (rc != 0) throw new Exception(plc.ErrorText(rc));
            return S7.GetDIntAt(buf, 0);
        }

        void PulseExec( int db, int ofsBitByte, int bit, int ms = 50)
        {
            var b = new byte[1]; plc.DBRead(db, ofsBitByte, 1, b);
            b[0] = (byte)(b[0] | (1 << bit)); plc.DBWrite(db, ofsBitByte, 1, b);
            System.Threading.Thread.Sleep(ms);
            b[0] = (byte)(b[0] & ~(1 << bit)); plc.DBWrite(db, ofsBitByte, 1, b);
        }
        ////////////////////////////////
        // ====== Nội suy Cartesian → IK → gửi Absolute ======
        private async Task MoveLinearCartesianJointStreamAsync(
            double x0, double y0, double z0,
            double x1, double y1, double z1,
            double vTcp, int steps = 1000, int TsMs = 10)
        {
            double dx = x1 - x0, dy = y1 - y0, dz = z1 - z0;
            double L = Math.Sqrt(dx * dx + dy * dy + dz * dz);
            if (L < 1e-9) { await SendAbsAndWaitIkAsync(x1, y1, z1, vTcp); return; }

            for (int i = 0; i <= steps; i++)
            {
                double s = (double)i / steps;
                double x = x0 + dx * s;
                double y = y0 + dy * s;
                double z = z0 + dz * s;

                double th1 = 0, th2 = 0, th3 = 0;
                int st = delta_calcInverse(x, y, z, ref th1, ref th2, ref th3);
                if (st != 0) throw new Exception("Điểm vượt workspace.");

                await SendAbsAndWaitAsync((float)th1, (float)th2, (float)th3, (float)vTcp);
                await Task.Delay(TsMs);
            }
        }

        /// ======= Tốc độ nhỏ hơn 0 thì ======
        private async Task SendAbsAndWaitIkAsync(double x, double y, double z, double vTcp)
        {
            double th1 = 0, th2 = 0, th3 = 0;
            int st = delta_calcInverse(x, y, z, ref th1, ref th2, ref th3);
            if (st != 0) throw new Exception("Điểm đích vượt workspace.");

            await SendAbsAndWaitAsync((float)th1, (float)th2, (float)th3, (float)vTcp);
        }

        // ====== Gửi setpoint và chờ Done ======
        private async Task SendAbsAndWaitAsync(float th1, float th2, float th3, float vel)
        {
            SendJointSetpoint(th1, th2, th3, vel);
            //await Task.Delay(200);
            DateTime t0 = DateTime.UtcNow;
            while (true)
            {
                var fb = ReadFeedback();
                if (fb.Error) throw new Exception("PLC báo lỗi.");
                if (!fb.Done && !fb.Busy) break;
                if ((DateTime.UtcNow - t0).TotalSeconds > 8) throw new Exception("Timeout chờ Done.");
                await Task.Delay(20);
            }
        }

        // ====== Gửi xuống DB_CMD ======
        private void SendJointSetpoint(float th1, float th2, float th3, float vel)
        {
            
            WriteReal(DB_CMD, 0, th1);
            WriteReal(DB_CMD, 4, th2);
            WriteReal(DB_CMD, 8, th3);
            WriteReal(DB_CMD, 12, vel);
            WriteDInt(DB_CMD, 16, ++_seq);

        }
        // ====== Feedback ======
        private PlcFeedback ReadFeedback()
        {
            var fb = new PlcFeedback
            {
                Th1 = ReadReal(DB_FB, 0)/(-10),   // DBD0
                Th2 = ReadReal(DB_FB, 4)/(-10),   // DBD4
                Th3 = ReadReal(DB_FB, 8)/10,   // DBD8
                Busy = ReadBoolBit(DB_FB, 12, 0), // DBX12.0
                Done = ReadBoolBit(DB_FB, 12, 1), // DBX12.1
                Error = ReadBoolBit(DB_FB, 12, 2)  // DBX12.2
            };
            return fb;
        }
        private class PlcFeedback
        {
            public float Th1, Th2, Th3;
            public bool Busy, Done, Error;
        }

        private void btn_home_click(object sender, EventArgs e)
        {
            if (!plc.Connected)
            {
                MessageBox.Show("PLC chưa kết nối.");
                return;
            }
            WriteReal(33, 12, (float)100.0);
            WriteBoolBit(33, 20, 0, true);
            WriteBoolBit(33, 20, 0, false);
        }

        private void RefreshFeedbackUi()
        {
            if (!plc.Connected) return;
            try
            {
                var fb = ReadFeedback();
                double x = 0, y = 0, z = 0;
                int st = delta_calcForward(fb.Th1, fb.Th2, fb.Th3, ref x, ref y, ref z);
                if (st == 0)
                {
                    textBox_ns_x.Text = x.ToString("0.00");
                    textBox_ns_y.Text = y.ToString("0.00");
                    textBox_ns_z.Text = z.ToString("0.00");
                }
            }
            catch { }
        }

        private async void btn_exc_click(object sender, EventArgs e)
        {
            try
            {
                if (!plc.Connected) { MessageBox.Show("PLC chưa kết nối"); return; }
                
                double x1 = (double)numericUpDown_x.Value;
                double y1 = (double)numericUpDown_y.Value;
                double z1 = (double)numericUpDown_z.Value;
                double vTcp = (double)numericUpDown_vel.Value;

                // Lấy điểm bắt đầu từ feedback (joint → FK)
                var fb = ReadFeedback();
                double x0 = 0, y0 = 0, z0 = 0;
                int stFK = delta_calcForward(fb.Th1, fb.Th2, fb.Th3, ref x0, ref y0, ref z0);
                if (stFK != 0) { MessageBox.Show("FK lỗi từ feedback"); return; }

                await MoveLinearCartesianJointStreamAsync(x0, y0, z0, x1, y1, z1, vTcp, TsMs: 20);
                MessageBox.Show("✅ Đã tới đích.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }

        }
    }
}
