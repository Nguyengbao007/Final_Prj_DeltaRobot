using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using Sharp7;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV.Util;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using WinTimer = System.Windows.Forms.Timer;


namespace Project_CK
{
    public partial class Form2 : Form
    {
        public double ee = 40;     // end effector
        public double ff = 60;// base
        public double re = 310;
        public double rf = 150;
        const Double sqrt3 = 1.732;
        const Double pi = 3.141593;    // PI
        const Double sin120 = 0.8660254;
        const Double cos120 = -0.5;
        const Double tan60 = 1.732;
        const Double sin30 = 0.5;
        const Double tan30 = 0.57735;

        /*
        const int DB_CMD = 33; // DB điều khiển
        const int DB_FB = 18; // DB phản hồi
        private int _seq = 0;
        private System.Windows.Forms.Timer fbTimer;
        */

        // ---------- Capture state ----------
        private VideoCapture _cap;
        private CancellationTokenSource _cts;
        private Task _captureTask;
        private System.Windows.Forms.Timer _uiTimer;
        private Bitmap _latestBmp; // dùng Interlocked/lock để đổi
 
        private readonly string[] _labels = { "Bo", "cookie_trang", "Cookie_xanh", "Dao", "Green", "Red", "Socola", "Xoai", "Yellow" };
        private const string MODEL_PATH = @"C:\Users\Hoang\Documents\10_20\Project_CK\Project_CK\best (2).onnx";
        
        
        private volatile int _brightness = 0;   // -100..+100

