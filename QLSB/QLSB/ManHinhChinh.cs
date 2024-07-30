using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLSB
{
    public partial class ManHinhChinh : Form
    {
        private string username;
        public ManHinhChinh()
        {
            InitializeComponent();
        }
        private Form currentFormChild;
        private void openChildForm(Form childForm)
        {
            if (currentFormChild != null)
            {
                currentFormChild.Close();
            }
            currentFormChild = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            panel_body.Controls.Add(childForm);
            panel_body.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
        }
        private void button1_Click(object sender, EventArgs e)
        {
        }
        private void btn_DatSan_Click(object sender, EventArgs e)
        {
            openChildForm(new DatSan());
        }

        private void btn_QLSB_Click(object sender, EventArgs e)
        {
            openChildForm(new QuanLySan());
        }

        private void label2_Click(object sender, EventArgs e)
        {
            DialogResult r = MessageBox.Show("Bạn có muốn thoát", "Thoát", MessageBoxButtons.YesNo);
            if(r == DialogResult.Yes)
            {
                this.Close();
            }
            
        }

        private void btn_dichvu_Click(object sender, EventArgs e)
        {

        }

        private void btn_HeThong_Click(object sender, EventArgs e)
        {
            openChildForm(new HeThong());
        }
        public void SetUsername(string username)
        {
            this.username = username;
            lblHello.Text = "Xin chào, " + username;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            openChildForm(new QuanLyDoanhThu());
        }

        private void panel_left_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel_body_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            
        }

        private void btn_exit_Click(object sender, EventArgs e)
        {
            DialogResult r = MessageBox.Show("Bạn có muốn thoát không", "Thoát", MessageBoxButtons.YesNo);
            if(r == DialogResult.Yes)
            {
                this.Hide();
                Login lg = new Login();
                lg.Show();
            }
        }
    }
}
