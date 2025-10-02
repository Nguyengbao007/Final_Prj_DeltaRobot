namespace Project_CK
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2));
            groupBox1 = new GroupBox();
            btn_FWD = new Button();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            textBox_theta3 = new TextBox();
            textBox_theta2 = new TextBox();
            textBox_theta1 = new TextBox();
            groupBox2 = new GroupBox();
            btn_INV = new Button();
            label6 = new Label();
            label5 = new Label();
            label4 = new Label();
            textBox_z = new TextBox();
            textBox_y = new TextBox();
            textBox_x = new TextBox();
            groupBox3 = new GroupBox();
            textBox_view_theta3 = new TextBox();
            textBox_view_theta2 = new TextBox();
            textBox_view_theta1 = new TextBox();
            label12 = new Label();
            label11 = new Label();
            label10 = new Label();
            label9 = new Label();
            label8 = new Label();
            label7 = new Label();
            textBox_view_z = new TextBox();
            textBox_view_y = new TextBox();
            textBox_view_x = new TextBox();
            combox_plc = new ComboBox();
            groupBox4 = new GroupBox();
            btn_disconnect = new Button();
            btn_connect = new Button();
            label_status_connection = new Label();
            label14 = new Label();
            label13 = new Label();
            pictureBox = new PictureBox();
            groupBox5 = new GroupBox();
            btn_stopvideo = new Button();
            btn_startvideo = new Button();
            trackBar_zoom = new TrackBar();
            label17 = new Label();
            trackBar_saturation = new TrackBar();
            label16 = new Label();
            trackBar_dosang = new TrackBar();
            label15 = new Label();
            groupbox = new GroupBox();
            button6 = new Button();
            button5 = new Button();
            button4 = new Button();
            button3 = new Button();
            button2 = new Button();
            button1 = new Button();
            groupBox6 = new GroupBox();
            textBox_ns_z = new TextBox();
            textBox_ns_y = new TextBox();
            textBox_ns_x = new TextBox();
            numericUpDown_vel = new NumericUpDown();
            btn_exc = new Button();
            label21 = new Label();
            numericUpDown_z = new NumericUpDown();
            label20 = new Label();
            numericUpDown_y = new NumericUpDown();
            label19 = new Label();
            label18 = new Label();
            numericUpDown_x = new NumericUpDown();
            btn_home = new Button();
            groupBox8 = new GroupBox();
            label26 = new Label();
            textBox_viewplc_z = new TextBox();
            label25 = new Label();
            textBox_viewplc_y = new TextBox();
            label24 = new Label();
            textBox_viewplc_x = new TextBox();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox).BeginInit();
            groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar_zoom).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBar_saturation).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBar_dosang).BeginInit();
            groupbox.SuspendLayout();
            groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_vel).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_z).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_y).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_x).BeginInit();
            groupBox8.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(btn_FWD);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(textBox_theta3);
            groupBox1.Controls.Add(textBox_theta2);
            groupBox1.Controls.Add(textBox_theta1);
            groupBox1.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            groupBox1.Location = new Point(3, 494);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(223, 148);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Forward Kinematics";
            // 
            // btn_FWD
            // 
            btn_FWD.Image = (Image)resources.GetObject("btn_FWD.Image");
            btn_FWD.Location = new Point(162, 67);
            btn_FWD.Name = "btn_FWD";
            btn_FWD.Size = new Size(55, 23);
            btn_FWD.TabIndex = 6;
            btn_FWD.UseVisualStyleBackColor = true;
            btn_FWD.Click += btn_FWD_click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            label3.Location = new Point(3, 99);
            label3.Name = "label3";
            label3.Size = new Size(58, 17);
            label3.TabIndex = 5;
            label3.Text = "Theta3 :";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            label2.Location = new Point(3, 70);
            label2.Name = "label2";
            label2.Size = new Size(58, 17);
            label2.TabIndex = 4;
            label2.Text = "Theta2 :";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(3, 41);
            label1.Name = "label1";
            label1.Size = new Size(58, 17);
            label1.TabIndex = 3;
            label1.Text = "Theta1 :";
            // 
            // textBox_theta3
            // 
            textBox_theta3.Location = new Point(67, 93);
            textBox_theta3.Name = "textBox_theta3";
            textBox_theta3.Size = new Size(89, 25);
            textBox_theta3.TabIndex = 2;
            // 
            // textBox_theta2
            // 
            textBox_theta2.Location = new Point(67, 64);
            textBox_theta2.Name = "textBox_theta2";
            textBox_theta2.Size = new Size(89, 25);
            textBox_theta2.TabIndex = 1;
            // 
            // textBox_theta1
            // 
            textBox_theta1.Location = new Point(67, 35);
            textBox_theta1.Name = "textBox_theta1";
            textBox_theta1.Size = new Size(89, 25);
            textBox_theta1.TabIndex = 0;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(btn_INV);
            groupBox2.Controls.Add(label6);
            groupBox2.Controls.Add(label5);
            groupBox2.Controls.Add(label4);
            groupBox2.Controls.Add(textBox_z);
            groupBox2.Controls.Add(textBox_y);
            groupBox2.Controls.Add(textBox_x);
            groupBox2.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            groupBox2.Location = new Point(589, 494);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(194, 148);
            groupBox2.TabIndex = 3;
            groupBox2.TabStop = false;
            groupBox2.Text = "Inverse Kinematics";
            // 
            // btn_INV
            // 
            btn_INV.Image = (Image)resources.GetObject("btn_INV.Image");
            btn_INV.Location = new Point(6, 66);
            btn_INV.Name = "btn_INV";
            btn_INV.Size = new Size(55, 23);
            btn_INV.TabIndex = 7;
            btn_INV.UseVisualStyleBackColor = true;
            btn_INV.Click += btn_INV_click;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label6.Location = new Point(163, 99);
            label6.Name = "label6";
            label6.Size = new Size(24, 17);
            label6.TabIndex = 6;
            label6.Text = ": Z";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.Location = new Point(163, 70);
            label5.Name = "label5";
            label5.Size = new Size(24, 17);
            label5.TabIndex = 5;
            label5.Text = ": Y";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.Location = new Point(162, 41);
            label4.Name = "label4";
            label4.Size = new Size(25, 17);
            label4.TabIndex = 4;
            label4.Text = ": X";
            label4.Click += label4_Click;
            // 
            // textBox_z
            // 
            textBox_z.Location = new Point(67, 93);
            textBox_z.Name = "textBox_z";
            textBox_z.Size = new Size(89, 25);
            textBox_z.TabIndex = 2;
            // 
            // textBox_y
            // 
            textBox_y.Location = new Point(67, 64);
            textBox_y.Name = "textBox_y";
            textBox_y.Size = new Size(89, 25);
            textBox_y.TabIndex = 1;
            // 
            // textBox_x
            // 
            textBox_x.Location = new Point(67, 35);
            textBox_x.Name = "textBox_x";
            textBox_x.Size = new Size(89, 25);
            textBox_x.TabIndex = 0;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(textBox_view_theta3);
            groupBox3.Controls.Add(textBox_view_theta2);
            groupBox3.Controls.Add(textBox_view_theta1);
            groupBox3.Controls.Add(label12);
            groupBox3.Controls.Add(label11);
            groupBox3.Controls.Add(label10);
            groupBox3.Controls.Add(label9);
            groupBox3.Controls.Add(label8);
            groupBox3.Controls.Add(label7);
            groupBox3.Controls.Add(textBox_view_z);
            groupBox3.Controls.Add(textBox_view_y);
            groupBox3.Controls.Add(textBox_view_x);
            groupBox3.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            groupBox3.Location = new Point(232, 494);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(351, 148);
            groupBox3.TabIndex = 4;
            groupBox3.TabStop = false;
            groupBox3.Text = "Update";
            // 
            // textBox_view_theta3
            // 
            textBox_view_theta3.Location = new Point(230, 92);
            textBox_view_theta3.Name = "textBox_view_theta3";
            textBox_view_theta3.Size = new Size(100, 25);
            textBox_view_theta3.TabIndex = 12;
            // 
            // textBox_view_theta2
            // 
            textBox_view_theta2.Location = new Point(230, 61);
            textBox_view_theta2.Name = "textBox_view_theta2";
            textBox_view_theta2.Size = new Size(100, 25);
            textBox_view_theta2.TabIndex = 11;
            // 
            // textBox_view_theta1
            // 
            textBox_view_theta1.Location = new Point(230, 30);
            textBox_view_theta1.Name = "textBox_view_theta1";
            textBox_view_theta1.Size = new Size(100, 25);
            textBox_view_theta1.TabIndex = 10;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label12.Location = new Point(16, 100);
            label12.Name = "label12";
            label12.Size = new Size(24, 17);
            label12.TabIndex = 9;
            label12.Text = "Z :";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label11.Location = new Point(17, 69);
            label11.Name = "label11";
            label11.Size = new Size(24, 17);
            label11.TabIndex = 8;
            label11.Text = "Y :";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label10.Location = new Point(166, 100);
            label10.Name = "label10";
            label10.Size = new Size(58, 17);
            label10.TabIndex = 7;
            label10.Text = "Theta3 :";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label9.Location = new Point(166, 69);
            label9.Name = "label9";
            label9.Size = new Size(58, 17);
            label9.TabIndex = 6;
            label9.Text = "Theta2 :";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label8.Location = new Point(166, 38);
            label8.Name = "label8";
            label8.Size = new Size(58, 17);
            label8.TabIndex = 5;
            label8.Text = "Theta1 :";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label7.Location = new Point(16, 38);
            label7.Name = "label7";
            label7.Size = new Size(25, 17);
            label7.TabIndex = 4;
            label7.Text = "X :";
            // 
            // textBox_view_z
            // 
            textBox_view_z.Location = new Point(46, 92);
            textBox_view_z.Name = "textBox_view_z";
            textBox_view_z.Size = new Size(100, 25);
            textBox_view_z.TabIndex = 2;
            // 
            // textBox_view_y
            // 
            textBox_view_y.Location = new Point(46, 61);
            textBox_view_y.Name = "textBox_view_y";
            textBox_view_y.Size = new Size(100, 25);
            textBox_view_y.TabIndex = 1;
            // 
            // textBox_view_x
            // 
            textBox_view_x.Location = new Point(47, 30);
            textBox_view_x.Name = "textBox_view_x";
            textBox_view_x.Size = new Size(100, 25);
            textBox_view_x.TabIndex = 0;
            // 
            // combox_plc
            // 
            combox_plc.FormattingEnabled = true;
            combox_plc.Location = new Point(77, 27);
            combox_plc.Name = "combox_plc";
            combox_plc.Size = new Size(146, 25);
            combox_plc.TabIndex = 5;
            combox_plc.Tag = "";
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(btn_disconnect);
            groupBox4.Controls.Add(btn_connect);
            groupBox4.Controls.Add(label_status_connection);
            groupBox4.Controls.Add(label14);
            groupBox4.Controls.Add(label13);
            groupBox4.Controls.Add(combox_plc);
            groupBox4.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            groupBox4.Location = new Point(12, 12);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(340, 100);
            groupBox4.TabIndex = 6;
            groupBox4.TabStop = false;
            groupBox4.Text = "PLC Connection";
            // 
            // btn_disconnect
            // 
            btn_disconnect.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btn_disconnect.Location = new Point(234, 58);
            btn_disconnect.Name = "btn_disconnect";
            btn_disconnect.Size = new Size(85, 32);
            btn_disconnect.TabIndex = 10;
            btn_disconnect.Text = "Disconnect";
            btn_disconnect.UseVisualStyleBackColor = true;
            btn_disconnect.Click += btn_disconnect_Click;
            // 
            // btn_connect
            // 
            btn_connect.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btn_connect.Location = new Point(234, 27);
            btn_connect.Name = "btn_connect";
            btn_connect.Size = new Size(85, 28);
            btn_connect.TabIndex = 9;
            btn_connect.Text = "Connect";
            btn_connect.UseVisualStyleBackColor = true;
            btn_connect.Click += btn_connect_Click;
            // 
            // label_status_connection
            // 
            label_status_connection.AutoSize = true;
            label_status_connection.Location = new Point(139, 70);
            label_status_connection.Name = "label_status_connection";
            label_status_connection.Size = new Size(76, 17);
            label_status_connection.TabIndex = 8;
            label_status_connection.Text = "Disconnect";
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new Point(6, 70);
            label14.Name = "label14";
            label14.Size = new Size(127, 17);
            label14.TabIndex = 7;
            label14.Text = "Connection status :";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(6, 35);
            label13.Name = "label13";
            label13.Size = new Size(65, 17);
            label13.TabIndex = 6;
            label13.Text = "Address :";
            // 
            // pictureBox
            // 
            pictureBox.Location = new Point(789, 8);
            pictureBox.Name = "pictureBox";
            pictureBox.Size = new Size(640, 480);
            pictureBox.TabIndex = 7;
            pictureBox.TabStop = false;
            // 
            // groupBox5
            // 
            groupBox5.Controls.Add(btn_stopvideo);
            groupBox5.Controls.Add(btn_startvideo);
            groupBox5.Controls.Add(trackBar_zoom);
            groupBox5.Controls.Add(label17);
            groupBox5.Controls.Add(trackBar_saturation);
            groupBox5.Controls.Add(label16);
            groupBox5.Controls.Add(trackBar_dosang);
            groupBox5.Controls.Add(label15);
            groupBox5.Location = new Point(789, 494);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new Size(640, 148);
            groupBox5.TabIndex = 9;
            groupBox5.TabStop = false;
            groupBox5.Text = "Setting";
            // 
            // btn_stopvideo
            // 
            btn_stopvideo.Location = new Point(402, 22);
            btn_stopvideo.Name = "btn_stopvideo";
            btn_stopvideo.Size = new Size(75, 23);
            btn_stopvideo.TabIndex = 8;
            btn_stopvideo.Text = "Stop";
            btn_stopvideo.UseVisualStyleBackColor = true;
            btn_stopvideo.Click += btn_stopvideo_click;
            // 
            // btn_startvideo
            // 
            btn_startvideo.Location = new Point(310, 22);
            btn_startvideo.Name = "btn_startvideo";
            btn_startvideo.Size = new Size(75, 23);
            btn_startvideo.TabIndex = 7;
            btn_startvideo.Text = "Start";
            btn_startvideo.UseVisualStyleBackColor = true;
            btn_startvideo.Click += btn_startvideo_Click;
            // 
            // trackBar_zoom
            // 
            trackBar_zoom.Location = new Point(83, 103);
            trackBar_zoom.Name = "trackBar_zoom";
            trackBar_zoom.Size = new Size(201, 45);
            trackBar_zoom.TabIndex = 6;
            trackBar_zoom.TickFrequency = 100;
            trackBar_zoom.TickStyle = TickStyle.Both;
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label17.Location = new Point(6, 113);
            label17.Name = "label17";
            label17.Size = new Size(45, 15);
            label17.TabIndex = 5;
            label17.Text = "Zoom :";
            // 
            // trackBar_saturation
            // 
            trackBar_saturation.Location = new Point(83, 61);
            trackBar_saturation.Name = "trackBar_saturation";
            trackBar_saturation.Size = new Size(201, 45);
            trackBar_saturation.TabIndex = 4;
            trackBar_saturation.TickFrequency = 100;
            trackBar_saturation.TickStyle = TickStyle.Both;
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label16.Location = new Point(6, 71);
            label16.Name = "label16";
            label16.Size = new Size(71, 15);
            label16.TabIndex = 3;
            label16.Text = "Saturation :";
            // 
            // trackBar_dosang
            // 
            trackBar_dosang.Location = new Point(83, 15);
            trackBar_dosang.Name = "trackBar_dosang";
            trackBar_dosang.Size = new Size(201, 45);
            trackBar_dosang.TabIndex = 2;
            trackBar_dosang.TickFrequency = 100;
            trackBar_dosang.TickStyle = TickStyle.Both;
            trackBar_dosang.Scroll += trackBar1_Scroll;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label15.Location = new Point(6, 35);
            label15.Name = "label15";
            label15.Size = new Size(59, 15);
            label15.TabIndex = 1;
            label15.Text = "Độ Sáng :";
            // 
            // groupbox
            // 
            groupbox.Controls.Add(button6);
            groupbox.Controls.Add(button5);
            groupbox.Controls.Add(button4);
            groupbox.Controls.Add(button3);
            groupbox.Controls.Add(button2);
            groupbox.Controls.Add(button1);
            groupbox.Location = new Point(537, 21);
            groupbox.Name = "groupbox";
            groupbox.Size = new Size(208, 124);
            groupbox.TabIndex = 10;
            groupbox.TabStop = false;
            groupbox.Text = "Move Jog";
            // 
            // button6
            // 
            button6.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button6.Location = new Point(118, 80);
            button6.Name = "button6";
            button6.Size = new Size(75, 23);
            button6.TabIndex = 5;
            button6.Text = "-Z";
            button6.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            button5.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button5.Location = new Point(118, 51);
            button5.Name = "button5";
            button5.Size = new Size(75, 23);
            button5.TabIndex = 4;
            button5.Text = "-Y";
            button5.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            button4.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button4.Location = new Point(118, 22);
            button4.Name = "button4";
            button4.Size = new Size(75, 23);
            button4.TabIndex = 3;
            button4.Text = "-X";
            button4.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button3.Location = new Point(6, 80);
            button3.Name = "button3";
            button3.Size = new Size(75, 23);
            button3.TabIndex = 2;
            button3.Text = "+Z";
            button3.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button2.Location = new Point(6, 51);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 1;
            button2.Text = "+Y";
            button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button1.Location = new Point(6, 22);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 0;
            button1.Text = "+X";
            button1.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            groupBox6.Controls.Add(textBox_ns_z);
            groupBox6.Controls.Add(textBox_ns_y);
            groupBox6.Controls.Add(textBox_ns_x);
            groupBox6.Controls.Add(numericUpDown_vel);
            groupBox6.Controls.Add(btn_exc);
            groupBox6.Controls.Add(label21);
            groupBox6.Controls.Add(numericUpDown_z);
            groupBox6.Controls.Add(label20);
            groupBox6.Controls.Add(numericUpDown_y);
            groupBox6.Controls.Add(label19);
            groupBox6.Controls.Add(label18);
            groupBox6.Controls.Add(numericUpDown_x);
            groupBox6.Controls.Add(btn_home);
            groupBox6.Location = new Point(3, 193);
            groupBox6.Name = "groupBox6";
            groupBox6.Size = new Size(403, 183);
            groupBox6.TabIndex = 11;
            groupBox6.TabStop = false;
            groupBox6.Text = "Control Manual";
            // 
            // textBox_ns_z
            // 
            textBox_ns_z.Location = new Point(273, 108);
            textBox_ns_z.Name = "textBox_ns_z";
            textBox_ns_z.Size = new Size(100, 23);
            textBox_ns_z.TabIndex = 23;
            // 
            // textBox_ns_y
            // 
            textBox_ns_y.Location = new Point(273, 73);
            textBox_ns_y.Name = "textBox_ns_y";
            textBox_ns_y.Size = new Size(100, 23);
            textBox_ns_y.TabIndex = 22;
            // 
            // textBox_ns_x
            // 
            textBox_ns_x.Location = new Point(273, 42);
            textBox_ns_x.Name = "textBox_ns_x";
            textBox_ns_x.Size = new Size(100, 23);
            textBox_ns_x.TabIndex = 21;
            // 
            // numericUpDown_vel
            // 
            numericUpDown_vel.Location = new Point(67, 145);
            numericUpDown_vel.Name = "numericUpDown_vel";
            numericUpDown_vel.Size = new Size(94, 23);
            numericUpDown_vel.TabIndex = 20;
            // 
            // btn_exc
            // 
            btn_exc.Location = new Point(96, 22);
            btn_exc.Name = "btn_exc";
            btn_exc.Size = new Size(75, 23);
            btn_exc.TabIndex = 19;
            btn_exc.Text = "EXECUTE";
            btn_exc.UseVisualStyleBackColor = true;
            btn_exc.Click += btn_exc_click;
            // 
            // label21
            // 
            label21.AutoSize = true;
            label21.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label21.Location = new Point(6, 108);
            label21.Name = "label21";
            label21.Size = new Size(54, 17);
            label21.TabIndex = 18;
            label21.Text = "Axis Z :";
            // 
            // numericUpDown_z
            // 
            numericUpDown_z.DecimalPlaces = 2;
            numericUpDown_z.Location = new Point(77, 108);
            numericUpDown_z.Name = "numericUpDown_z";
            numericUpDown_z.Size = new Size(90, 23);
            numericUpDown_z.TabIndex = 17;
            // 
            // label20
            // 
            label20.AutoSize = true;
            label20.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label20.Location = new Point(6, 79);
            label20.Name = "label20";
            label20.Size = new Size(54, 17);
            label20.TabIndex = 16;
            label20.Text = "Axis Y :";
            // 
            // numericUpDown_y
            // 
            numericUpDown_y.DecimalPlaces = 2;
            numericUpDown_y.Location = new Point(77, 79);
            numericUpDown_y.Name = "numericUpDown_y";
            numericUpDown_y.Size = new Size(90, 23);
            numericUpDown_y.TabIndex = 15;
            // 
            // label19
            // 
            label19.AutoSize = true;
            label19.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label19.Location = new Point(6, 50);
            label19.Name = "label19";
            label19.Size = new Size(55, 17);
            label19.TabIndex = 14;
            label19.Text = "Axis X :";
            // 
            // label18
            // 
            label18.AutoSize = true;
            label18.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label18.Location = new Point(0, 151);
            label18.Name = "label18";
            label18.Size = new Size(65, 17);
            label18.TabIndex = 13;
            label18.Text = "Velocity :";
            // 
            // numericUpDown_x
            // 
            numericUpDown_x.DecimalPlaces = 2;
            numericUpDown_x.Location = new Point(77, 50);
            numericUpDown_x.Name = "numericUpDown_x";
            numericUpDown_x.Size = new Size(90, 23);
            numericUpDown_x.TabIndex = 12;
            // 
            // btn_home
            // 
            btn_home.Location = new Point(15, 22);
            btn_home.Name = "btn_home";
            btn_home.Size = new Size(75, 23);
            btn_home.TabIndex = 0;
            btn_home.Text = "HOME";
            btn_home.UseVisualStyleBackColor = true;
            btn_home.Click += btn_home_click;
            // 
            // groupBox8
            // 
            groupBox8.Controls.Add(label26);
            groupBox8.Controls.Add(textBox_viewplc_z);
            groupBox8.Controls.Add(label25);
            groupBox8.Controls.Add(textBox_viewplc_y);
            groupBox8.Controls.Add(label24);
            groupBox8.Controls.Add(textBox_viewplc_x);
            groupBox8.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            groupBox8.Location = new Point(3, 118);
            groupBox8.Name = "groupBox8";
            groupBox8.Size = new Size(395, 69);
            groupBox8.TabIndex = 13;
            groupBox8.TabStop = false;
            groupBox8.Text = "View";
            // 
            // label26
            // 
            label26.AutoSize = true;
            label26.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label26.Location = new Point(266, 25);
            label26.Name = "label26";
            label26.Size = new Size(24, 17);
            label26.TabIndex = 16;
            label26.Text = "Z :";
            // 
            // textBox_viewplc_z
            // 
            textBox_viewplc_z.Location = new Point(289, 21);
            textBox_viewplc_z.Name = "textBox_viewplc_z";
            textBox_viewplc_z.Size = new Size(100, 25);
            textBox_viewplc_z.TabIndex = 15;
            // 
            // label25
            // 
            label25.AutoSize = true;
            label25.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label25.Location = new Point(137, 24);
            label25.Name = "label25";
            label25.Size = new Size(24, 17);
            label25.TabIndex = 14;
            label25.Text = "Y :";
            // 
            // textBox_viewplc_y
            // 
            textBox_viewplc_y.Location = new Point(159, 22);
            textBox_viewplc_y.Name = "textBox_viewplc_y";
            textBox_viewplc_y.Size = new Size(100, 25);
            textBox_viewplc_y.TabIndex = 13;
            // 
            // label24
            // 
            label24.AutoSize = true;
            label24.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label24.Location = new Point(5, 22);
            label24.Name = "label24";
            label24.Size = new Size(25, 17);
            label24.TabIndex = 12;
            label24.Text = "X :";
            // 
            // textBox_viewplc_x
            // 
            textBox_viewplc_x.Location = new Point(28, 22);
            textBox_viewplc_x.Name = "textBox_viewplc_x";
            textBox_viewplc_x.Size = new Size(100, 25);
            textBox_viewplc_x.TabIndex = 11;
            // 
            // Form2
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1426, 654);
            Controls.Add(groupBox8);
            Controls.Add(groupBox6);
            Controls.Add(groupbox);
            Controls.Add(groupBox5);
            Controls.Add(pictureBox);
            Controls.Add(groupBox4);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Name = "Form2";
            Text = "Form2";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox).EndInit();
            groupBox5.ResumeLayout(false);
            groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar_zoom).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBar_saturation).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBar_dosang).EndInit();
            groupbox.ResumeLayout(false);
            groupBox6.ResumeLayout(false);
            groupBox6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_vel).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_z).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_y).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_x).EndInit();
            groupBox8.ResumeLayout(false);
            groupBox8.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private Label label3;
        private Label label2;
        private Label label1;
        private TextBox textBox_theta3;
        private TextBox textBox_theta2;
        private TextBox textBox_theta1;
        private GroupBox groupBox2;
        private Label label6;
        private Label label5;
        private Label label4;
        private TextBox textBox_z;
        private TextBox textBox_y;
        private TextBox textBox_x;
        private GroupBox groupBox3;
        private TextBox textBox_view_theta3;
        private TextBox textBox_view_theta2;
        private TextBox textBox_view_theta1;
        private Label label12;
        private Label label11;
        private Label label10;
        private Label label9;
        private Label label8;
        private Label label7;
        private TextBox textBox_view_z;
        private TextBox textBox_view_y;
        private TextBox textBox_view_x;
        private Button btn_FWD;
        private Button btn_INV;
        private ComboBox combox_plc;
        private GroupBox groupBox4;
        private Label label13;
        private Button btn_disconnect;
        private Button btn_connect;
        private Label label_status_connection;
        private Label label14;
        private PictureBox pictureBox;
        private GroupBox groupBox5;
        private Label label15;
        private TrackBar trackBar_dosang;
        private TrackBar trackBar_zoom;
        private Label label17;
        private TrackBar trackBar_saturation;
        private Label label16;
        private Button btn_stopvideo;
        private Button btn_startvideo;
        private GroupBox groupbox;
        private Button button6;
        private Button button5;
        private Button button4;
        private Button button3;
        private Button button2;
        private Button button1;
        private GroupBox groupBox6;
        private Button btn_home;
        private GroupBox groupBox8;
        private Label label26;
        private TextBox textBox_viewplc_z;
        private Label label25;
        private TextBox textBox_viewplc_y;
        private Label label24;
        private TextBox textBox_viewplc_x;
        private NumericUpDown numericUpDown_x;
        private Label label21;
        private NumericUpDown numericUpDown_z;
        private Label label20;
        private NumericUpDown numericUpDown_y;
        private Label label19;
        private Label label18;
        private Button btn_exc;
        private NumericUpDown numericUpDown_vel;
        private TextBox textBox_ns_z;
        private TextBox textBox_ns_y;
        private TextBox textBox_ns_x;
    }
}