namespace Project_CK
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            textBox_matkhau.UseSystemPasswordChar = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string user = textBox_tendangnhap.Text.Trim();
            string pass = textBox_matkhau.Text.Trim();

            // Kiểm tra rỗng
            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu!",
                                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra đăng nhập đúng
            if (user == "admin" && pass == "123")
            {
                // Mở form mới
                Form2 f2 = new Form2();
                f2.Show();

                // Ẩn form login (nếu muốn)
                this.Hide();
            }
            else
            {
                // Sai tài khoản
                MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu!",
                                "Lỗi đăng nhập", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }
    }
}
