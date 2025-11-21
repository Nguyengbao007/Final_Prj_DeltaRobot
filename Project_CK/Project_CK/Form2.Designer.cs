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
            combox_plc = new ComboBox();
            groupBox4 = new GroupBox();
            btn_disconnect = new Button();
            btn_connect = new Button();
            label_status_connection = new Label();
            label14 = new Label();
            label13 = new Label();
            groupBox6 = new GroupBox();
            groupBox11 = new GroupBox();
            button_y_sub = new Button();
            button_y_add = new Button();
            button_z_sub = new Button();
            button_z_add = new Button();
            button_x_sub = new Button();
            button_x_add = new Button();
            btn_home = new Button();
            button_manual = new Button();
            btn_exc = new Button();
            label21 = new Label();
            groupBox8 = new GroupBox();
            label26 = new Label();
            textBox_viewplc_z = new TextBox();
            label25 = new Label();
            textBox_viewplc_y = new TextBox();
            label24 = new Label();
            textBox_viewplc_x = new TextBox();
            numericUpDown_z = new NumericUpDown();
            label20 = new Label();
            numericUpDown_y = new NumericUpDown();
            label19 = new Label();
            numericUpDown_x = new NumericUpDown();
            textBox_theta1 = new TextBox();
            textBox_theta2 = new TextBox();
            textBox_theta3 = new TextBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            btn_FWD = new Button();
            groupBox1 = new GroupBox();
            textBox_x = new TextBox();
            textBox_y = new TextBox();
            textBox_z = new TextBox();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            btn_INV = new Button();
            groupBox2 = new GroupBox();
            textBox_view_x = new TextBox();
            textBox_view_y = new TextBox();
            textBox_view_z = new TextBox();
            label7 = new Label();
            label8 = new Label();
            label9 = new Label();
            label10 = new Label();
            label11 = new Label();
            label12 = new Label();
            textBox_view_theta1 = new TextBox();
            textBox_view_theta2 = new TextBox();
            textBox_view_theta3 = new TextBox();
            groupBox3 = new GroupBox();
            pictureBox = new PictureBox();
            groupBox5 = new GroupBox();
            btn_stopvideo = new Button();
            btn_startvideo = new Button();
            groupBox7 = new GroupBox();
            button_home_1 = new Button();
            button_stop = new Button();
            button_start = new Button();
            btn_auto = new Button();
            groupBox9 = new GroupBox();
            textBox_vat_3 = new TextBox();
            textBox_vat_2 = new TextBox();
            textBox_vat_1 = new TextBox();
            label17 = new Label();
            label16 = new Label();
            label15 = new Label();
            groupBox10 = new GroupBox();
            groupBox4.SuspendLayout();
            groupBox6.SuspendLayout();
            groupBox11.SuspendLayout();
            groupBox8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_z).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_y).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_x).BeginInit();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox).BeginInit();
            groupBox5.SuspendLayout();
            groupBox7.SuspendLayout();
            groupBox9.SuspendLayout();
            groupBox10.SuspendLayout();
            SuspendLayout();
            // 
            // combox_plc
            // 
            combox_plc.FormattingEnabled = true;
            combox_plc.Location = new Point(77, 27);
            combox_plc.Name = "combox_plc";
            combox_plc.Size = new Size(169, 25);
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
            groupBox4.ForeColor = SystemColors.ButtonHighlight;
            groupBox4.Location = new Point(3, 12);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(372, 100);
            groupBox4.TabIndex = 6;
            groupBox4.TabStop = false;
            groupBox4.Text = "PLC Connection";
            // 
            // btn_disconnect
            // 
            btn_disconnect.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btn_disconnect.Location = new Point(263, 58);
            btn_disconnect.Name = "btn_disconnect";
            btn_disconnect.Size = new Size(85, 32);
            btn_disconnect.TabIndex = 10;
            btn_disconnect.Text = "Disconnect";
            btn_disconnect.UseVisualStyleBackColor = false;
            btn_disconnect.Click += btn_disconnect_Click;
            // 
            // btn_connect
            // 
            btn_connect.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btn_connect.Location = new Point(263, 24);
            btn_connect.Name = "btn_connect";
            btn_connect.Size = new Size(85, 32);
            btn_connect.TabIndex = 9;
            btn_connect.Text = "Connect";
            btn_connect.UseVisualStyleBackColor = false;
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
            // groupBox6
            // 
            groupBox6.Controls.Add(groupBox11);
            groupBox6.Controls.Add(btn_home);
            groupBox6.Controls.Add(button_manual);
            groupBox6.Controls.Add(btn_exc);
            groupBox6.Controls.Add(label21);
            groupBox6.Controls.Add(groupBox8);
            groupBox6.Controls.Add(numericUpDown_z);
            groupBox6.Controls.Add(label20);
            groupBox6.Controls.Add(numericUpDown_y);
            groupBox6.Controls.Add(label19);
            groupBox6.Controls.Add(numericUpDown_x);
            groupBox6.Font = new Font("Tahoma", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            groupBox6.ForeColor = SystemColors.ButtonHighlight;
            groupBox6.Location = new Point(8, 121);
            groupBox6.Name = "groupBox6";
            groupBox6.Size = new Size(590, 288);
            groupBox6.TabIndex = 11;
            groupBox6.TabStop = false;
            groupBox6.Text = "MANUAL";
            // 
            // groupBox11
            // 
            groupBox11.Controls.Add(button_y_sub);
            groupBox11.Controls.Add(button_y_add);
            groupBox11.Controls.Add(button_z_sub);
            groupBox11.Controls.Add(button_z_add);
            groupBox11.Controls.Add(button_x_sub);
            groupBox11.Controls.Add(button_x_add);
            groupBox11.ForeColor = SystemColors.ButtonHighlight;
            groupBox11.Location = new Point(300, 88);
            groupBox11.Name = "groupBox11";
            groupBox11.Size = new Size(284, 194);
            groupBox11.TabIndex = 26;
            groupBox11.TabStop = false;
            groupBox11.Text = "JOG";
            // 
            // button_y_sub
            // 
            button_y_sub.Font = new Font("Microsoft Sans Serif", 15.75F, FontStyle.Bold);
            button_y_sub.Location = new Point(64, 128);
            button_y_sub.Name = "button_y_sub";
            button_y_sub.Size = new Size(50, 40);
            button_y_sub.TabIndex = 25;
            button_y_sub.Text = "⬇";
            button_y_sub.UseVisualStyleBackColor = false;
            button_y_sub.Click += btn_y_sub_click;
            // 
            // button_y_add
            // 
            button_y_add.Font = new Font("Microsoft Sans Serif", 15.75F, FontStyle.Bold);
            button_y_add.Location = new Point(64, 24);
            button_y_add.Name = "button_y_add";
            button_y_add.Size = new Size(50, 40);
            button_y_add.TabIndex = 24;
            button_y_add.Text = "⬆";
            button_y_add.UseVisualStyleBackColor = false;
            button_y_add.Click += btn_y_add_click;
            // 
            // button_z_sub
            // 
            button_z_sub.Font = new Font("Microsoft Sans Serif", 15.75F, FontStyle.Bold);
            button_z_sub.Location = new Point(194, 117);
            button_z_sub.Name = "button_z_sub";
            button_z_sub.Size = new Size(50, 40);
            button_z_sub.TabIndex = 23;
            button_z_sub.Text = "⬋";
            button_z_sub.UseVisualStyleBackColor = false;
            button_z_sub.Click += btn_z_sub_click;
            // 
            // button_z_add
            // 
            button_z_add.Font = new Font("Microsoft Sans Serif", 15.75F, FontStyle.Bold);
            button_z_add.Location = new Point(194, 33);
            button_z_add.Name = "button_z_add";
            button_z_add.Size = new Size(50, 40);
            button_z_add.TabIndex = 22;
            button_z_add.Text = "⬈";
            button_z_add.UseVisualStyleBackColor = false;
            button_z_add.Click += btn_z_add_click;
            // 
            // button_x_sub
            // 
            button_x_sub.Font = new Font("Microsoft Sans Serif", 15.75F, FontStyle.Bold);
            button_x_sub.Location = new Point(118, 75);
            button_x_sub.Name = "button_x_sub";
            button_x_sub.Size = new Size(50, 40);
            button_x_sub.TabIndex = 21;
            button_x_sub.Text = "⮕";
            button_x_sub.UseVisualStyleBackColor = false;
            button_x_sub.Click += btn_x_add_click;
            // 
            // button_x_add
            // 
            button_x_add.Font = new Font("Microsoft Sans Serif", 15.75F, FontStyle.Bold);
            button_x_add.Location = new Point(6, 75);
            button_x_add.Name = "button_x_add";
            button_x_add.Size = new Size(50, 40);
            button_x_add.TabIndex = 20;
            button_x_add.Text = "⬅";
            button_x_add.UseVisualStyleBackColor = false;
            button_x_add.Click += btn_x_sub_click;
            // 
            // btn_home
            // 
            btn_home.Location = new Point(199, 160);
            btn_home.Name = "btn_home";
            btn_home.Size = new Size(85, 36);
            btn_home.TabIndex = 0;
            btn_home.Text = "HOME";
            btn_home.UseVisualStyleBackColor = false;
            btn_home.Click += btn_home_click;
            // 
            // button_manual
            // 
            button_manual.Location = new Point(199, 90);
            button_manual.Name = "button_manual";
            button_manual.Size = new Size(85, 36);
            button_manual.TabIndex = 15;
            button_manual.Text = "MANUAL";
            button_manual.UseVisualStyleBackColor = false;
            button_manual.Click += btn_manual_click;
            // 
            // btn_exc
            // 
            btn_exc.Location = new Point(199, 231);
            btn_exc.Name = "btn_exc";
            btn_exc.Size = new Size(85, 36);
            btn_exc.TabIndex = 19;
            btn_exc.Text = "EXECUTE";
            btn_exc.UseVisualStyleBackColor = false;
            btn_exc.Click += btn_exc_click;
            // 
            // label21
            // 
            label21.AutoSize = true;
            label21.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label21.Location = new Point(16, 239);
            label21.Name = "label21";
            label21.Size = new Size(54, 17);
            label21.TabIndex = 18;
            label21.Text = "Axis Z :";
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
            groupBox8.ForeColor = SystemColors.ButtonHighlight;
            groupBox8.Location = new Point(6, 17);
            groupBox8.Name = "groupBox8";
            groupBox8.Size = new Size(408, 65);
            groupBox8.TabIndex = 13;
            groupBox8.TabStop = false;
            groupBox8.Text = "View";
            // 
            // label26
            // 
            label26.AutoSize = true;
            label26.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label26.Location = new Point(281, 25);
            label26.Name = "label26";
            label26.Size = new Size(24, 17);
            label26.TabIndex = 16;
            label26.Text = "Z :";
            // 
            // textBox_viewplc_z
            // 
            textBox_viewplc_z.Location = new Point(311, 19);
            textBox_viewplc_z.Name = "textBox_viewplc_z";
            textBox_viewplc_z.Size = new Size(71, 25);
            textBox_viewplc_z.TabIndex = 15;
            // 
            // label25
            // 
            label25.AutoSize = true;
            label25.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label25.Location = new Point(145, 24);
            label25.Name = "label25";
            label25.Size = new Size(24, 17);
            label25.TabIndex = 14;
            label25.Text = "Y :";
            // 
            // textBox_viewplc_y
            // 
            textBox_viewplc_y.Location = new Point(175, 19);
            textBox_viewplc_y.Name = "textBox_viewplc_y";
            textBox_viewplc_y.Size = new Size(71, 25);
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
            textBox_viewplc_x.Location = new Point(36, 19);
            textBox_viewplc_x.Name = "textBox_viewplc_x";
            textBox_viewplc_x.Size = new Size(71, 25);
            textBox_viewplc_x.TabIndex = 11;
            // 
            // numericUpDown_z
            // 
            numericUpDown_z.DecimalPlaces = 2;
            numericUpDown_z.Location = new Point(87, 239);
            numericUpDown_z.Name = "numericUpDown_z";
            numericUpDown_z.Size = new Size(90, 23);
            numericUpDown_z.TabIndex = 17;
            // 
            // label20
            // 
            label20.AutoSize = true;
            label20.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label20.Location = new Point(16, 169);
            label20.Name = "label20";
            label20.Size = new Size(54, 17);
            label20.TabIndex = 16;
            label20.Text = "Axis Y :";
            // 
            // numericUpDown_y
            // 
            numericUpDown_y.DecimalPlaces = 2;
            numericUpDown_y.Location = new Point(87, 169);
            numericUpDown_y.Name = "numericUpDown_y";
            numericUpDown_y.Size = new Size(90, 23);
            numericUpDown_y.TabIndex = 15;
            // 
            // label19
            // 
            label19.AutoSize = true;
            label19.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label19.Location = new Point(11, 98);
            label19.Name = "label19";
            label19.Size = new Size(55, 17);
            label19.TabIndex = 14;
            label19.Text = "Axis X :";
            // 
            // numericUpDown_x
            // 
            numericUpDown_x.DecimalPlaces = 2;
            numericUpDown_x.Location = new Point(82, 98);
            numericUpDown_x.Name = "numericUpDown_x";
            numericUpDown_x.Size = new Size(90, 23);
            numericUpDown_x.TabIndex = 12;
            // 
            // textBox_theta1
            // 
            textBox_theta1.Location = new Point(62, 35);
            textBox_theta1.Name = "textBox_theta1";
            textBox_theta1.Size = new Size(83, 25);
            textBox_theta1.TabIndex = 0;
            // 
            // textBox_theta2
            // 
            textBox_theta2.Location = new Point(62, 64);
            textBox_theta2.Name = "textBox_theta2";
            textBox_theta2.Size = new Size(83, 25);
            textBox_theta2.TabIndex = 1;
            // 
            // textBox_theta3
            // 
            textBox_theta3.Location = new Point(62, 93);
            textBox_theta3.Name = "textBox_theta3";
            textBox_theta3.Size = new Size(83, 25);
            textBox_theta3.TabIndex = 2;
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
            // btn_FWD
            // 
            btn_FWD.Image = (Image)resources.GetObject("btn_FWD.Image");
            btn_FWD.Location = new Point(169, 64);
            btn_FWD.Name = "btn_FWD";
            btn_FWD.Size = new Size(56, 23);
            btn_FWD.TabIndex = 6;
            btn_FWD.UseVisualStyleBackColor = true;
            btn_FWD.Click += btn_FWD_click;
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
            groupBox1.ForeColor = SystemColors.ButtonHighlight;
            groupBox1.Location = new Point(0, 410);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(250, 148);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Forward Kinematics";
            // 
            // textBox_x
            // 
            textBox_x.Location = new Point(67, 37);
            textBox_x.Name = "textBox_x";
            textBox_x.Size = new Size(83, 25);
            textBox_x.TabIndex = 0;
            // 
            // textBox_y
            // 
            textBox_y.Location = new Point(67, 66);
            textBox_y.Name = "textBox_y";
            textBox_y.Size = new Size(83, 25);
            textBox_y.TabIndex = 1;
            // 
            // textBox_z
            // 
            textBox_z.Location = new Point(67, 95);
            textBox_z.Name = "textBox_z";
            textBox_z.Size = new Size(83, 25);
            textBox_z.TabIndex = 2;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.Location = new Point(171, 41);
            label4.Name = "label4";
            label4.Size = new Size(25, 17);
            label4.TabIndex = 4;
            label4.Text = ": X";
            label4.Click += label4_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.Location = new Point(171, 70);
            label5.Name = "label5";
            label5.Size = new Size(24, 17);
            label5.TabIndex = 5;
            label5.Text = ": Y";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label6.Location = new Point(171, 99);
            label6.Name = "label6";
            label6.Size = new Size(24, 17);
            label6.TabIndex = 6;
            label6.Text = ": Z";
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
            groupBox2.ForeColor = SystemColors.ButtonHighlight;
            groupBox2.Location = new Point(583, 409);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(250, 148);
            groupBox2.TabIndex = 3;
            groupBox2.TabStop = false;
            groupBox2.Text = "Inverse Kinematics";
            // 
            // textBox_view_x
            // 
            textBox_view_x.Location = new Point(36, 30);
            textBox_view_x.Name = "textBox_view_x";
            textBox_view_x.Size = new Size(83, 25);
            textBox_view_x.TabIndex = 0;
            // 
            // textBox_view_y
            // 
            textBox_view_y.Location = new Point(36, 61);
            textBox_view_y.Name = "textBox_view_y";
            textBox_view_y.Size = new Size(83, 25);
            textBox_view_y.TabIndex = 1;
            // 
            // textBox_view_z
            // 
            textBox_view_z.Location = new Point(36, 92);
            textBox_view_z.Name = "textBox_view_z";
            textBox_view_z.Size = new Size(83, 25);
            textBox_view_z.TabIndex = 2;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label7.Location = new Point(6, 41);
            label7.Name = "label7";
            label7.Size = new Size(25, 17);
            label7.TabIndex = 4;
            label7.Text = "X :";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label8.Location = new Point(137, 41);
            label8.Name = "label8";
            label8.Size = new Size(58, 17);
            label8.TabIndex = 5;
            label8.Text = "Theta1 :";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label9.Location = new Point(137, 70);
            label9.Name = "label9";
            label9.Size = new Size(58, 17);
            label9.TabIndex = 6;
            label9.Text = "Theta2 :";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label10.Location = new Point(137, 99);
            label10.Name = "label10";
            label10.Size = new Size(58, 17);
            label10.TabIndex = 7;
            label10.Text = "Theta3 :";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label11.Location = new Point(6, 70);
            label11.Name = "label11";
            label11.Size = new Size(24, 17);
            label11.TabIndex = 8;
            label11.Text = "Y :";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label12.Location = new Point(6, 99);
            label12.Name = "label12";
            label12.Size = new Size(24, 17);
            label12.TabIndex = 9;
            label12.Text = "Z :";
            // 
            // textBox_view_theta1
            // 
            textBox_view_theta1.Location = new Point(213, 63);
            textBox_view_theta1.Name = "textBox_view_theta1";
            textBox_view_theta1.Size = new Size(83, 25);
            textBox_view_theta1.TabIndex = 10;
            // 
            // textBox_view_theta2
            // 
            textBox_view_theta2.Location = new Point(213, 31);
            textBox_view_theta2.Name = "textBox_view_theta2";
            textBox_view_theta2.Size = new Size(83, 25);
            textBox_view_theta2.TabIndex = 11;
            // 
            // textBox_view_theta3
            // 
            textBox_view_theta3.Location = new Point(213, 91);
            textBox_view_theta3.Name = "textBox_view_theta3";
            textBox_view_theta3.Size = new Size(83, 25);
            textBox_view_theta3.TabIndex = 12;
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
            groupBox3.ForeColor = SystemColors.ButtonHighlight;
            groupBox3.Location = new Point(256, 410);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(321, 148);
            groupBox3.TabIndex = 4;
            groupBox3.TabStop = false;
            groupBox3.Text = "Update";
            // 
            // pictureBox
            // 
            pictureBox.Location = new Point(839, 1);
            pictureBox.Name = "pictureBox";
            pictureBox.Size = new Size(381, 557);
            pictureBox.TabIndex = 7;
            pictureBox.TabStop = false;
            // 
            // groupBox5
            // 
            groupBox5.Controls.Add(btn_stopvideo);
            groupBox5.Controls.Add(btn_startvideo);
            groupBox5.ForeColor = SystemColors.ButtonHighlight;
            groupBox5.Location = new Point(16, 129);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new Size(196, 69);
            groupBox5.TabIndex = 9;
            groupBox5.TabStop = false;
            groupBox5.Text = "Camera";
            // 
            // btn_stopvideo
            // 
            btn_stopvideo.Location = new Point(111, 22);
            btn_stopvideo.Name = "btn_stopvideo";
            btn_stopvideo.Size = new Size(75, 40);
            btn_stopvideo.TabIndex = 8;
            btn_stopvideo.Text = "Close";
            btn_stopvideo.UseVisualStyleBackColor = false;
            btn_stopvideo.Click += btn_stopvideo_click;
            // 
            // btn_startvideo
            // 
            btn_startvideo.Location = new Point(22, 22);
            btn_startvideo.Name = "btn_startvideo";
            btn_startvideo.Size = new Size(76, 40);
            btn_startvideo.TabIndex = 7;
            btn_startvideo.Text = "Open";
            btn_startvideo.UseVisualStyleBackColor = false;
            btn_startvideo.Click += btn_startvideo_Click;
            // 
            // groupBox7
            // 
            groupBox7.BackColor = SystemColors.ActiveCaptionText;
            groupBox7.Controls.Add(button_home_1);
            groupBox7.Controls.Add(button_stop);
            groupBox7.Controls.Add(button_start);
            groupBox7.Controls.Add(btn_auto);
            groupBox7.ForeColor = SystemColors.ButtonHighlight;
            groupBox7.Location = new Point(16, 22);
            groupBox7.Name = "groupBox7";
            groupBox7.Size = new Size(196, 103);
            groupBox7.TabIndex = 15;
            groupBox7.TabStop = false;
            groupBox7.Text = "MENU";
            // 
            // button_home_1
            // 
            button_home_1.Location = new Point(24, 56);
            button_home_1.Name = "button_home_1";
            button_home_1.Size = new Size(74, 32);
            button_home_1.TabIndex = 17;
            button_home_1.Text = "Home";
            button_home_1.UseVisualStyleBackColor = false;
            button_home_1.Click += btn_home_1;
            // 
            // button_stop
            // 
            button_stop.Location = new Point(111, 20);
            button_stop.Name = "button_stop";
            button_stop.Size = new Size(75, 30);
            button_stop.TabIndex = 18;
            button_stop.Text = "Stop";
            button_stop.Click += btn_emergency;
            // 
            // button_start
            // 
            button_start.Location = new Point(23, 20);
            button_start.Name = "button_start";
            button_start.Size = new Size(75, 32);
            button_start.TabIndex = 15;
            button_start.Text = "Start";
            button_start.UseVisualStyleBackColor = false;
            button_start.Click += btn_start_click;
            // 
            // btn_auto
            // 
            btn_auto.ForeColor = SystemColors.ButtonHighlight;
            btn_auto.Location = new Point(111, 56);
            btn_auto.Name = "btn_auto";
            btn_auto.Size = new Size(77, 32);
            btn_auto.TabIndex = 14;
            btn_auto.Text = "AUTO";
            btn_auto.UseVisualStyleBackColor = false;
            btn_auto.Click += btn_auto_click;
            // 
            // groupBox9
            // 
            groupBox9.BackColor = SystemColors.ActiveCaptionText;
            groupBox9.Controls.Add(textBox_vat_3);
            groupBox9.Controls.Add(textBox_vat_2);
            groupBox9.Controls.Add(textBox_vat_1);
            groupBox9.Controls.Add(label17);
            groupBox9.Controls.Add(label16);
            groupBox9.Controls.Add(label15);
            groupBox9.ForeColor = SystemColors.ButtonHighlight;
            groupBox9.Location = new Point(16, 209);
            groupBox9.Name = "groupBox9";
            groupBox9.Size = new Size(196, 182);
            groupBox9.TabIndex = 16;
            groupBox9.TabStop = false;
            groupBox9.Text = "Kết Quả";
            // 
            // textBox_vat_3
            // 
            textBox_vat_3.Location = new Point(89, 138);
            textBox_vat_3.Name = "textBox_vat_3";
            textBox_vat_3.Size = new Size(69, 23);
            textBox_vat_3.TabIndex = 5;
            // 
            // textBox_vat_2
            // 
            textBox_vat_2.Location = new Point(89, 85);
            textBox_vat_2.Name = "textBox_vat_2";
            textBox_vat_2.Size = new Size(69, 23);
            textBox_vat_2.TabIndex = 4;
            // 
            // textBox_vat_1
            // 
            textBox_vat_1.Location = new Point(89, 38);
            textBox_vat_1.Name = "textBox_vat_1";
            textBox_vat_1.Size = new Size(69, 23);
            textBox_vat_1.TabIndex = 3;
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label17.Location = new Point(42, 139);
            label17.Name = "label17";
            label17.Size = new Size(44, 17);
            label17.TabIndex = 2;
            label17.Text = "Vật 3:";
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label16.Location = new Point(42, 86);
            label16.Name = "label16";
            label16.Size = new Size(44, 17);
            label16.TabIndex = 1;
            label16.Text = "Vật 2:";
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label15.Location = new Point(42, 39);
            label15.Name = "label15";
            label15.Size = new Size(44, 17);
            label15.TabIndex = 0;
            label15.Text = "Vật 1:";
            // 
            // groupBox10
            // 
            groupBox10.Controls.Add(groupBox9);
            groupBox10.Controls.Add(groupBox7);
            groupBox10.Controls.Add(groupBox5);
            groupBox10.Font = new Font("Tahoma", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            groupBox10.ForeColor = SystemColors.ButtonHighlight;
            groupBox10.Location = new Point(604, 12);
            groupBox10.Name = "groupBox10";
            groupBox10.Size = new Size(229, 397);
            groupBox10.TabIndex = 17;
            groupBox10.TabStop = false;
            groupBox10.Text = "AUTO";
            // 
            // Form2
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaptionText;
            ClientSize = new Size(1228, 563);
            Controls.Add(groupBox10);
            Controls.Add(groupBox6);
            Controls.Add(pictureBox);
            Controls.Add(groupBox4);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Name = "Form2";
            Text = "Form2";
            Load += Form2_Load;
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            groupBox6.ResumeLayout(false);
            groupBox6.PerformLayout();
            groupBox11.ResumeLayout(false);
            groupBox8.ResumeLayout(false);
            groupBox8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_z).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_y).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_x).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox).EndInit();
            groupBox5.ResumeLayout(false);
            groupBox7.ResumeLayout(false);
            groupBox9.ResumeLayout(false);
            groupBox9.PerformLayout();
            groupBox10.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private ComboBox combox_plc;
        private GroupBox groupBox4;
        private Label label13;
        private Button btn_disconnect;
        private Button btn_connect;
        private Label label_status_connection;
        private Label label14;
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
        private Button btn_exc;
        private Button button_manual;
        private TextBox textBox_theta1;
        private TextBox textBox_theta2;
        private TextBox textBox_theta3;
        private Label label1;
        private Label label2;
        private Label label3;
        private Button btn_FWD;
        private GroupBox groupBox1;
        private TextBox textBox_x;
        private TextBox textBox_y;
        private TextBox textBox_z;
        private Label label4;
        private Label label5;
        private Label label6;
        private Button btn_INV;
        private GroupBox groupBox2;
        private TextBox textBox_view_x;
        private TextBox textBox_view_y;
        private TextBox textBox_view_z;
        private Label label7;
        private Label label8;
        private Label label9;
        private Label label10;
        private Label label11;
        private Label label12;
        private TextBox textBox_view_theta1;
        private TextBox textBox_view_theta2;
        private TextBox textBox_view_theta3;
        private GroupBox groupBox3;
        private PictureBox pictureBox;
        private GroupBox groupBox5;
        private Button btn_stopvideo;
        private Button btn_startvideo;
        private GroupBox groupBox7;
        private Button button_home_1;
        private Button button_stop;
        private Button button_start;
        private Button btn_auto;
        private GroupBox groupBox9;
        private TextBox textBox_vat_3;
        private TextBox textBox_vat_2;
        private TextBox textBox_vat_1;
        private Label label17;
        private Label label16;
        private Label label15;
        private GroupBox groupBox10;
        private Button button_x_add;
        private Button button_y_sub;
        private Button button_y_add;
        private Button button_z_sub;
        private Button button_z_add;
        private Button button_x_sub;
        private GroupBox groupBox11;
    }
}