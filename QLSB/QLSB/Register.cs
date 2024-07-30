using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace QLSB
{
    public partial class Register : Form
    {
        string connectionString = ConnectionStringManager.GetConnectionString();

        public Register()
        {
            InitializeComponent();
        }

        private void btn_DangNhap_Click(object sender, EventArgs e)
        {
            this.Hide();
            Login lg = new Login();
            lg.Show();
        }
        private bool IsUserExists(string UserName)
        {
            string query = "SELECT COUNT(*) FROM Account WHERE Username = @Username";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Username", UserName);

                    int userCount = (int)cmd.ExecuteScalar();
                    return userCount > 0;
                }
            }
        }
        private bool RegisterUser(string UserName, string Pass)
        {
            // Kiểm tra xem tài khoản đã tồn tại chưa
            if (IsUserExists(UserName))
            {
                return false; // Tài khoản đã tồn tại
            }

            // Lưu thông tin người dùng vào cơ sở dữ liệu
            string query = "INSERT INTO Account (Username, Passworduser, DisplayName) VALUES (@Username, @Passworduser, @DisplayName)";

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.AddWithValue("@Username", UserName);
                    cmd.Parameters.AddWithValue("@Passworduser", Pass);
                    cmd.Parameters.AddWithValue("@DisplayName", "Display Name Here"); // Thay thế bằng tên hiển thị thích hợp

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }
        private void btn_DangKy_Click(object sender, EventArgs e)
        {
            string userName = txtUserName.Text;
            string pass = txtPass.Text;
            string cpw = txtConfirmPass.Text;
            if (pass != cpw)
            {
                MessageBox.Show("Mật khẩu không khớp");
            }
            else
            {
                if (RegisterUser(userName, pass))
                {
                    MessageBox.Show("Mật khảu đăng ký thành công :)))");
                }
                else
                {
                    MessageBox.Show("Mật khẩu không đăng ký thành công. Vui lòng đăng ký lại!!");
                }
            }
        }

        private void Register_Load(object sender, EventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {

        }
    }
}
