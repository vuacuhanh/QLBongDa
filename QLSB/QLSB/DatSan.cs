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
using System.Globalization;

namespace QLSB
{
    public partial class DatSan : Form
    {
        SqlConnection cn;
        SqlDataAdapter da_sb;
        DataSet ds_sb;
        DataColumn[] key = new DataColumn[1];
        public DatSan()
        {

            InitializeComponent();
            cn = new SqlConnection("Data Source=tranan\\sqlexpress;Initial Catalog=QL_SANBONG_TEST1;Integrated Security=True");
            string strSelect = "select * from DatSan";
            da_sb = new SqlDataAdapter(strSelect, cn);
            ds_sb = new DataSet();
            da_sb.Fill(ds_sb, "DATSAN");
            foreach (DataRow row in ds_sb.Tables["DATSAN"].Rows)
            {
                string maSanBong = row["MaSanBong"].ToString().Trim();
                row["MaSanBong"] = maSanBong;
            }
            // Thiết lập DataColumn như bạn đã chỉ định trước đó
            cboMaSan.DataSource = ds_sb.Tables["DATSAN"];
            cboMaSan.DisplayMember = "MaSanBong";
        }


        private void Load_LoaiThue()
        {
            string[] lt = { "Đặt trước", "Trực tiếp" };
            cboLoaiThue.Items.AddRange(lt);
        }
        private void DatSan_Load(object sender, EventArgs e)
        {
            Load_LoaiThue();
            if (ds_sb != null && ds_sb.Tables.Contains("DATSAN"))
            {
                DataTable dt = ds_sb.Tables["DATSAN"];
                dataGridViewDatSan.DataSource = dt;
            }
            dateTimePickerBatDau.Format = DateTimePickerFormat.Custom;
            dateTimePickerBatDau.CustomFormat = "dd/MM/yyyy";
            dateTimePickerKetThuc.Format = DateTimePickerFormat.Custom;
            dateTimePickerKetThuc.CustomFormat = "dd/MM/yyyy";
        }

