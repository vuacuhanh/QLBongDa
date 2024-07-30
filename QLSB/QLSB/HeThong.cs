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
    public partial class HeThong : Form
    {
        string connectionString = ConnectionStringManager.GetConnectionString();
        DataSet ds_HT = new DataSet();
        SqlDataAdapter da_HT;
        public HeThong()
        {
            InitializeComponent();
        }
        void Load_DuLieu_HT()
        {

            string strselect = "select * from Account";
            da_HT = new SqlDataAdapter(strselect, connectionString);
            da_HT.Fill(ds_HT, "Account");
            dataGridViewSystem.DataSource = ds_HT.Tables["Account"];
            DataColumn[] key = new DataColumn[1];
            key[0] = ds_HT.Tables["Account"].Columns[0];
            ds_HT.Tables["Account"].PrimaryKey = key;

        }
        void load_NguoiDung()
        {
            string[] a = { "Quản trị viên", "Người dùng thường" };
            cboUserType.Items.AddRange(a);
        }
        private void HeThong_Load(object sender, EventArgs e)
        {
            Load_DuLieu_HT();
            load_NguoiDung();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridViewSystem_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridViewSystem.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

        }

        private void dataGridViewSystem_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewSystem.SelectedRows.Count > 0)
            {
                // Lấy dòng được chọn
                DataGridViewRow selectedRow = dataGridViewSystem.SelectedRows[0];

                // Lấy giá trị của cột tương ứng từ dòng được chọn
                string TenNguoiDung = selectedRow.Cells["Username"].Value.ToString();
                string MatKhau = selectedRow.Cells["Passworduser"].Value.ToString();
                string DisplayName = selectedRow.Cells["DisplayName"].Value.ToString();
                string UserType = selectedRow.Cells["UserType"].Value.ToString();
                // Gán giá trị vào các TextBox
                txtUserName.Text = TenNguoiDung;
                txtPass.Text = MatKhau;
                txtDisName.Text = DisplayName;
                cboUserType.Text = UserType;
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string searchedUsername = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(searchedUsername))
            {
                DataRow foundRow = ds_HT.Tables["Account"].Rows.Find(searchedUsername);
                if (foundRow != null)
                {
                    txtUserName.Text = foundRow["Username"].ToString();
                    txtPass.Text = foundRow["Passworduser"].ToString();
                    txtDisName.Text = foundRow["DisplayName"].ToString();
                    cboUserType.Text = foundRow["UserType"].ToString();
                    foreach (DataGridViewRow row in dataGridViewSystem.Rows)
                    {
                        if (row.Cells["Username"].Value.ToString() == searchedUsername)
                        {
                            row.Selected = true;
                            dataGridViewSystem.CurrentCell = row.Cells[0];
                            break;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Không có người dùng này", "Search Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Nhập tên người dùng cần tìm.", "Search Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void ClearTextBoxes()
        {
            txtUserName.Text = "";
            txtPass.Text = "";
            txtDisName.Text = "";
            cboUserType.Text = "";
        }

        private void btn_AddHT_Click(object sender, EventArgs e)
        {
            string username = txtUserName.Text.Trim();
            string password = txtPass.Text.Trim();
            string displayName = txtDisName.Text.Trim();
            string userType = cboUserType.Text.Trim();

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string insertQuery = "INSERT INTO Account (Username, Passworduser, DisplayName, UserType) " +
                                             "VALUES (@Username, @Passworduser, @DisplayName, @UserType)";

                        using (SqlCommand command = new SqlCommand(insertQuery, connection))
                        {
                            command.Parameters.AddWithValue("@Username", username);
                            command.Parameters.AddWithValue("@Passworduser", password);
                            command.Parameters.AddWithValue("@DisplayName", displayName);
                            command.Parameters.AddWithValue("@UserType", userType);

                            command.ExecuteNonQuery();
                        }
                    }

                    // Refresh DataGridView
                    Load_DuLieu_HT();
                    ClearTextBoxes();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error adding user to database: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please enter a username and password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private void UpdatePasswordInDatabase(string username, string newPassword)
        {
            try
            {
                string query = "UPDATE Account SET Passworduser = @Passworduser WHERE Username = @Username";

                using (SqlConnection cn = new SqlConnection(connectionString))
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, cn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Passworduser", newPassword);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi cập nhật mật khẩu: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnSaveHT_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtUserName.Text.Trim()) && !string.IsNullOrEmpty(txtPass.Text.Trim()))
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string updateQuery = "UPDATE Account SET Passworduser = @Passworduser, DisplayName = @DisplayName, UserType = @UserType " +
                                             "WHERE Username = @Username";

                        using (SqlCommand command = new SqlCommand(updateQuery, connection))
                        {
                            command.Parameters.AddWithValue("@Username", txtUserName.Text.Trim());
                            command.Parameters.AddWithValue("@Passworduser", txtPass.Text.Trim());
                            command.Parameters.AddWithValue("@DisplayName", txtDisName.Text.Trim());
                            command.Parameters.AddWithValue("@UserType", cboUserType.Text.Trim());

                            command.ExecuteNonQuery();
                        }
                    }

                    // Refresh DataGridView
                    Load_DuLieu_HT();
                    MessageBox.Show("Update successful.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Please enter the required information to update.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating user: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnFixHT_Click(object sender, EventArgs e)
        {
            try
            {
                txtUserName.Enabled = false;
                txtPass.Enabled = true;
                txtDisName.Enabled = true;
                cboUserType.Enabled = true;
                MessageBox.Show("Chế độ chỉnh sửa: Bạn có thể chỉnh sửa mật khẩu, tên hiển thị và loại người dùng.", "Edit Mode", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDeleteHT_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridViewSystem.SelectedRows.Count > 0)
                {
                    DialogResult result = MessageBox.Show("Bạn có muốn xoá người dùng này?", "Xác nhận xoá", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();

                            string deleteQuery = "DELETE FROM Account WHERE Username = @Username";

                            using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                            {
                                command.Parameters.AddWithValue("@Username", txtUserName.Text.Trim());

                                command.ExecuteNonQuery();
                            }
                        }

                        // Refresh DataGridView
                        Load_DuLieu_HT();
                        ClearTextBoxes();
                        MessageBox.Show("Đã xoá người dùng.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Chọn 1 người dùng để xoá.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xoá người dùng: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
