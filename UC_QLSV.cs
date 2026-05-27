using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLSV
{
    public partial class UC_QLSV : UserControl
    {
        databaseDataContext db = new databaseDataContext();
        public UC_QLSV()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void UC_QLSV_Load(object sender, EventArgs e)
        {
            LoadData();
            LoadDSLH4CBX();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //string mSSV = txtMSSV.Text;
            //string hoTen = txtHoTen.Text;
            //string gioiTinh = cboGioiTinh.Text;
            //string ngaySinh = dtpNgaySinh.Text;
            tbl_sinhvien sinhvien = new tbl_sinhvien();
            sinhvien.id = txtMSSV.Text;
            sinhvien.hoten = txtHoTen.Text;
            sinhvien.gioitinh = cboGioiTinh.Text;
            sinhvien.ngaysinh = DateTime.Parse(dtpNgaySinh.Text);
            try
            {
                db.tbl_sinhviens.InsertOnSubmit(sinhvien);
                db.SubmitChanges();
                MessageBox.Show("Thêm sinh viên thành công!");  
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            db.tbl_sinhviens.InsertOnSubmit(sinhvien);
            db.SubmitChanges();
            LoadData();
        }
        public void LoadData()
        {
            List<tbl_sinhvien> dSSV = db.tbl_sinhviens.ToList();
            dgv_DSSV.DataSource = dSSV;
        }

        public void LoadDSLH4CBX() // Load dữ liệu cho comboBox
        {
            List<tbl_lophoc> dSLH = db.tbl_lophocs.ToList();
            //cboMaLop.Text = "68PM12";
            cboMaLop.DataSource = dSLH;
            cboMaLop.DisplayMember = "tenLop"; // Hiển thị tên lớp
            cboMaLop.ValueMember = "malop"; // Giá trị thực tế là id của lớp
        }
    }
}