        private WinTimer plcTimer;
        private void EnsureYoloLoaded()
        {
            if (_yolo != null) return;

            if (!System.IO.File.Exists(MODEL_PATH))
            {
                MessageBox.Show($"Không tìm thấy model: {MODEL_PATH}");
                return;
            }

            try
            {
                _yolo = new YoloOnnxSafe(
                    onnxPath: MODEL_PATH,
                    classNames: _labels,
                    useDirectML: false,   // bật true nếu bạn có DML GPU
                    inputW: 640, inputH: 640
                )
                {
                    ScoreThresh = 0.30f,
                    NmsThresh = 0.45f
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show("Load YOLO thất bại: " + ex.Message);
                _yolo = null;
            }
        }
        // shared khung hình mới nhất để UI lấy
        

        // ---------- CAMERA CONFIG ----------
        private const int CAM_INDEX = 1;
        private const int WIDTH = 640;
        private const int HEIGHT = 640;
        private const int TARGET_FPS = 30;
        private YoloOnnxSafe _yolo;
        private bool _roiEnabled = true;
        private Rectangle _roiDisp = Rectangle.Empty; // ROI cố định theo ảnh hiển thị
        public struct Track
        {
            public int Id;
            public RectangleF Rect;
            public PointF Center;
            public int ClassId;
            public string Label;
            public int Missed;
            public PointF LastInRoiCenter;
            public DateTime? Missed2AtUtc;

            // ➕ THÊM 3 TRƯỜNG NÀY
            public double X0_mm;        // X (mm) tại thời điểm rời ROI (Missed==2)
            public double Y0_mm;        // Y (mm) tại thời điểm rời ROI (Missed==2)
            public double XRobotFixed;  // Giá trị X (sau offset/rotate) cố định để gửi liên tục
        }
        private readonly Dictionary<int, Track> _tracks = new Dictionary<int, Track>();
        private int _nextTrackId = 1;
        private const float MATCH_MAX_DIST_PX = 60f;
        // Scale pixel -> DINT (tuỳ chỉnh)
        private const int SCALE_NUM = 1;
        private const int SCALE_DEN = 1;
        // ---------- PLC ----------
        public S7Client plc;
        private CancellationTokenSource _plcCts;
        private Task _plcTask;
        // ---------- UI ----------
        private Button btnStart, btnStop;
        // --- DB & Offset (SỬA CHO KHỚP PLC CỦA BẠN) ---
        private const int DB_NUMBER = 49;   // ví dụ DB1
        private const int OFF_X_DINT = 0;  // DBB0..3
        private const int OFF_Y_DINT = 4;  // DBB4..7
        private const int OFF_T_INT = 12;  // DBB8..9
        private const int OFF_Z_DINT = 8;


        //10_15
        // --- Lọc box nhỏ ---
        private const int MIN_W_PX = 24;       // bề rộng tối thiểu (px)
        private const int MIN_H_PX = 24;       // bề cao  tối thiểu (px)
        private const int MIN_AREA_PX = 24 * 24;  // diện tích tối thiểu (px^2)
        private const float MIN_ASPECT = 0.5f;     // w/h tối thiểu
        private const float MAX_ASPECT = 2.0f;     // w/h tối đa
        private const float MIN_ROI_AREA_RATIO = 0.004f; // 0.4% diện tích ROI (lọc theo tỉ lệ)
        private const float CONTAIN_SUPPRESS_IOU = 0.5f; // nms đơn giản cho box chồng
        // ===== Meta cho letterbox raw -> 640x640 =====
        struct ResizeMeta
        {
            public int OrigW, OrigH;     // kích thước ảnh gốc (vd 1920x1080)
            public int NewW, NewH;       // 640x640
            public double Scale;         // = Min(640/OrigW, 640/OrigH)
            public int PadX, PadY;       // viền đen (letterbox)
        }
        private ResizeMeta _lastResizeMeta;
        // ===== Calib camera
        // --- Z0 của mặt phẳng làm việc (mm) ---
        private const double Z0_MM = 0.0;          // mặt phẳng băng tải là Z=0
        private const double CAM_HEIGHT = -300.0;   // camera cao 310mm so với mặt phẳng

        // --- K (1920x1080) bạn đã cung cấp ---
        private readonly double[,] _K_1920 = new double[,]
        {
    { 1474.9241195700, 0.0000000000, 953.0940681343 },
    { 0.0000000000, 1476.6458229523, 526.5786938637 },
    { 0.0000000000, 0.0000000000, 1.0000000000 }
        };

        // Nếu chưa có distortion thực, tạm dùng zero
        private readonly double[] _dist_1920 = new double[] { 0, 0, 0, 0, 0 };

        // --- Extrinsic world->camera (Rcw, tcw) cho top-down (không nghiêng) ---
 private readonly double[,] _Rcw = new double[,]
{
    { 1, 0, 0 },
    { 0, 1, 0 },
    { 0, 0, 1 }
};


        // t = (0, 0, 310) để tâm camera Cw = -R^T t = (0,0,310)
        private readonly double[] _tcw = new double[] { 0, 0, CAM_HEIGHT };
        private readonly Queue<(int X, int Y, int Z, short Type)> _plcQueue = new();
        private readonly List<(DateTime DueUtc, int X, int Y, int Z, short Type)> _delayedPlc = new();

        // vị trí mm tại thời điểm Missed==2
        private readonly Dictionary<int, double> _exitMmX = new();
        private readonly Dictionary<int, double> _exitMmY = new();

        // hàng đợi gửi trễ (bạn đang có sẵn _delayedPlc + SEND_DELAY_S)
        private const double SEND_DELAY_S = 7.0;          // gửi sau 7 giây kể từ Missed==2
        private const int BELT_DIR_X = +1;           // +1 dọc +X; -1 dọc -X; 0 nếu băng tải không theo X
        private const int BELT_DIR_Y = 0;           // +1 dọc +Y; -1 dọc -Y; 0 nếu băng tải không theo Y
        private const double BELT_SPEED_MM_PER_S = 200.0; // có thể cập nhật runtime theo PLC
        private const double OUT_LIFETIME_S = 12.0;       // xoá track sau 12s kể từ Missed==2

        public Form2()
        {
            InitializeComponent();
            plc = new S7Client();
            var labels = new[]
 {
    "Bo", "cookie_trang", "Cookie_xanh", "Dao",
    "Green", "Red", "Socola", "Xoai", "Yellow"
};

            var yolo = new YoloOnnxSafe(
                onnxPath: @"C:\Users\Hoang\Documents\10_20\Project_CK\Project_CK\best (2).onnx",
                classNames: labels,
                useDirectML: false,
                inputW: 640, inputH: 640
            )
            {
                ScoreThresh = 0.80f,
                NmsThresh = 0.45f
            };


            // cấu hình combobox PLC
            combox_plc.Items.Add("192.168.0.1");   // ví dụ IP S7-1200
            combox_plc.Text = "192.168.0.1";       // giá trị mặc định
            combox_plc.DropDownStyle = ComboBoxStyle.DropDown;
            UpdateStatus(false);
            numericUpDown_z.Minimum = -400;   // giới hạn nhỏ nhất
            numericUpDown_z.Maximum = -200;
            numericUpDown_z.Increment = 1M;
            numericUpDown_x.Minimum = -140;   // giới hạn nhỏ nhất
            numericUpDown_x.Maximum = 140;
            numericUpDown_x.Increment = 10M;
            numericUpDown_y.Minimum = -140;   // giới hạn nhỏ nhất
            numericUpDown_y.Maximum = 140;
            numericUpDown_y.Increment = 10M;
            numericUpDown_vel.Minimum = 0;
            numericUpDown_vel.Maximum = 3000;
            _roiDisp = new Rectangle(90, 200, 240, 280);
            /*
            System.Windows.Forms.Timer fbTimer = new System.Windows.Forms.Timer();
            fbTimer = new System.Windows.Forms.Timer();
            fbTimer.Interval = 200;
            fbTimer.Tick += (s, e) => RefreshFeedbackUi();
            fbTimer.Start();
            */
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
        ////////////////////////////////
        private void InitFixedRoiIfNeeded(int imgW, int imgH)
        {
            if (_roiDisp.Width > 0) return;
            // ví dụ: ROI = 50% chiều rộng x 45% chiều cao, đặt giữa
            int w = (int)(imgW * 0.50);
            int h = (int)(imgH * 0.45);
            int x = (imgW - w) / 2;
            int y = (imgH - h) / 2;
            _roiDisp = new Rectangle(x, y, w, h);
        }

        private void btn_startvideo_Click(object sender, EventArgs e)
        {
            if (_cap != null) return;

            EnsureYoloLoaded();          // nạp YOLO nếu chưa
            EnsurePlcSenderRunning();    // khởi chạy luồng gửi PLC nền

            _cap = new VideoCapture(1, VideoCapture.API.DShow);
            try { _cap.Set(CapProp.FrameWidth, WIDTH); } catch { }
            try { _cap.Set(CapProp.FrameHeight, HEIGHT); } catch { }
            try { _cap.Set(CapProp.Fps, TARGET_FPS); } catch { }
            try { _cap.Set(CapProp.FourCC, VideoWriter.Fourcc('M', 'J', 'P', 'G')); } catch { }

            _cts = new CancellationTokenSource();

            _captureTask = Task.Run(() =>
            {
                using var frame = new Mat();
                while (!_cts.IsCancellationRequested)
                {
                    if (!_cap.Read(frame) || frame.IsEmpty) { Thread.Sleep(1); continue; }

                    using (var rawBmp = frame.ToBitmap())
                    using (var srcBmp = (rawBmp.Width == 640 && rawBmp.Height == 640)
                                         ? (Bitmap)rawBmp.Clone()
                                         : PadToSquare640WithMeta(rawBmp, out _lastResizeMeta)) // lưu meta để map ngược khi gửi PLC
                    {
                        // nếu nguồn vốn đã là 640x640 thì set meta identity để pipeline thống nhất
                        if (rawBmp.Width == 640 && rawBmp.Height == 640)
                        {
                            _lastResizeMeta = new ResizeMeta
                            {
                                OrigW = 640,
                                OrigH = 640,
                                NewW = 640,
                                NewH = 640,
                                Scale = 1.0,
                                PadX = 0,
                                PadY = 0
                            };
                        }

                        var drawBmp = (Bitmap)srcBmp.Clone();

                        // 1) Khởi tạo ROI một lần (trên khung 640×640)
                        if (_roiEnabled && _roiDisp.Width <= 0)
                        {
                            int w = (int)(srcBmp.Width * 0.50);
                            int h = (int)(srcBmp.Height * 0.45);
                            int x = (srcBmp.Width - w) / 2;
                            int y = (srcBmp.Height - h) / 2;
                            _roiDisp = new Rectangle(x, y, w, h);
                        }

                        // 2) Vẽ ROI
                        using (var gRoi = Graphics.FromImage(drawBmp))
                        using (var roiPen = new Pen(Color.Lime, 2))
                        {
                            if (_roiEnabled && _roiDisp.Width >= 4 && _roiDisp.Height >= 4)
                                gRoi.DrawRectangle(roiPen, _roiDisp);
                        }

                        // 3) YOLO detect trong ROI (tọa độ 640×640)
                        var dets = new List<YoloOnnxSafe.Det>();
                        if (_yolo != null && _roiEnabled)
                        {
                            var roi = Rectangle.Intersect(_roiDisp, new Rectangle(0, 0, srcBmp.Width, srcBmp.Height));
                            if (roi.Width >= 8 && roi.Height >= 8)
                            {
                                try
                                {
                                    using (var roiBmp = srcBmp.Clone(roi, PixelFormat.Format24bppRgb))
                                    {
                                        var detsRoi = _yolo.Infer(roiBmp);

                                        // dịch box ROI-local -> ảnh 640 toàn cục
                                        dets = detsRoi.Select(d =>
                                            new YoloOnnxSafe.Det(
                                                new RectangleF(d.Rect.X + roi.Left, d.Rect.Y + roi.Top, d.Rect.Width, d.Rect.Height),
                                                d.Score, d.ClassId, d.Label)
                                        ).ToList();
                                    }

                                    // chỉ giữ box nằm TRỌN ROI
                                    dets = dets.Where(d =>
                                    {
                                        int x1 = (int)Math.Floor(d.Rect.Left);
                                        int y1 = (int)Math.Floor(d.Rect.Top);
                                        int x2 = (int)Math.Ceiling(d.Rect.Right);
                                        int y2 = (int)Math.Ceiling(d.Rect.Bottom);
                                        return _roiDisp.Contains(Rectangle.FromLTRB(x1, y1, x2, y2));
                                    }).ToList();

                                    // lọc kích thước/aspect trên 640×640
                                    const int MIN_W_PX = 120, MIN_H_PX = 120, MIN_AREA_PX = 120 * 120;
                                    const float MIN_ASPECT = 0.5f, MAX_ASPECT = 1.3f;
                                    dets = dets.Where(d =>
                                    {
                                        float w = d.Rect.Width, h = d.Rect.Height;
                                        if (w < MIN_W_PX || h < MIN_H_PX) return false;
                                        if (w * h < MIN_AREA_PX) return false;
                                        float ar = w / h;
                                        return !(ar < MIN_ASPECT || ar > MAX_ASPECT);
                                    }).ToList();
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine("YOLO error: " + ex.Message);
                                    dets.Clear();
                                }
                            }
                        }

                        // 4) Tracking + (GỬI PLC được map ngược về khung gốc bên trong hàm của bạn)
                        //    Lưu ý: hàm UpdateTracksAndSendPlcOnExit của bạn phải dùng _lastResizeMeta để map 640 -> gốc khi Missed==2
                        UpdateTracksAndSendPlcOnExit(dets);

                        // 5) Vẽ theo track còn sống trên 640×640
                        DrawTracks(drawBmp);

                        // 6) Cập nhật UI (giữ nguyên pictureBox 640×640)
                        var old = pictureBox.Image;
                        pictureBox.Image = drawBmp;
                        old?.Dispose();
                    }
                }
            });

            _uiTimer = new System.Windows.Forms.Timer { Interval = Math.Max(1, 1000 / TARGET_FPS) };
            _uiTimer.Tick += (s2, ev2) =>
            {
                // nếu bạn dùng cơ chế _latestBmp thì cập nhật ở đây;
                // hiện tại đã set trực tiếp Image ngay trong capture loop để đơn giản.
            };
            _uiTimer.Start();
        }


        ////////////////////////////////
        private static Bitmap PadToSquare640(Bitmap src)
        {
            const int S = 640;
            float r = Math.Min(S / (float)src.Width, S / (float)src.Height);
            int nw = (int)Math.Round(src.Width * r);
            int nh = (int)Math.Round(src.Height * r);
            int padX = (S - nw) / 2;
            int padY = (S - nh) / 2;

            var canvas = new Bitmap(S, S, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            using (var g = Graphics.FromImage(canvas))
            {
                g.Clear(Color.Black);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
                g.DrawImage(src, new Rectangle(padX, padY, nw, nh));
            }
            return canvas;
        }

        private void EnsurePlcSenderRunning()
        {
            if (_plcTask == null || _plcTask.IsCompleted)
            {
                _plcCts = new CancellationTokenSource();
                _plcTask = Task.Run(() =>
                {
                    var buf = new byte[14]; // 2 DINT (8 byte) + 1 INT (2 byte)
                    while (!_plcCts.IsCancellationRequested)
                    {
                        try
                        {
                            if (_plcQueue.TryDequeue(out var item))
                            {
                                Array.Clear(buf, 0, buf.Length);
                                S7.SetDIntAt(buf, OFF_X_DINT, item.X);
                                S7.SetDIntAt(buf, OFF_Y_DINT, item.Y);
                                S7.SetDIntAt(buf, OFF_Z_DINT, item.Z);
                                S7.SetIntAt(buf, OFF_T_INT, item.Type);

                                // Viết 1 lần cả block để tránh ghi đè “nguyên byte”
                                int rc = plc.DBWrite(DB_NUMBER, 0, buf.Length, buf);
                                if (rc != 0) System.Diagnostics.Debug.WriteLine("DBWrite failed: " + plc.ErrorText(rc));
                            }
                            else Thread.Sleep(2);
                        }
                        catch { Thread.Sleep(10); }
                    }
                }, _plcCts.Token);
            }
        }
        private void UpdateTracksAndSendPlcOnExit(List<YoloOnnxSafe.Det> dets)
        {
            // ===== Cấu hình băng tải =====//ban dau 20
            const double BELT_SPEED_MM_PER_S = 65.0;   // tốc độ băng tải (mm/s)
            const int BELT_DIR_Y = +1;     // +1 nếu chạy theo +Y, -1 nếu ngược lại
            const double OUT_LIFETIME_S = 9.0;   // xóa track sau khi vật ở ngoài ROI >12s
            double Z0_MM = -310;  // Z cố định (mm)

            var matched = new HashSet<int>();
            var nowUtc = DateTime.UtcNow;

            // --- 1) Match detection -> track ---
            foreach (var d in dets)
            {
                var c = new PointF(d.Rect.X + d.Rect.Width / 2f, d.Rect.Y + d.Rect.Height / 2f);
                int bestId = -1;
                float bestDist = float.MaxValue;

                foreach (var kv in _tracks)
                {
                    var t = kv.Value;
                    if (t.ClassId != d.ClassId) continue;
                    float dx = t.Center.X - c.X, dy = t.Center.Y - c.Y;
                    float dist = (float)Math.Sqrt(dx * dx + dy * dy);
                    if (dist < bestDist) { bestDist = dist; bestId = kv.Key; }
                }

                if (bestId >= 0 && bestDist <= MATCH_MAX_DIST_PX)
                {
                    var t = _tracks[bestId];
                    t.Rect = d.Rect;
                    t.Center = c;
                    t.Label = d.Label;
                    t.Missed = 0;
                    t.Missed2AtUtc = null;

                    if (_roiEnabled && _roiDisp.Contains((int)c.X, (int)c.Y))
                        t.LastInRoiCenter = c;

                    _tracks[bestId] = t;
                    matched.Add(bestId);
                }
                else
                {
                    var t = new Track
                    {
                        Id = _nextTrackId++,
                        Rect = d.Rect,
                        Center = c,
                        ClassId = d.ClassId,
                        Label = d.Label,
                        Missed = 0,
                        LastInRoiCenter = c,
                        Missed2AtUtc = null,
                        X0_mm = 0,
                        Y0_mm = 0,
                        XRobotFixed = 0
                    };
                    _tracks[t.Id] = t;
                    matched.Add(t.Id);
                }
            }

            // --- 2) Xử lý vật không match (rời ROI) ---
            var toRemove = new List<int>();

            foreach (var kv in _tracks)
            {
                int id = kv.Key;
                var t = kv.Value;

                if (matched.Contains(id))
                    continue;

                t.Missed++;

                // Khi vật vừa rời ROI (Missed == 2)
                if (t.Missed == 2 && t.Missed2AtUtc == null)
                {
                    t.Missed2AtUtc = nowUtc;

                    // 640x640 → 1920x1080 → mm
                    PointF p1920_exit = MapBackPoint_ResizedToOrig(t.LastInRoiCenter, _lastResizeMeta);
                    var (X0_mm, Y0_mm) = Pixel1920ToMm_OnPlane(p1920_exit.X, p1920_exit.Y);
                    t.X0_mm = X0_mm;
                    t.Y0_mm = Y0_mm;

                    // X cố định sau offset
                    double Xw_off = -(X0_mm - 110);
                    double Yw_off = -(Y0_mm - 250);
                    t.XRobotFixed = Xw_off;
                }

                // Nếu đã có mốc rời ROI → gửi liên tục
                if (t.Missed2AtUtc.HasValue)
                {
                    double dt_s = (nowUtc - t.Missed2AtUtc.Value).TotalSeconds;
                    if (dt_s < 0) dt_s = 0;

                    // Y di chuyển theo vận tốc băng tải
                    double Yw_mm = t.Y0_mm + BELT_DIR_Y * BELT_SPEED_MM_PER_S * dt_s;
                    double Xw_mm = t.X0_mm; // X cố định

                    // Offset
                    double Xw_off = -(Xw_mm - 110);
                    // double Yw_off = -(Yw_mm - 250);
                    double Yw_off = -(Yw_mm - 280-30);

                    // 🔧 Bù X và Y nếu cần

                    if (Xw_off < -50)
                    {
                        Xw_off += -25;
                        Yw_off -= -50;
                        Z0_MM = -300;
                    }
                    if (Xw_off >= -50&& Xw_off <= -45)
                    {
                        Xw_off += -15;
                        Yw_off -= -40;
                        Z0_MM = -303;
                    }
                    if (Xw_off > -45 && Xw_off <= -43)
                    {
                        Xw_off += -10;
                        Yw_off -= -45;
                        Z0_MM = -303;
                    }
                    if (Xw_off > -43 && Xw_off <= -40)
                    {
                        Xw_off += -5;
                        Yw_off -= -30;
                        Z0_MM = -305;
                    }
                    if (Xw_off <= -37 && Xw_off > -40)
                    {
                        Xw_off += 0;
                        Yw_off -= -10;
                        Z0_MM = -306;
                    }
                    if (Xw_off <= -35 && Xw_off > -37)
                    {
                        Xw_off += 0;
                        Yw_off -= -10;
                        Z0_MM = -306;
                    }
                    if (Xw_off > -35)
                    {
                        Xw_off += 10;
                        Yw_off -= -10;
                        Z0_MM = -308;
                    }
                    /*if (Xw_off > -40)
                    {
                        Xw_off += -10;
                        Yw_off -= -30;
                        Z0_MM = -305;
                    }*/

                    // Gửi xuống PLC (X, Y, Z, Type)
                    short type = (short)t.ClassId;
                    int x_mm_dint = (int)Math.Round(Xw_off);
                    int y_mm_dint = (int)Math.Round(Yw_off);
                    int z_mm_dint = (int)Math.Round(Z0_MM); // Z cố định

                    _plcQueue.Enqueue((x_mm_dint, y_mm_dint, z_mm_dint, type));

                    // Xóa track sau OUT_LIFETIME_S
                    if (dt_s > OUT_LIFETIME_S)
                        toRemove.Add(id);
                }

                _tracks[id] = t;
            }

            foreach (var id in toRemove)
                _tracks.Remove(id);

            // --- 3) Gửi các item đến hạn (nếu có) ---
            SendDuePlcIfAny();
        }








        // helper: đặt trong Form2 (nếu chưa có)

        // Drop-in thay cho hàm cũ
        private static string SafeLabel(int id, string[] names)
        {
            return (id >= 0 && id < (names?.Length ?? 0)) ? names[id] : $"cls{id}";
        }

        private void DrawTracks(Bitmap drawBmp)
        {
            if (drawBmp == null || _tracks == null) return;

            using var g = Graphics.FromImage(drawBmp);
            using var pen = new Pen(Color.Lime, 2);
            using var labelFont = new Font("Segoe UI", 9f, FontStyle.Bold);
            using var coordFont = new Font("Segoe UI", 9f);

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            float W = drawBmp.Width;
            float H = drawBmp.Height;

            // Duyệt trực tiếp, không Where(... tt != null ...)
            foreach (var t in _tracks.Values)
            {
                // Nếu bạn dùng struct Track, t không thể null; lọc Missed tại đây
                if (t.Missed != 0) continue;

                // Bảo vệ NaN/Infinity
                float rx = float.IsFinite(t.Rect.X) ? t.Rect.X : 0f;
                float ry = float.IsFinite(t.Rect.Y) ? t.Rect.Y : 0f;
                float rw = float.IsFinite(t.Rect.Width) ? t.Rect.Width : 0f;
                float rh = float.IsFinite(t.Rect.Height) ? t.Rect.Height : 0f;
                if (rw < 0) rw = 0;
                if (rh < 0) rh = 0;

                g.DrawRectangle(pen, rx, ry, rw, rh);

                // Tâm
                float cx = float.IsFinite(t.Center.X) ? t.Center.X : (rx + rw / 2f);
                float cy = float.IsFinite(t.Center.Y) ? t.Center.Y : (ry + rh / 2f);
                using (var dot = new SolidBrush(Color.Red))
                {
                    float dx = float.IsFinite(cx) ? cx - 3f : 0f;
                    float dy = float.IsFinite(cy) ? cy - 3f : 0f;
                    g.FillEllipse(dot, dx, dy, 6f, 6f);
                }

                // Nhãn: ưu tiên t.Label; fallback theo ClassId an toàn
                string labelText = !string.IsNullOrWhiteSpace(t.Label)
                    ? t.Label
                    : SafeLabel(t.ClassId, _labels);

                float lx = rx;
                float ly = Math.Max(0, ry - labelFont.Height);
                g.DrawString(labelText, labelFont, Brushes.Black, lx + 1, ly + 1);
                g.DrawString(labelText, labelFont, Brushes.Yellow, lx, ly);

                // Tọa độ
                string coordText = $"({(int)Math.Round(cx)},{(int)Math.Round(cy)})";
                var s = g.MeasureString(coordText, coordFont);
                float tx = cx + 8f, ty = cy - 8f;

                if (!float.IsFinite(tx)) tx = 0;
                if (!float.IsFinite(ty)) ty = 0;

                if (tx + s.Width > W - 2) tx = cx - 8f - s.Width;
                if (ty < 0) ty = cy + 8f;
                if (ty + s.Height > H - 2) ty = H - s.Height - 2;

                g.DrawString(coordText, coordFont, Brushes.Black, tx + 1, ty + 1);
                g.DrawString(coordText, coordFont, Brushes.Red, tx, ty);
            }
        }



        private void StopEverything()
        {
            try { _uiTimer?.Stop(); _uiTimer?.Dispose(); _uiTimer = null; } catch { }

            try { _cts?.Cancel(); _captureTask?.Wait(200); }
            catch { }
            finally { _captureTask = null; _cts?.Dispose(); _cts = null; }

            try { _cap?.Dispose(); _cap = null; } catch { }

            try { _plcCts?.Cancel(); _plcTask?.Wait(200); }
            catch { }
            finally { _plcTask = null; _plcCts?.Dispose(); _plcCts = null; }

            var old = pictureBox.Image; pictureBox.Image = null; old?.Dispose();

            _tracks.Clear(); _nextTrackId = 1;
        }
        ///10_15
        private static Bitmap PadToSquare640WithMeta(Bitmap raw, out ResizeMeta meta)
        {
            int origW = raw.Width, origH = raw.Height;
            int newW = 640, newH = 640;

            double sx = (double)newW / origW;
            double sy = (double)newH / origH;
            double scale = Math.Min(sx, sy);
            int drawW = (int)Math.Round(origW * scale);
            int drawH = (int)Math.Round(origH * scale);
            int padX = (newW - drawW) / 2;
            int padY = (newH - drawH) / 2;

            var dst = new Bitmap(newW, newH, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            using (var g = Graphics.FromImage(dst))
            {
                g.Clear(Color.Black);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.DrawImage(raw, new Rectangle(padX, padY, drawW, drawH));
            }

            meta = new ResizeMeta { OrigW = origW, OrigH = origH, NewW = newW, NewH = newH, Scale = scale, PadX = padX, PadY = padY };
            return dst;
        }

        // Map NGƯỢC: điểm trong 640x640 (đã letterbox) -> về khung gốc (vd 1920x1080)
        private static PointF MapBackPoint_ResizedToOrig(PointF p, in ResizeMeta m)
        {
            float u = (float)((p.X - m.PadX) / m.Scale);
            float v = (float)((p.Y - m.PadY) / m.Scale);
            return new PointF(u, v);
        }
        private (double Xw_mm, double Yw_mm) Pixel1920ToMm_OnPlane(double u1920, double v1920)
        {
            // 1) undistortPoints -> lấy tia chuẩn hoá (x_n, y_n) trong hệ camera
            using var K = new Mat(3, 3, DepthType.Cv64F, 1);
            K.SetTo(DoubleMatrixToArray(_K_1920));

            using var dist = new Mat(_dist_1920.Length, 1, DepthType.Cv64F, 1);
            dist.SetTo(_dist_1920);

            // Dùng VectorOfPointF để tránh lỗi nhiều kênh
            using var srcPts = new VectorOfPointF(new[] { new System.Drawing.PointF((float)u1920, (float)v1920) });
            using var dstPts = new VectorOfPointF();
            CvInvoke.UndistortPoints(srcPts, dstPts, K, dist, null, null);

            var pNorm = dstPts[0];        // (x_n, y_n)
            double xn = pNorm.X;
            double yn = pNorm.Y;

            // d_c = [x_n, y_n, 1]^T
            double[] dc = new double[] { xn, yn, 1.0 };

            // 2) Chuyển tia & tâm camera sang hệ world
            var RT = Transpose3x3(_Rcw);              // R^T
            var Cw = Negate(MulMatVec(RT, _tcw));     // Cw = -R^T * t  (với cấu hình top-down: (0,0,310))
            var dw = MulMatVec(RT, dc);               // d_w = R^T * d_c

            // 3) Giao tuyến với mặt phẳng Z = Z0_MM (ví dụ Z0=0)
            double lambda = (Z0_MM - Cw[2]) / dw[2];
            double Xw = Cw[0] + lambda * dw[0];
            double Yw = Cw[1] + lambda * dw[1];

            return (Xw, Yw);
        }

        // ===== Helpers nho nhỏ =====
        private static double[] DoubleMatrixToArray(double[,] M)
        {
            int r = M.GetLength(0), c = M.GetLength(1);
            var arr = new double[r * c];
            int k = 0;
            for (int i = 0; i < r; i++)
                for (int j = 0; j < c; j++)
                    arr[k++] = M[i, j];
            return arr;
        }
        private static double[,] Transpose3x3(double[,] A)
        {
            return new double[,] {
        { A[0,0], A[1,0], A[2,0] },
        { A[0,1], A[1,1], A[2,1] },
        { A[0,2], A[1,2], A[2,2] },
    };
        }
        private static double[] MulMatVec(double[,] M, double[] v)
        {
            return new double[] {
        M[0,0]*v[0] + M[0,1]*v[1] + M[0,2]*v[2],
        M[1,0]*v[0] + M[1,1]*v[1] + M[1,2]*v[2],
        M[2,0]*v[0] + M[2,1]*v[1] + M[2,2]*v[2],
    };
        }
        private static double[] Negate(double[] v) => new double[] { -v[0], -v[1], -v[2] };

        private void SendDuePlcIfAny()
        {
            var nowUtc = DateTime.UtcNow;
            for (int i = _delayedPlc.Count - 1; i >= 0; i--)
            {
                if (_delayedPlc[i].DueUtc <= nowUtc)
                {
                    var msg = _delayedPlc[i];
                    _plcQueue.Enqueue((msg.X, msg.Y, msg.Z, msg.Type));
                    _delayedPlc.RemoveAt(i);
                }
            }
        }
        ////////////////////////////////

        private void btn_stopvideo_click(object sender, EventArgs e)
        {
            StopCamera();
            StopEverything();
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

            public float ScoreThresh { get; set; } = 0.30f; // sau sigmoid
            public float NmsThresh { get; set; } = 0.45f;

            public YoloOnnxSafe(string onnxPath, string[] classNames, bool useDirectML = false, int inputW = 640, int inputH = 640)
            {
                var opt = new SessionOptions();
                if (useDirectML) opt.AppendExecutionProvider_DML();
                else opt.AppendExecutionProvider_CPU();

                _sess = new InferenceSession(onnxPath, opt);
                _inpW = inputW; _inpH = inputH;
                _inputName = _sess.InputMetadata.Keys.First();
                _outputName = _sess.OutputMetadata.Keys.First(); // YOLOv8 thường chỉ 1 output
                _names = classNames ?? Array.Empty<string>();
            }

            public List<Det> Infer(Bitmap bgr)
            {
                var (resized, scale, padX, padY) = Letterbox(bgr, _inpW, _inpH);

                // BGR -> RGB, [1,3,H,W], 0..1
                var input = new DenseTensor<float>(new[] { 1, 3, _inpH, _inpW });
                var data = BitmapToBytes24(resized);
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

                var inputs = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor(_inputName, input) };
                using var results = _sess.Run(inputs);
                var t = results.First(o => o.Name == _outputName).AsTensor<float>();

                var dets = ParseDetectionsYolov8(t, scale, padX, padY, bgr.Width, bgr.Height);
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

                var canvas = new Bitmap(dstW, dstH, PixelFormat.Format24bppRgb);
                using var g = Graphics.FromImage(canvas);
                g.Clear(Color.Black);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
                g.DrawImage(src, new Rectangle(padX, padY, nw, nh));
                return (canvas, r, padX, padY);
            }

            private static byte[] BitmapToBytes24(Bitmap bmp)
            {
                var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                var bd = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                int bytes = Math.Abs(bd.Stride) * bd.Height;
                byte[] buffer = new byte[bytes];
                Marshal.Copy(bd.Scan0, buffer, 0, bytes);
                bmp.UnlockBits(bd);
                return buffer;
            }

            private List<Det> ParseDetectionsYolov8(Tensor<float> t, float scale, int padX, int padY, int origW, int origH)
            {
                var list = new List<Det>();

                // Chuẩn hóa shape: hỗ trợ [1,1,C,N], [1,1,N,C], [1,C,N], [1,N,C], [N,C]
                int[] rawDims = t.Dimensions.ToArray();
                int[] dims = rawDims;

                if (dims.Length == 4 && dims[0] == 1 && dims[1] == 1)
                {
                    // [1,1,C,N] hoặc [1,1,N,C] -> [1,C,N] hoặc [1,N,C] (giữ batch=1)
                    dims = new[] { 1, dims[2], dims[3] };
                }
                else if (dims.Length == 2)
                {
                    // [N,C] -> [1,N,C]
                    dims = new[] { 1, dims[0], dims[1] };
                }

                var dimsStr = dims.Select(d => d.ToString()).ToArray();
                System.Diagnostics.Debug.WriteLine("Output dims (norm): " + string.Join("x", dimsStr));

                if (dims.Length != 3)
                {
                    System.Diagnostics.Debug.WriteLine("Unsupported output dims (still): " + string.Join("x", dimsStr));
                    return list;
                }

                int d1 = dims[1]; // có thể là C hoặc N
                int d2 = dims[2]; // phần còn lại

                // Suy luận C (4+numClasses) và N
                bool assumeCxN = (d1 <= d2); // phần lớn trường hợp C < N
                int C = assumeCxN ? d1 : d2;
                int N = assumeCxN ? d2 : d1;

                if (C < 5 || N <= 0)
                {
                    System.Diagnostics.Debug.WriteLine($"Bad (C,N)=({C},{N})");
                    return list;
                }

                int clsStart = 4;
                int numClasses = C - clsStart;
                if (numClasses <= 0)
                {
                    System.Diagnostics.Debug.WriteLine($"numClasses invalid: {numClasses}");
                    return list;
                }

                // Thăm dò để biết có cần sigmoid không (nhiều model xuất logits)
                int probe = Math.Min(N, 200);
                float pMax = float.NegativeInfinity, pMin = float.PositiveInfinity;
                for (int i = 0; i < probe; i++)
                {
                    for (int c = clsStart; c < C; c++)
                    {
                        float v = assumeCxN ? t[0, c, i] : t[0, i, c];
                        if (v > pMax) pMax = v;
                        if (v < pMin) pMin = v;
                    }
                }
                bool needSigmoid = (pMax > 1f || pMin < 0f);

                for (int i = 0; i < N; i++)
                {
                    float cx = assumeCxN ? t[0, 0, i] : t[0, i, 0];
                    float cy = assumeCxN ? t[0, 1, i] : t[0, i, 1];
                    float w = assumeCxN ? t[0, 2, i] : t[0, i, 2];
                    float h = assumeCxN ? t[0, 3, i] : t[0, i, 3];

                    if (w <= 1f || h <= 1f) continue; // lọc nhiễu boxes quá nhỏ

                    int best = -1;
                    float bestScore = 0f;
                    for (int c = clsStart; c < C; c++)
                    {
                        float s = assumeCxN ? t[0, c, i] : t[0, i, c];
                        if (needSigmoid) s = 1f / (1f + MathF.Exp(-s));
                        if (s > bestScore) { bestScore = s; best = c - clsStart; }
                    }
                    if (best < 0 || bestScore < ScoreThresh) continue;

                    var rect = UnletterBox(cx, cy, w, h, scale, padX, padY, origW, origH);
                    if (rect.Width < 1f || rect.Height < 1f) continue;

                    string label = (best >= 0 && best < _names.Length) ? _names[best] : $"cls{best}";
                    list.Add(new Det(rect, bestScore, best, label));
                }

                return list;
            }

            private static RectangleF UnletterBox(float cx, float cy, float w, float h,
                                                  float scale, int padX, int padY, int ow, int oh)
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

            private static List<Det> Nms(List<Det> boxes, float thr)
            {
                var res = new List<Det>();
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

            private static float Clamp(float v, float min, float max) => (v < min) ? min : (v > max ? max : v);
        }



        private void viewplc(object sender, EventArgs e)
        {
            byte[] buf = new byte[12];
            int result = plc.DBRead(111, 0, buf.Length, buf);
            double x_rotate, y_rotate, z_rotate;
            if (result == 0)
            {
                float theta1 = S7.GetRealAt(buf, 0);
                float theta2 = S7.GetRealAt(buf, 4);
                float theta3 = S7.GetRealAt(buf, 8);

                double X_viewplc = 0, Y_viewplc = 0, Z_viewplc = 0;
                int Status1 = delta_calcForward(theta1 / 10, theta2 / 10, theta3 / 10, ref X_viewplc, ref Y_viewplc, ref Z_viewplc);
                RotateZ(X_viewplc *0.9 , Y_viewplc * 0.9 , Z_viewplc, thetaDeg: 330, out x_rotate, out y_rotate, out z_rotate);

                if (Status1 == 0)
                {
                    textBox_viewplc_x.Text = FormatValue(x_rotate);
                    textBox_viewplc_y.Text = FormatValue(y_rotate);
                    textBox_viewplc_z.Text = FormatValue(z_rotate);
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
        public void WriteBool(int db, int byteOffset, int bitOffset, bool value)
        {
            // 1️⃣ Đọc byte hiện tại
            byte[] buf = new byte[1];
            int rc = plc.DBRead(db, byteOffset, buf.Length, buf);
            if (rc != 0)
                throw new Exception(plc.ErrorText(rc));

            // 2️⃣ Sửa đúng bit cần ghi
            S7.SetBitAt( buf, 0, bitOffset, value);

            // 3️⃣ Ghi lại nguyên byte sau khi chỉnh
            rc = plc.DBWrite(db, byteOffset, buf.Length, buf);
            if (rc != 0)
                throw new Exception(plc.ErrorText(rc));
        }
        public bool ReadBool(int db, int byteOffset, int bitOffset)
        {
            byte[] buf = new byte[1];
            int rc = plc.DBRead(db, byteOffset, buf.Length, buf);
            if (rc != 0) throw new Exception(plc.ErrorText(rc));

            return S7.GetBitAt(buf, 0, bitOffset);
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
        static void RotateZ(double x, double y, double z, double thetaDeg,
                             out double xr, out double yr, out double zr)
        {
            double r = Math.PI * thetaDeg / 180.0;
            double c = Math.Cos(r);
            double s = Math.Sin(r);
            xr = c * x - s * y;
            yr = s * x + c * y;
            zr = z; // xoay quanh Z thì Z không đổi
        }   
       
        private async void btn_exc_click(object sender, EventArgs e)
        {
            double x1 = (double)numericUpDown_x.Value;
            double y1 = (double)numericUpDown_y.Value;
            double z1 = (double)numericUpDown_z.Value;
            double vTcp = (double)numericUpDown_vel.Value;
            double x_rotate, y_rotate, z_rotate;
            RotateZ(x1/0.9, y1/0.9 , z1, thetaDeg: 30, out x_rotate, out y_rotate, out z_rotate);
            WriteReal(44, 0, (float)x_rotate);
            WriteReal(44, 4, (float)y_rotate);
            WriteReal(44, 8, (float)z_rotate);
            WriteReal(44, 12, (float)vTcp);
            WriteBool(47, 0, 0, false);
            WriteBool(47, 0, 0, true);


        }
        private void btn_home_click(object sender, EventArgs e)
        {
            if (!plc.Connected)
            {
                MessageBox.Show("PLC chưa kết nối.");
                return;
            }
            WriteBool(48, 0, 0, false);
            WriteBool(48, 0, 0, true);
            WriteBool(33, 0, 0, false);
            WriteBool(33, 0, 0, true);
        }

    }
}
