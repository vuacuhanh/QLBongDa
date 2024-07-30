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
    public partial class QuanLyDoanhThu : Form
    {
        string connectionString = ConnectionStringManager.GetConnectionString();

        DataSet ds_QLSB = new DataSet();
        SqlDataAdapter da_QLKH;
        SqlDataAdapter da_QLNV;
        public QuanLyDoanhThu()
        {
            InitializeComponent();
            dataGridView1.SelectionChanged += DataGridView1_SelectionChanged;
            dataGridView2.SelectionChanged += DataGridView2_SelectionChanged;

        }
        void Load_DuLieu_KH()
        {
            string strselect = "select * from KHACHHANG";
            da_QLKH = new SqlDataAdapter(strselect, connectionString);
            da_QLKH.Fill(ds_QLSB, "KHACHHANG");
            dataGridView1.DataSource = ds_QLSB.Tables["KHACHHANG"];
            DataColumn[] key = new DataColumn[1];
            key[0] = ds_QLSB.Tables["KHACHHANG"].Columns[0];
            ds_QLSB.Tables["KHACHHANG"].PrimaryKey = key;
        }
        void Load_Gender()
        {
            string[] gender = { "Nam", "Nữ" };
            comboBoxGender.Items.AddRange(gender);
        }
       
        private void QuanLy_Load(object sender, EventArgs e)
        {
            Load_DuLieu_KH();
            Load_Gender();
            LoadDataToDataGridView2();

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtIDKH.Text == string.Empty)
            {
                MessageBox.Show("Chưa nhập mã khách hàng");
                txtIDKH.Focus();
                return;
            }
            if (txtNameKH.Text == string.Empty)
            {
                MessageBox.Show("Chưa nhập tên khách hàng");
                txtNameKH.Focus();
                return;
            }
            DataRow existingRow = ds_QLSB.Tables["KHACHHANG"].Rows.Find(txtIDKH.Text);
            if (txtIDKH.Enabled == true)
            {
                if (existingRow != null)
                {
                    MessageBox.Show("Mã khách hàng đã tồn tại. Vui lòng sử dụng mã khách hàng khác.");
                    return;
                }
                DataRow insert_new = ds_QLSB.Tables["KHACHHANG"].NewRow();
                insert_new["MaKH"] = txtIDKH.Text;
                insert_new["TenKH"] = txtNameKH.Text;
                insert_new["GioiTinh"] = comboBoxGender.SelectedItem.ToString();
                insert_new["SDT"] = txtSDT.Text;
                insert_new["DiaChi"] = txtAdrr.Text;
                ds_QLSB.Tables["KHACHHANG"].Rows.Add(insert_new);
            }
            else
            {
                DataRow update_New = ds_QLSB.Tables["KHACHHANG"].Rows.Find(txtIDKH.Text);
                if (update_New != null)
                {
                    update_New["TenKH"] = txtNameKH.Text;
                    update_New["GioiTinh"] = comboBoxGender.SelectedItem.ToString();
                    update_New["SDT"] = txtSDT.Text;
                    update_New["DiaChi"] = txtAdrr.Text;
                }
            }
            SqlCommandBuilder cmb = new SqlCommandBuilder(da_QLKH);
            da_QLKH.Update(ds_QLSB, "KHACHHANG");
            MessageBox.Show("Thành Công");
        }
        private void ClearInputFields()
        {
            txtIDKH.Text = string.Empty;
            txtNameKH.Text = string.Empty;
            comboBoxGender.SelectedIndex = -1;
            txtSDT.Text = string.Empty;
            txtAdrr.Text = string.Empty;
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (txtIDKH.Text == string.Empty)
            {
                MessageBox.Show("Chưa chọn khách hàng cần xóa");
                return;
            }

            DataRow deleteRow = ds_QLSB.Tables["KHACHHANG"].Rows.Find(txtIDKH.Text);
            if (deleteRow != null)
            {
                deleteRow.Delete();
                SqlCommandBuilder cmb = new SqlCommandBuilder(da_QLKH);
                da_QLKH.Update(ds_QLSB, "KHACHHANG");
                MessageBox.Show("Xóa thành công");

                // Sau khi xóa, bạn có thể làm sạch các điều khiển nhập liệu
                ClearInputFields();
            }
            else
            {
                MessageBox.Show("Không tìm thấy khách hàng cần xóa");
            }
        }
        private bool isEditing = false;

        private void btnSua_Click(object sender, EventArgs e)
        {
            
            if (dataGridView1.SelectedRows.Count > 0)
            {
                if (!isEditing)
                {
                    // Bật chế độ chỉnh sửa
                    isEditing = true;

                    // Cho phép chỉnh sửa các TextBox
                    txtIDKH.Enabled = false; 
                    txtNameKH.Enabled = true;
                    comboBoxGender.Enabled = true;
                    txtSDT.Enabled = true;
                    txtAdrr.Enabled = true;
                }
                else
                {
                    // Lấy thông tin từ TextBox
                    string maKH = txtIDKH.Text;
                    string tenKH = txtNameKH.Text;
                    string GioiTinh = comboBoxGender.Text;
                    string SDT = txtSDT.Text;
                    string DiaChi = txtAdrr.Text;

                    // Kiểm tra các trường không được để trống
                    if (string.IsNullOrWhiteSpace(maKH) || string.IsNullOrWhiteSpace(tenKH) || string.IsNullOrWhiteSpace(GioiTinh) || string.IsNullOrWhiteSpace(SDT) || string.IsNullOrWhiteSpace(DiaChi))
                    {
                        MessageBox.Show("Vui lòng điền đầy đủ thông tin khách hàng.");
                        return;
                    }

                    // Tạo câu lệnh SQL để cập nhật sân vào cơ sở dữ liệu
                    string strSql = "UPDATE KHACHHANG SET MaKH = @MaKH, TenKH = @TenKH, GioiTinh = @GioiTinh,SDT = @SDT,DiaChi = @DiaChi WHERE MaKH = @MaKH";

                    // Mở kết nối đến cơ sở dữ liệu
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        // Thực hiện lệnh SQL để cập nhật sân bóng
                        using (SqlCommand cmd = new SqlCommand(strSql, connection))
                        {
                            cmd.Parameters.AddWithValue("@MaKH", maKH);
                            cmd.Parameters.AddWithValue("@TenKH", tenKH);
                            cmd.Parameters.AddWithValue("@GioiTinh", GioiTinh);
                            cmd.Parameters.AddWithValue("@SDT", SDT);
                            cmd.Parameters.AddWithValue("@DiaChi", DiaChi);
                            try
                            {
                                cmd.ExecuteNonQuery();
                                MessageBox.Show("Khách hàng đã được cập nhật trong cơ sở dữ liệu.");

                                // Tắt chế độ chỉnh sửa
                                isEditing = false;

                                // Tắt chỉnh sửa các TextBox
                                txtNameKH.Enabled = false;
                                comboBoxGender.Enabled = false;
                                txtSDT.Enabled = false;
                                txtAdrr.Enabled = false;

                                // Cập nhật DataGridView
                                Load_DuLieu_KH();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Lỗi khi cập nhật khách hàng: " + ex.Message);
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một khách hàng để chỉnh sửa.");
            }

        }

        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                // Populate textboxes with data from the selected row
                txtIDKH.Text = selectedRow.Cells["MaKH"].Value.ToString();
                txtNameKH.Text = selectedRow.Cells["TenKH"].Value.ToString();
                comboBoxGender.SelectedItem = selectedRow.Cells["GioiTinh"].Value.ToString();
                txtSDT.Text = selectedRow.Cells["SDT"].Value.ToString();
                txtAdrr.Text = selectedRow.Cells["DiaChi"].Value.ToString();
            }
        }

        private void dataGridViewNV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void btnAddNV_Click(object sender, EventArgs e)
        {
            
        }

        private void btnDeleteNV_Click(object sender, EventArgs e)
        {
            
        }

        private void btnFixNV_Click(object sender, EventArgs e)
        {

        }


        private void LoadDataToDataGridView2()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Thực hiện truy vấn để lấy dữ liệu từ bảng HOADON
                string query = "SELECT MaKH, MaSanBong, GioDa, DonGia, ThanhTien FROM HOADON";
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                // Kiểm tra xem cột "ThanhTien" đã tồn tại trong dataTable chưa
                if (!dataTable.Columns.Contains("ThanhTien"))
                {
                    DataColumn thanhTienColumn = new DataColumn("ThanhTien", typeof(decimal));
                    dataTable.Columns.Add(thanhTienColumn);
                }

                // Lặp qua từng dòng trong DataTable để tính và cập nhật ThanhTien
                foreach (DataRow row in dataTable.Rows)
                {
                    // Lấy giá trị từ cột GioDa
                    object gioDaValue = row["GioDa"];

                    // Kiểm tra xem giá trị có tồn tại và không phải là DBNull không
                    if (gioDaValue != DBNull.Value)
                    {
                        // Chuyển đổi giá trị sang decimal
                        decimal soPhutDa = Convert.ToDecimal(gioDaValue);

                        // Lấy giá trị từ cột DonGia
                        object donGiaValue = row["DonGia"];

                        // Kiểm tra xem giá trị có tồn tại và không phải là DBNull không
                        if (donGiaValue != DBNull.Value)
                        {
                            // Chuyển đổi giá trị sang decimal
                            decimal donGia = Convert.ToDecimal(donGiaValue);

                            // Gọi hàm TinhThanhTien để tính ThanhTien
                            decimal thanhTien = TinhThanhTien(soPhutDa, donGia);

                            // Gán giá trị ThanhTien vào cột ThanhTien của dòng hiện tại
                            row["ThanhTien"] = thanhTien;
                        }
                    }
                }

                // Đổ dữ liệu lên DataGridView2
                dataGridView2.DataSource = dataTable;

                // Cài đặt sự kiện để xử lý khi người dùng chọn một dòng trên DataGridView2
                dataGridView2.SelectionChanged += DataGridView2_SelectionChanged;

                // Hiển thị dữ liệu của dòng đầu tiên (nếu có)
                if (dataGridView2.Rows.Count > 0)
                {
                    DisplayDataFromSelectedRow2(0);
                }
            }
        }

        private void DataGridView2_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView2.SelectedRows[0];

                // Lấy giá trị từ cột ThanhTien
                object thanhTienValue = selectedRow.Cells["ThanhTien"].Value;

                // Kiểm tra xem giá trị có tồn tại và không phải là DBNull không
                txtThanhTienHD.Text = thanhTienValue != DBNull.Value ? thanhTienValue.ToString() : "Không có thông tin Thành tiền.";

                // Gọi hàm DisplayDataFromSelectedRow2 để hiển thị dữ liệu từ dòng được chọn lên các TextBox khác
                DisplayDataFromSelectedRow2(selectedRow.Index);
            }
        }

        private void DisplayDataFromSelectedRow2(int rowIndex)
        {
            DataGridViewRow selectedRow = dataGridView2.Rows[rowIndex];

            // Hiển thị dữ liệu trong các TextBox và DateTimePicker
            txtMaKHHD.Text = selectedRow.Cells["MaKH"].Value.ToString();
            txtMaSanHD.Text = selectedRow.Cells["MaSanBong"].Value.ToString();
            txtGioDaHD.Text = selectedRow.Cells["GioDa"].Value.ToString();
            txtDonGiaHD.Text = selectedRow.Cells["DonGia"].Value.ToString();

            // Load giá trị từ cột ThanhTien (nếu có)
            object thanhTienValue = selectedRow.Cells["ThanhTien"].Value;
            txtThanhTienHD.Text = thanhTienValue != DBNull.Value ? thanhTienValue.ToString() : "Không có thông tin Thành tiền.";
        }

        private void DataGridView2_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Kiểm tra nếu đang xử lý cột ThanhTien
            if (e.ColumnIndex == dataGridView2.Columns["ThanhTien"].Index && e.RowIndex >= 0)
            {
                // Lấy giá trị của cột GioDa và DonGia
                decimal gioDa = Convert.ToDecimal(dataGridView2.Rows[e.RowIndex].Cells["GioDa"].Value);
                decimal donGia = Convert.ToDecimal(dataGridView2.Rows[e.RowIndex].Cells["DonGia"].Value);

                // Tính giá trị ThanhTien
                decimal thanhTien = gioDa * donGia;

                // Hiển thị giá trị ThanhTien trong ô
                e.Value = thanhTien.ToString("N2"); // Format giá trị để hiển thị đúng định dạng số

                // Ngăn chặn DataGridView hiển thị giá trị từ DataSource để tránh bị ghi đè
                e.FormattingApplied = true;
            }
        }

        private decimal TinhThanhTien(decimal soPhutDa, decimal donGia)
        {
            // Tính ThanhTien: (soPhutDa / 60) * donGia
            return (soPhutDa / 60) * donGia;
        }


        private decimal TinhDoanhThu(int thang, int nam)
        {
            decimal tongDoanhThu = 0;

            // Kết nối đến cơ sở dữ liệu
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Thực hiện truy vấn để lấy tổng doanh thu của tháng và năm cụ thể
                string query = "SELECT " +
                               "    SUM(h.ThanhTien) AS TongDoanhThu " +
                               "FROM " +
                               "    DATSAN d " +
                               "JOIN " +
                               "    HOADON h ON d.MaSanBong = h.MaSanBong AND d.MaKH = h.MaKH " +
                               "WHERE " +
                               "    d.BatDau IS NOT NULL " +
                               "    AND YEAR(d.BatDau) = @Nam " +
                               "    AND MONTH(d.BatDau) = @Thang";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Thang", thang);
                    cmd.Parameters.AddWithValue("@Nam", nam);

                    // Thực hiện truy vấn và lấy kết quả
                    object result = cmd.ExecuteScalar();

                    // Kiểm tra xem kết quả có tồn tại và không phải là DBNull không
                    if (result != null && result != DBNull.Value)
                    {
                        tongDoanhThu = Convert.ToDecimal(result);
                    }
                }
            }

            return tongDoanhThu;
        }
        private decimal TinhDoanhThuTheoNam(int nam)
        {
            decimal tongDoanhThu = 0;

            // Kết nối đến cơ sở dữ liệu
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Thực hiện truy vấn để lấy tổng doanh thu của năm cụ thể
                string query = "SELECT " +
                               "    SUM(h.ThanhTien) AS TongDoanhThu " +
                               "FROM " +
                               "    DATSAN d " +
                               "JOIN " +
                               "    HOADON h ON d.MaSanBong = h.MaSanBong AND d.MaKH = h.MaKH " +
                               "WHERE " +
                               "    d.BatDau IS NOT NULL " +
                               "    AND YEAR(d.BatDau) = @Nam";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Nam", nam);

                    // Thực hiện truy vấn và lấy kết quả
                    object result = cmd.ExecuteScalar();

                    // Kiểm tra xem kết quả có tồn tại và không phải là DBNull không
                    if (result != null && result != DBNull.Value)
                    {
                        tongDoanhThu = Convert.ToDecimal(result);
                    }
                }
            }

            return tongDoanhThu;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            int thang, nam;

            // Kiểm tra người dùng đã nhập đúng định dạng cho tháng và năm chưa
            if (int.TryParse(textThang.Text, out thang) && int.TryParse(textNam.Text, out nam))
            {
                // Gọi hàm tính doanh thu theo tháng và năm từ TextBox
                decimal tongDoanhThuTheoThang = TinhDoanhThu(thang, nam);

                // Hiển thị kết quả lên TextBox
                textTongDoanhThu.Text = tongDoanhThuTheoThang.ToString("N2"); // Format giá trị để hiển thị đúng định dạng số
            }
            else if (int.TryParse(txtSearch.Text, out nam))
            {
                // Gọi hàm tính doanh thu theo năm từ TextBox
                decimal tongDoanhThuTheoNam = TinhDoanhThuTheoNam(nam);

                // Hiển thị kết quả lên TextBox
                textTongDoanhThu.Text = tongDoanhThuTheoNam.ToString("N2"); // Format giá trị để hiển thị đúng định dạng số
            }
            else
            {
                MessageBox.Show("Vui lòng nhập tháng hoặc năm hợp lệ.");
            }
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView2.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

        }
    }
}

