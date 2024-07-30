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
    public partial class QuanLySan : Form
    {
        string connectionString = ConnectionStringManager.GetConnectionString();
        DataSet ds_QLSB = new DataSet();
        SqlDataAdapter da_QLS;
        SqlDataAdapter da_QLLS;
        bool isEditing = true;
        public QuanLySan()
        {
            InitializeComponent();
        }
        void Load_DuLieu_QLS()
        {

            string strselect = "select * from SANBONG";
            da_QLS = new SqlDataAdapter(strselect, connectionString);
            da_QLS.Fill(ds_QLSB, "SANBONG");
            dataGridViewQLS.DataSource = ds_QLSB.Tables["SANBONG"];
            DataColumn[] key = new DataColumn[1];
            key[0] = ds_QLSB.Tables["SANBONG"].Columns[0];
            ds_QLSB.Tables["SANBONG"].PrimaryKey = key;

        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridViewQLS.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }

        private void QuanLySan_Load(object sender, EventArgs e)
        {
            Load_DuLieu_QLS();
            Load_DuLieu_QLLS();
        }
        void Load_DuLieu_QLLS()
        {
            string strselect = "select * from LOAISAN";
            da_QLLS = new SqlDataAdapter(strselect, connectionString);
            da_QLLS.Fill(ds_QLSB, "LOAISAN");
            dataGridViewQLLS.DataSource = ds_QLSB.Tables["LOAISAN"];
            DataColumn[] key = new DataColumn[1];
            key[0] = ds_QLSB.Tables["LOAISAN"].Columns[0];
            ds_QLSB.Tables["LOAISAN"].PrimaryKey = key;

        }
        private void dataGridViewQLLS_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridViewQLS.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtTenSan_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtLoaiSan_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridViewQLS_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewQLS.SelectedRows.Count > 0)
            {
                // Lấy dòng được chọn
                DataGridViewRow selectedRow = dataGridViewQLS.SelectedRows[0];

                // Lấy giá trị của cột tương ứng từ dòng được chọn
                string maSanBong = selectedRow.Cells["MaSanBong"].Value.ToString();
                string maLoaiSan = selectedRow.Cells["MaLoaiSan"].Value.ToString();
                string tenSanBong = selectedRow.Cells["TenSanBong"].Value.ToString();
                string TinhTrang = selectedRow.Cells["TinhTrang"].Value.ToString();


                // Gán giá trị vào các TextBox
                txtMaSan.Text = maSanBong;
                txtLoaiSan.Text = maLoaiSan;
                txtTenSan.Text = tenSanBong;
                txtTinhTrang.Text = TinhTrang;
            }
        }

        private void btnAddQLS_Click(object sender, EventArgs e)
        {
            string maSanBong = txtMaSan.Text;
            string maLoaiSan = txtLoaiSan.Text;
            string tenSanBong = txtTenSan.Text;
            string tinhTrang = txtTinhTrang.Text;

            // Kiểm tra các trường không được để trống
            if (string.IsNullOrWhiteSpace(maSanBong) || string.IsNullOrWhiteSpace(maLoaiSan) || string.IsNullOrWhiteSpace(tenSanBong) || string.IsNullOrWhiteSpace(tinhTrang))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin sân bóng.");
                return;
            }

            // Tạo câu lệnh SQL để thêm sân vào cơ sở dữ liệu
            string strSql = "INSERT INTO SANBONG (MaSanBong, MaLoaiSan, TenSanBong, TinhTrang) " +
                            $"VALUES ('{maSanBong}', '{maLoaiSan}', N'{tenSanBong}', N'{tinhTrang}')";

            // Mở kết nối đến cơ sở dữ liệu
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                // Thực hiện lệnh SQL để thêm sân
                using (SqlCommand cmd = new SqlCommand(strSql, connection))
                {
                    try
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Sân bóng đã được thêm vào cơ sở dữ liệu.");
                        // Sau khi thêm, có thể cần làm mới dữ liệu trên DataGridViewQLS.
                        Load_DuLieu_QLS();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi thêm sân bóng: " + ex.Message);
                    }
                }
            }
            int newRowIdx = dataGridViewQLS.Rows.Count - 1;

            // Clear TextBoxes
            txtMaSan.Clear();
            txtLoaiSan.Clear();
            txtTenSan.Clear();
            txtTinhTrang.Clear();

            // Focus on the newly added row
            dataGridViewQLS.CurrentCell = dataGridViewQLS.Rows[newRowIdx].Cells[0];
            dataGridViewQLS.Rows[newRowIdx].Selected = true;
        }

        private void btnDeleteQLS_Click(object sender, EventArgs e)
        {
            if (dataGridViewQLS.SelectedRows.Count > 0)
            {
                // Lấy dòng được chọn
                DataGridViewRow selectedRow = dataGridViewQLS.SelectedRows[0];

                // Lấy MaSanBong từ dòng được chọn
                string maSanBong = selectedRow.Cells["MaSanBong"].Value.ToString();

                // Xác nhận với người dùng trước khi xóa
                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa sân bóng này?", "Xác nhận xóa", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    // Tạo câu lệnh SQL để xóa sân bóng từ cơ sở dữ liệu
                    string strSql = $"DELETE FROM SANBONG WHERE MaSanBong = '{maSanBong}'";

                    // Mở kết nối đến cơ sở dữ liệu
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        // Thực hiện lệnh SQL để xóa sân bóng
                        using (SqlCommand cmd = new SqlCommand(strSql, connection))
                        {
                            try
                            {
                                cmd.ExecuteNonQuery();
                                MessageBox.Show("Sân bóng đã được xóa khỏi cơ sở dữ liệu.");

                                // Xóa dòng khỏi DataGridView
                                dataGridViewQLS.Rows.Remove(selectedRow);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Lỗi khi xóa sân bóng: " + ex.Message);
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một sân bóng để xóa.");
            }
        }

        private void btnFixQLS_Click(object sender, EventArgs e)
        {
            if (dataGridViewQLS.SelectedRows.Count > 0)
            {
                if (!isEditing)
                {
                    // Bật chế độ chỉnh sửa
                    isEditing = true;

                    // Cho phép chỉnh sửa các TextBox
                    txtMaSan.Enabled = true;
                    txtLoaiSan.Enabled = true;
                    txtTenSan.Enabled = true;
                    txtTinhTrang.Enabled = true;
                }
                else
                {
                    // Lấy thông tin từ TextBox
                    string maSanBong = txtMaSan.Text;
                    string maLoaiSan = txtLoaiSan.Text;
                    string tenSanBong = txtTenSan.Text;
                    string tinhTrang = txtTinhTrang.Text;

                    // Kiểm tra các trường không được để trống
                    if (string.IsNullOrWhiteSpace(maSanBong) || string.IsNullOrWhiteSpace(maLoaiSan) || string.IsNullOrWhiteSpace(tenSanBong) || string.IsNullOrWhiteSpace(tinhTrang))
                    {
                        MessageBox.Show("Vui lòng điền đầy đủ thông tin sân bóng.");
                        return;
                    }

                    // Tạo câu lệnh SQL để cập nhật sân vào cơ sở dữ liệu
                    string strSql = "UPDATE SANBONG SET MaLoaiSan = @MaLoaiSan, TenSanBong = @TenSanBong, TinhTrang = @TinhTrang WHERE MaSanBong = @MaSanBong";

                    // Mở kết nối đến cơ sở dữ liệu
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        // Thực hiện lệnh SQL để cập nhật sân bóng
                        using (SqlCommand cmd = new SqlCommand(strSql, connection))
                        {
                            cmd.Parameters.AddWithValue("@MaSanBong", maSanBong);
                            cmd.Parameters.AddWithValue("@MaLoaiSan", maLoaiSan);
                            cmd.Parameters.AddWithValue("@TenSanBong", tenSanBong);
                            cmd.Parameters.AddWithValue("@TinhTrang", tinhTrang);

                            try
                            {
                                cmd.ExecuteNonQuery();
                                MessageBox.Show("Sân bóng đã được cập nhật trong cơ sở dữ liệu.");

                                // Tắt chế độ chỉnh sửa
                                isEditing = false;

                                // Tắt chỉnh sửa các TextBox
                                txtMaSan.Enabled = false;
                                txtLoaiSan.Enabled = false;
                                txtTenSan.Enabled = false;
                                txtTinhTrang.Enabled = false;

                                // Cập nhật DataGridView
                                Load_DuLieu_QLS();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Lỗi khi cập nhật sân bóng: " + ex.Message);
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một sân bóng để chỉnh sửa.");
            }
        }

        private void btnAddQLLS_Click(object sender, EventArgs e)
        {
            string maLoaiSan = txtMaLoaiSan.Text;
            string tenLoaiSan = txtTenLoaiSan.Text;
            string GiaTien = txtDonGiaLoaiSan.Text;

            // Kiểm tra các trường không được để trống
            if ( string.IsNullOrWhiteSpace(maLoaiSan) || string.IsNullOrWhiteSpace(tenLoaiSan) || string.IsNullOrWhiteSpace(GiaTien))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin sân bóng.");
                return;
            }

            // Tạo câu lệnh SQL để thêm sân vào cơ sở dữ liệu
            string strSql = "INSERT INTO LOAISAN (MaLoaiSan, TenLoaiSan, GiaTien) " +
                            $"VALUES ( '{maLoaiSan}', N'{tenLoaiSan}', N'{GiaTien}')";

            // Mở kết nối đến cơ sở dữ liệu
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                // Thực hiện lệnh SQL để thêm sân
                using (SqlCommand cmd = new SqlCommand(strSql, connection))
                {
                    try
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Sân bóng đã được thêm vào cơ sở dữ liệu.");
                        // Sau khi thêm, có thể cần làm mới dữ liệu trên DataGridViewQLS.
                        Load_DuLieu_QLLS();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi thêm sân bóng: " + ex.Message);
                    }
                }
            }
        }
    

        private void btnDeleteQLLS_Click(object sender, EventArgs e)
        {
            if (dataGridViewQLLS.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridViewQLLS.SelectedRows[0];
                string maLoaiSan = selectedRow.Cells["MaLoaiSan"].Value.ToString();
                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa sân bóng này?", "Xác nhận xóa", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    string strSql = $"DELETE FROM LOAISAN WHERE maLoaiSan = '{maLoaiSan}'";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        using (SqlCommand cmd = new SqlCommand(strSql, connection))
                        {
                            try
                            {
                                cmd.ExecuteNonQuery();
                                MessageBox.Show("Sân bóng đã được xóa khỏi cơ sở dữ liệu.");
                                dataGridViewQLLS.Rows.Remove(selectedRow);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Lỗi khi xóa sân bóng: " + ex.Message);
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một sân bóng để xóa.");
            }
        }

        private void btnFixQLLS_Click(object sender, EventArgs e)
        {
            if (dataGridViewQLLS.SelectedRows.Count > 0)
            {
                if (!isEditing)
                {
                    // Bật chế độ chỉnh sửa
                    isEditing = true;

                    // Cho phép chỉnh sửa các TextBox
                    txtMaLoaiSan.Enabled = true;
                    txtTenLoaiSan.Enabled = true;
                    txtDonGiaLoaiSan.Enabled = true;
                }
                else
                {
                    // Lấy thông tin từ TextBox
                    string maLoaiSan = txtMaLoaiSan.Text;
                    string tenLoaiSan = txtTenLoaiSan.Text;
                    string GiaTien = txtDonGiaLoaiSan.Text;

                    // Kiểm tra các trường không được để trống
                    if ( string.IsNullOrWhiteSpace(maLoaiSan) || string.IsNullOrWhiteSpace(tenLoaiSan) || string.IsNullOrWhiteSpace(GiaTien))
                    {
                        MessageBox.Show("Vui lòng điền đầy đủ thông tin sân bóng.");
                        return;
                    }

                    // Tạo câu lệnh SQL để cập nhật sân vào cơ sở dữ liệu
                    string strSql = "UPDATE LOAISAN SET MaLoaiSan = @MaLoaiSan, TenLoaiSan = @TenLoaiSan, GiaTien = @GiaTien WHERE MaLoaiSan = @MaLoaiSan";

                    // Mở kết nối đến cơ sở dữ liệu
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        // Thực hiện lệnh SQL để cập nhật sân bóng
                        using (SqlCommand cmd = new SqlCommand(strSql, connection))
                        {
                            cmd.Parameters.AddWithValue("@MaLoaiSan", maLoaiSan);
                            cmd.Parameters.AddWithValue("@TenLoaiSan", tenLoaiSan);
                            cmd.Parameters.AddWithValue("@GiaTien", GiaTien);

                            try
                            {
                                cmd.ExecuteNonQuery();
                                MessageBox.Show("Sân bóng đã được cập nhật trong cơ sở dữ liệu.");

                                // Tắt chế độ chỉnh sửa
                                isEditing = false;

                                // Tắt chỉnh sửa các TextBox
                                txtMaLoaiSan.Enabled = false;
                                txtTenLoaiSan.Enabled = false;
                                txtDonGiaLoaiSan.Enabled = false;

                                // Cập nhật DataGridView
                                Load_DuLieu_QLLS();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Lỗi khi cập nhật sân bóng: " + ex.Message);
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một sân bóng để chỉnh sửa.");
            }
        }

        private void dataGridViewQLLS_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewQLLS.SelectedRows.Count > 0)
            {
                // Lấy dòng được chọn
                DataGridViewRow selectedRow = dataGridViewQLLS.SelectedRows[0];

                // Lấy giá trị của cột tương ứng từ dòng được chọn
                string maLoaiSan = selectedRow.Cells["MaLoaiSan"].Value.ToString();
                string tenLoaiSan = selectedRow.Cells["TenLoaiSan"].Value.ToString();
                string GiaTien = selectedRow.Cells["GiaTien"].Value.ToString();


                // Gán giá trị vào các TextBox
                txtMaLoaiSan.Text = maLoaiSan;
                txtTenLoaiSan.Text = tenLoaiSan;
                txtDonGiaLoaiSan.Text = GiaTien;
            }
        }

      

        private void btnRefreshQLLS_Click(object sender, EventArgs e)
        {
            Load_DuLieu_QLLS();
            if (isEditing)
            {
                isEditing = false;
                txtMaLoaiSan.Enabled = false;
                txtTenLoaiSan.Enabled = false;
                txtDonGiaLoaiSan.Enabled = false;
            }
        }

        private void btnRefreshQLS_Click(object sender, EventArgs e)
        {

            Load_DuLieu_QLS();
            if (isEditing)
            {
                isEditing = false;
                txtMaSan.Enabled = false;
                txtLoaiSan.Enabled = false;
                txtTenSan.Enabled = false;
                txtTinhTrang.Enabled = false;
            }
        }

        private void btnSaveQLS_Click(object sender, EventArgs e)
        {

        }

        private void btn_SaveQLLS_Click(object sender, EventArgs e)
        {
            if (dataGridViewQLLS.SelectedRows.Count > 0)
            {
                // Lấy dòng được chọn
                DataGridViewRow selectedRow = dataGridViewQLLS.SelectedRows[0];

                // Lấy thông tin từ TextBox
                string maLoaiSan = txtMaLoaiSan.Text;
                string tenLoaiSan = txtTenLoaiSan.Text;
                string GiaTien = txtDonGiaLoaiSan.Text;

                // Kiểm tra các trường không được để trống
                if (string.IsNullOrWhiteSpace(maLoaiSan) || string.IsNullOrWhiteSpace(tenLoaiSan) || string.IsNullOrWhiteSpace(GiaTien))
                {
                    MessageBox.Show("Vui lòng điền đầy đủ thông tin sân bóng.");
                    return;
                }

                // Lấy maLoaiSan từ dòng được chọn
                string selectedMaLoaiSan = selectedRow.Cells["MaLoaiSan"].Value.ToString();

                // Tạo câu lệnh SQL để cập nhật loại sân vào cơ sở dữ liệu
                string strSql = "UPDATE LOAISAN SET MaLoaiSan = @MaLoaiSan, TenLoaiSan = @TenLoaiSan, GiaTien = @GiaTien WHERE MaLoaiSan = @SelectedMaLoaiSan";

                // Mở kết nối đến cơ sở dữ liệu
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    // Thực hiện lệnh SQL để cập nhật loại sân
                    using (SqlCommand cmd = new SqlCommand(strSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@MaLoaiSan", maLoaiSan);
                        cmd.Parameters.AddWithValue("@TenLoaiSan", tenLoaiSan);
                        cmd.Parameters.AddWithValue("@GiaTien", GiaTien);
                        cmd.Parameters.AddWithValue("@SelectedMaLoaiSan", selectedMaLoaiSan);

                        try
                        {
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Loại sân đã được cập nhật trong cơ sở dữ liệu.");

                            // Tắt chế độ chỉnh sửa
                            isEditing = false;

                            // Tắt chỉnh sửa các TextBox
                            txtMaLoaiSan.Enabled = false;
                            txtTenLoaiSan.Enabled = false;
                            txtDonGiaLoaiSan.Enabled = false;

                            // Cập nhật DataGridView
                            Load_DuLieu_QLLS();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Lỗi khi cập nhật loại sân: " + ex.Message);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một loại sân để chỉnh sửa.");
            }

        }
    }
}
