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
using System.Security.Cryptography;
namespace QLSB
{
    public partial class Login : Form
    {
        string connectionString = ConnectionStringManager.GetConnectionString();

        public Login()
        {
            InitializeComponent();
            this.AcceptButton = btnDN;
        }

        private void Login_Load(object sender, EventArgs e)
        {

        }

        private void labelcloseX_Click(object sender, EventArgs e)
        {
            DialogResult r;
            r = MessageBox.Show("Bạn có muốn thoát", "Thoát", MessageBoxButtons.YesNo);
            if(r == DialogResult.Yes)
            {
                this.Close();
            }
        }
        private bool VerifyPassword(string enteredPassword, string savedHash)
        {  
            return enteredPassword == savedHash;
        }
        private bool LoginUser(string UserName, string PassW)
        {
            string query = "SELECT Passworduser FROM Account WHERE Username = @Username";
            bool isAuthenticated = false;

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.AddWithValue("@Username", UserName);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string savedHash = reader["Passworduser"].ToString();   
                            if (VerifyPassword(PassW, savedHash))
                            {
                                isAuthenticated = true;
                            }
                        }
                    }
                }
            }

            return isAuthenticated;
        }
        
        private void btnDN_Click(object sender, EventArgs e)
         {
            string username = textBoxTenDN.Text;
            string pass = textBoxPassw.Text;
            if(LoginUser(username, pass))
            {
                this.Hide();
                ManHinhChinh mhc = new ManHinhChinh();
                mhc.SetUsername(username);
                mhc.Show();
            }
            else
            {
                MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu. Vui lòng thử lại.");
            }
        }

        private void linkLabelDK_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide();
            Register rg = new Register();
            rg.Show();
        }
    }
}