        private void cboMaSan_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboMaSan.SelectedIndex >= 0 && ds_sb.Tables.Contains("DATSAN"))
            {
                DataRow selectedRow = ds_sb.Tables["DATSAN"].Rows[cboMaSan.SelectedIndex];

                // Sử dụng Try-Catch để xử lý trường hợp chọn sai kiểu dữ liệu
                try
                {
                    string maKhachHang = selectedRow["MaKH"].ToString().Trim();
                    txtMaKH.Text = maKhachHang;

                    string loaiThue = selectedRow["LoaiThue"].ToString().Trim();
                    cboLoaiThue.Text = loaiThue;

                    string donGia = selectedRow["DonGia"].ToString().Trim();
                    txtDonGia.Text = donGia;

                    // Đảm bảo rằng giá trị của BatDau và KetThuc là kiểu DateTime trước khi chuyển đổi
                    if (selectedRow["BatDau"] is DateTime && selectedRow["KetThuc"] is DateTime)
                    {
                        DateTime batDau = (DateTime)selectedRow["BatDau"];
                        DateTime ketThuc = (DateTime)selectedRow["KetThuc"];

                        dateTimePickerBatDau.Value = batDau.Date;
                        dateTimePickerKetThuc.Value = ketThuc.Date;

                        dateTimePickerGioBatDau.Value = batDau;
                        dateTimePickerGioKetThuc.Value = ketThuc;
                    }
                }
                catch (Exception ex)
                {
                    
                }
            }
        }
        private void LoadThoiGianToDateTimePickers()
        {
            if (ds_sb != null && ds_sb.Tables.Contains("DATSAN"))
            {
                DataTable dt = ds_sb.Tables["DATSAN"];
                if (dt.Rows.Count > 0)
                {
                    DateTime batDau = (DateTime)dt.Rows[0]["BatDau"];
                    DateTime ketThuc = (DateTime)dt.Rows[0]["KetThuc"];
                    dateTimePickerBatDau.Value = batDau;
                    dateTimePickerKetThuc.Value = ketThuc;
                }
            }
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridViewDatSan.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }

        private void dataGridView2_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewDatSan.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridViewDatSan.SelectedRows[0];
                DateTime batDau = (DateTime)selectedRow.Cells["BatDau"].Value;
                DateTime ketThuc = (DateTime)selectedRow.Cells["KetThuc"].Value;

                dateTimePickerBatDau.Value = batDau.Date;
                dateTimePickerKetThuc.Value = ketThuc.Date;
                dateTimePickerGioBatDau.Value = batDau;
                dateTimePickerGioKetThuc.Value = ketThuc;
            }
        }

        private void btnDatSan_Click(object sender, EventArgs e)
        {
            string maSanBong = cboMaSan.Text;
            DateTime ngayBatDau = dateTimePickerBatDau.Value;
            DateTime ngayKetThuc = dateTimePickerKetThuc.Value;
            DateTime gioBatDau = dateTimePickerGioBatDau.Value;
            DateTime gioKetThuc = dateTimePickerGioKetThuc.Value;
            DateTime batDau = ngayBatDau.Add(gioBatDau.TimeOfDay);
            DateTime ketThuc = ngayKetThuc.Add(gioKetThuc.TimeOfDay);
            string maKhachHang = txtMaKH.Text;
            string loaiThue = cboLoaiThue.Text;
            decimal donGia = Convert.ToDecimal(txtDonGia.Text);

            // Kiểm tra sự khả dụng của sân bóng và trùng lặp thời gian
            if (IsSanBongAvailable(maSanBong, batDau, ketThuc))
            {
                if (ketThuc > batDau)
                {
                    if (isEditMode)
                    {
                        // Nếu đang ở chế độ sửa đổi, cập nhật thông tin cho đặt sân đã chọn
                        UpdateDatSan(editedMaSanBong, editedMaKH, loaiThue, donGia, batDau, ketThuc);
                        MessageBox.Show("Cập nhật thông tin sân thành công!");
                    }
                    else
                    {
                        // Nếu không ở chế độ sửa đổi, thêm một đặt sân mới
                        InsertDatSan(maSanBong, batDau, ketThuc, maKhachHang, loaiThue, donGia);
                        //MessageBox.Show("Đặt sân thành công!");
                    }

                    // Làm mới DataSet
                    ds_sb.Clear();
                    da_sb.Fill(ds_sb, "DATSAN");

                    // Cập nhật lại DataGridView ngay sau khi đã lưu
                    RefreshDataGridView();

                    // Làm mới ComboBox và TextBox
                    cboMaSan.DataSource = ds_sb.Tables["DATSAN"];
                    cboMaSan.DisplayMember = "MaSanBong";

                    txtMaKH.Text = string.Empty;
                    cboLoaiThue.Text = string.Empty;
                    txtDonGia.Text = string.Empty;
                    dateTimePickerBatDau.Value = DateTime.Now;
                    dateTimePickerKetThuc.Value = DateTime.Now;
                    dateTimePickerGioBatDau.Value = DateTime.Now;
                    dateTimePickerGioKetThuc.Value = DateTime.Now;
                }
                else
                {
                    MessageBox.Show("Giờ kết thúc phải lớn hơn giờ bắt đầu.");
                }
            }
            else
            {
                MessageBox.Show("Sân bóng không khả dụng trong khoảng thời gian đã chọn.");
            }
        }
        private bool IsSanBongAvailable(string maSanBong, DateTime batDau, DateTime ketThuc)
        {
            using (SqlConnection connection = new SqlConnection("Data Source=tranan\\sqlexpress;Initial Catalog=QL_SANBONG_TEST1;Integrated Security=True"))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM DATSAN WHERE MaSanBong = @MaSanBong " +
                               "AND (@BatDau < KetThuc) AND (@KetThuc > BatDau)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MaSanBong", maSanBong);
                    command.Parameters.AddWithValue("@BatDau", batDau);
                    command.Parameters.AddWithValue("@KetThuc", ketThuc);

                    int count = Convert.ToInt32(command.ExecuteScalar());

                    return count == 0;
                }
            }
        }

        private void InsertDatSan(string maSanBong, DateTime batDau, DateTime ketThuc, string maKH, string loaiThue, decimal donGia)
        {
            using (SqlConnection connection = new SqlConnection("Data Source=tranan\\sqlexpress;Initial Catalog=QL_SANBONG_TEST1;Integrated Security=True"))
            {
                connection.Open();
                string query = "INSERT INTO DATSAN (MaSanBong, MaKH, BatDau, KetThuc, LoaiThue, DonGia) " +
                               "VALUES (@MaSanBong, @MaKH, @BatDau, @KetThuc, @LoaiThue, @DonGia)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MaSanBong", maSanBong);
                    command.Parameters.AddWithValue("@MaKH", maKH);
                    command.Parameters.AddWithValue("@BatDau", batDau);
                    command.Parameters.AddWithValue("@KetThuc", ketThuc);
                    command.Parameters.AddWithValue("@LoaiThue", loaiThue);
                    command.Parameters.AddWithValue("@DonGia", donGia);

                    command.ExecuteNonQuery();
                    MessageBox.Show("Đặt sân thành công!");
                }
            }
        }

        private bool IsDatSanExisted(string maSanBong, string maKH, DateTime batDau, DateTime ketThuc)
        {
            if (ds_sb != null && ds_sb.Tables.Contains("DATSAN"))
            {
                DataTable dt = ds_sb.Tables["DATSAN"];

                foreach (DataRow row in dt.Rows)
                {
                    string rowMaSanBong = row["MaSanBong"].ToString();
                    string rowMaKH = row["MaKH"].ToString();

                    // Check for null or empty strings
                    bool isMaSanBongMatch = string.IsNullOrEmpty(maSanBong) || maSanBong == rowMaSanBong;
                    bool isMaKHMatch = string.IsNullOrEmpty(maKH) || maKH == rowMaKH;

                    // Check if the columns contain DBNull.Value before casting to DateTime
                    if (!(row["BatDau"] is DBNull) && !(row["KetThuc"] is DBNull))
                    {
                        DateTime rowBatDau = (DateTime)row["BatDau"];
                        DateTime rowKetThuc = (DateTime)row["KetThuc"];

                        // Check for overlapping time periods
                        bool isTimeOverlap = (batDau >= rowBatDau && batDau < rowKetThuc) || (ketThuc > rowBatDau && ketThuc <= rowKetThuc);

                        if (isMaSanBongMatch && isMaKHMatch && isTimeOverlap)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }


        private void CancelDatSan(string maSanBong, DateTime batDau, DateTime ketThuc)
        {
            using (cn)
            {
                cn.Open();
                string query = "DELETE FROM DATSAN WHERE MaSanBong = @MaSanBong AND BatDau = @BatDau AND KetThuc = @KetThuc";
                using (SqlCommand command = new SqlCommand(query, cn))
                {
                    command.Parameters.AddWithValue("@MaSanBong", maSanBong);
                    command.Parameters.AddWithValue("@BatDau", batDau);
                    command.Parameters.AddWithValue("@KetThuc", ketThuc);

                    command.ExecuteNonQuery();
                    MessageBox.Show("Hủy đặt sân thành công!");
                }
            }
        }
       
        private bool isEditMode = false;
        private string editedMaSanBong = string.Empty;
        private string editedMaKH = string.Empty;

        private void UpdateDatSan(string maSanBong, string maKH, string loaiThue, decimal donGia, DateTime batDau, DateTime ketThuc)
        {
            using (SqlConnection connection = new SqlConnection("Data Source=tranan\\sqlexpress;Initial Catalog=QL_SANBONG_TEST1;Integrated Security=True"))
            {
                connection.Open();
                string query = "UPDATE DATSAN SET LoaiThue = @LoaiThue, DonGia = @DonGia, BatDau = @BatDau, KetThuc = @KetThuc " +
                               "WHERE MaSanBong = @MaSanBong AND MaKH = @MaKH";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MaSanBong", maSanBong);
                    command.Parameters.AddWithValue("@MaKH", maKH);
                    command.Parameters.AddWithValue("@LoaiThue", loaiThue);
                    command.Parameters.AddWithValue("@DonGia", donGia);
                    command.Parameters.AddWithValue("@BatDau", batDau);
                    command.Parameters.AddWithValue("@KetThuc", ketThuc);

                    command.ExecuteNonQuery();
                    MessageBox.Show("Cập nhật thông tin sân thành công!");
                }
            }
        }


        private void RefreshDataGridView()
        {
            if (ds_sb != null && ds_sb.Tables.Contains("DATSAN"))
            {
                DataTable dt = ds_sb.Tables["DATSAN"];
                dataGridViewDatSan.DataSource = dt;
            }
        }
        private void UpdateDateTimePickers()
        {
            if (dataGridViewDatSan.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridViewDatSan.SelectedRows[0];
                DateTime batDau = (DateTime)selectedRow.Cells["BatDau"].Value;
                DateTime ketThuc = (DateTime)selectedRow.Cells["KetThuc"].Value;

                dateTimePickerBatDau.Value = batDau.Date;
                dateTimePickerKetThuc.Value = ketThuc.Date;
                dateTimePickerGioBatDau.Value = batDau;
                dateTimePickerGioKetThuc.Value = ketThuc;
                dateTimePickerGioBatDau.Value = batDau;
                dateTimePickerGioKetThuc.Value = ketThuc;
            }
        }


      

       

        private void btnSuaSan_Click(object sender, EventArgs e)
        {

            if (dataGridViewDatSan.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridViewDatSan.SelectedRows[0];
                editedMaSanBong = selectedRow.Cells["MaSanBong"].Value.ToString();
                editedMaKH = selectedRow.Cells["MaKH"].Value.ToString();
                cboMaSan.Enabled = false;
                txtMaKH.Enabled = false;
                isEditMode = true;
                UpdateDateTimePickers();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một dòng để sửa thông tin sân.");
            }
        }

        private void btnSaveDS_Click(object sender, EventArgs e)
        {
            if (isEditMode)
            {
                string updatedLoaiThue = cboLoaiThue.Text;
                decimal updatedDonGia = Convert.ToDecimal(txtDonGia.Text);
                DateTime updatedBatDau = dateTimePickerBatDau.Value;
                DateTime updatedKetThuc = dateTimePickerKetThuc.Value;
                DateTime gioBatDau = dateTimePickerGioBatDau.Value;
                DateTime gioKetThuc = dateTimePickerGioKetThuc.Value;

                updatedBatDau = updatedBatDau.Add(gioBatDau.TimeOfDay);
                updatedKetThuc = updatedKetThuc.Add(gioKetThuc.TimeOfDay);

                // Cập nhật dữ liệu vào cơ sở dữ liệu
                UpdateDatSan(editedMaSanBong, editedMaKH, updatedLoaiThue, updatedDonGia, updatedBatDau, updatedKetThuc);

                // Làm mới DataSet
                ds_sb.Clear();
                da_sb.Fill(ds_sb, "DATSAN");

                // Cập nhật DataGridView ngay sau khi đã lưu
                RefreshDataGridView();

                // Phục hồi giao diện và trạng thái
                cboMaSan.Enabled = true;
                txtMaKH.Enabled = true;
                isEditMode = false;
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            DataTable dt = ds_sb.Tables["DATSAN"];
            DataRow newRow = dt.NewRow();
            newRow["MaSanBong"] = DBNull.Value;
            newRow["MaKH"] = DBNull.Value;
            newRow["LoaiThue"] = DBNull.Value;
            newRow["DonGia"] = DBNull.Value;
            newRow["BatDau"] = DBNull.Value;
            newRow["KetThuc"] = DBNull.Value;
            dt.Rows.Add(newRow);

            // Làm mới ComboBox
            cboMaSan.DataSource = null;
            cboMaSan.DataSource = ds_sb.Tables["DATSAN"];
            cboMaSan.DisplayMember = "MaSanBong";

            // Chọn dòng mới thêm vào
            int lastIndex = cboMaSan.Items.Count - 1;
            cboMaSan.SelectedIndex = lastIndex;

            // Hiển thị giá trị mặc định cho DateTimePicker
            dateTimePickerBatDau.Value = DateTime.Now;
            dateTimePickerKetThuc.Value = DateTime.Now;
            dateTimePickerGioBatDau.Value = DateTime.Now;
            dateTimePickerGioKetThuc.Value = DateTime.Now;
        }

        private void btnHuySan_Click(object sender, EventArgs e)
        {
            if (dataGridViewDatSan.SelectedRows.Count > 0)
            {

                DataGridViewRow selectedRow = dataGridViewDatSan.SelectedRows[0];


                string maSanBong = selectedRow.Cells["MaSanBong"].Value.ToString();
                DateTime batDau = (DateTime)selectedRow.Cells["BatDau"].Value;
                DateTime ketThuc = (DateTime)selectedRow.Cells["KetThuc"].Value;

                // Check if the reservation is within a valid cancellation period (if needed)

                // Implement the cancellation logic
                CancelDatSan(maSanBong, batDau, ketThuc);

                // Remove the row from the DataGridView
                dataGridViewDatSan.Rows.Remove(selectedRow);
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một dòng để hủy đặt sân.");
            }
        }

       
    }
}
