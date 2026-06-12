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
        int pageNumber = 1;      // Trang hiện tại
        int pageSize = 10;       // Số sinh viên trên mỗi trang
        int totalRecords = 0;    // Tổng số sinh viên trong DB
        int totalPages = 1;      // Tổng số trang
        string targetSearch = ""; // Lưu từ khóa tìm kiếm hiện tại
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
            //db.tbl_sinhviens.InsertOnSubmit(sinhvien);
            //db.SubmitChanges();
            //LoadData();
        }
        public void LoadData()
        {
            // 1. Tạo câu truy vấn cơ bản từ bảng sinh viên
            var query = db.tbl_sinhviens.AsQueryable();

            // Nếu có từ khóa tìm kiếm, thực hiện lọc theo ID hoặc Họ Tên
            if (!string.IsNullOrEmpty(targetSearch))
            {
                query = query.Where(sv => sv.id.Contains(targetSearch) ||
                                          sv.hoten.Contains(targetSearch));
            }

            // 2. Tính tổng số bản ghi và số trang dựa trên kết quả đã lọc
            totalRecords = query.Count();
            totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            if (totalPages == 0) totalPages = 1;

            // Kiểm tra ràng buộc trang
            if (pageNumber > totalPages) pageNumber = totalPages;
            if (pageNumber < 1) pageNumber = 1;

            // 3. Phân trang trên kết quả (đã lọc hoặc chưa lọc)
            var dSSV = query.Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

            dgv_DSSV.DataSource = dSSV;

            // 4. Hiển thị thông tin lên 2 Label của bạn
            lblPageInfo.Text = $"Trang {pageNumber}/{totalPages}";
            lblSum.Text = $"{totalRecords} Bản ghi";

            // 5. Bật/Tắt các nút điều hướng
            btnFirst.Enabled = pageNumber > 1;
            btnPrev.Enabled = pageNumber > 1;
            btnNext.Enabled = pageNumber < totalPages;
            btnLast.Enabled = pageNumber < totalPages;
        }

        public void LoadDSLH4CBX() // Load dữ liệu cho comboBox
        {
            List<tbl_lophoc> dSLH = db.tbl_lophocs.ToList();
            //cboMaLop.Text = "68PM12";
            cboMaLop.DataSource = dSLH;
            cboMaLop.DisplayMember = "tenLop"; // Hiển thị tên lớp
            cboMaLop.ValueMember = "malop"; // Giá trị thực tế là id của lớp
        }

        private void dtpNgaySinh_ValueChanged(object sender, EventArgs e)
        {
            
        }

        private void btnFirst_Click(object sender, EventArgs e)
        {
            pageNumber = 1;
            LoadData();
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (pageNumber > 1)
            {
                pageNumber--;
                LoadData();
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (pageNumber < totalPages)
            {
                pageNumber++;
                LoadData();
            }
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            pageNumber = totalPages;
            LoadData();
        }

        private void dgv_DSSV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra xem người dùng có click vào dòng hợp lệ hay không (tránh click vào Header dòng)
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgv_DSSV.Rows[e.RowIndex];

                // Đổ dữ liệu từ các cột của DataGridView vào TextBox/ComboBox tương ứng
                txtMSSV.Text = row.Cells["id"].Value?.ToString();
                txtHoTen.Text = row.Cells["hoten"].Value?.ToString();
                cboGioiTinh.Text = row.Cells["gioitinh"].Value?.ToString();

                // Xử lý Ngày sinh (Đề phòng dữ liệu ngày bị null)
                if (row.Cells["ngaysinh"].Value != null)
                {
                    dtpNgaySinh.Value = Convert.ToDateTime(row.Cells["ngaysinh"].Value);
                }

                // Xử lý Mã lớp (Sử dụng SelectedValue vì ComboBox của bạn đã được gán ValueMember = "malop")
                if (row.Cells["malop"].Value != null)
                {
                    cboMaLop.SelectedValue = row.Cells["malop"].Value.ToString();
                }

                // Khóa textbox Mã số sinh viên lại không cho sửa (vì ID thường là Primary Key cố định)
                txtMSSV.ReadOnly = true;
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            string maSV = txtMSSV.Text;

            // Tìm sinh viên cần sửa trong Database bằng LINQ
            tbl_sinhvien sinhvien = db.tbl_sinhviens.SingleOrDefault(sv => sv.id == maSV);

            if (sinhvien != null)
            {
                try
                {
                    // Cập nhật các thông tin mới từ Form nhập liệu
                    sinhvien.hoten = txtHoTen.Text;
                    sinhvien.gioitinh = cboGioiTinh.Text;
                    sinhvien.ngaysinh = dtpNgaySinh.Value;
                    sinhvien.malop = cboMaLop.SelectedValue?.ToString();

                    // Lưu thay đổi vào database
                    db.SubmitChanges();
                    MessageBox.Show("Cập nhật thông tin sinh viên thành công!");

                    LoadData(); // Tải lại bảng dữ liệu
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Có lỗi xảy ra: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Không tìm thấy sinh viên cần sửa hoặc mã SV không hợp lệ.");
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            // Xóa trắng các TextBox
            txtMSSV.Text = "";
            txtHoTen.Text = "";

            // Mở khóa lại ô Mã số sinh viên để chuẩn bị cho việc thêm mới nếu cần
            txtMSSV.ReadOnly = false;

            // Đặt lại ComboBox Giới tính và Ngày sinh về mặc định
            if (cboGioiTinh.Items.Count > 0) cboGioiTinh.SelectedIndex = 0;
            dtpNgaySinh.Value = DateTime.Now;

            // Đặt ComboBox Mã lớp về dòng đầu tiên (nếu có dữ liệu)
            if (cboMaLop.Items.Count > 0)
            {
                cboMaLop.SelectedIndex = 0;
            }
            else
            {
                cboMaLop.Text = "";
            }

            // Xóa trắng ô tìm kiếm và đặt lại biến tìm kiếm về rỗng
            txtTimKiem.Text = "";
            targetSearch = "";

            pageNumber = 1; // Quay về trang đầu
            LoadData();     // Tải lại toàn bộ danh sách sinh viên ban đầu
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            string maSV = txtMSSV.Text.Trim();

            // 1. Kiểm tra xem người dùng đã chọn sinh viên nào chưa
            if (string.IsNullOrEmpty(maSV))
            {
                MessageBox.Show("Vui lòng chọn một sinh viên từ danh sách để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Hiện hộp thoại xác nhận trước khi xóa để tránh người dùng ấn nhầm
            DialogResult dialogResult = MessageBox.Show($"Bạn có chắc chắn muốn xóa sinh viên có mã {maSV} không?",
                                                        "Xác nhận xóa",
                                                        MessageBoxButtons.YesNo,
                                                        MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                // 3. Tìm sinh viên cần xóa trong Database bằng LINQ
                tbl_sinhvien sinhvien = db.tbl_sinhviens.SingleOrDefault(sv => sv.id == maSV);

                if (sinhvien != null)
                {
                    try
                    {
                        // Xóa đối tượng khỏi bảng và lưu thay đổi
                        db.tbl_sinhviens.DeleteOnSubmit(sinhvien);
                        db.SubmitChanges();

                        MessageBox.Show("Xóa sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // 4. Làm mới vùng nhập liệu và tải lại dữ liệu phân trang mới
                        btnLamMoi_Click(sender, e);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Có lỗi xảy ra khi xóa: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Không tìm thấy sinh viên cần xóa trong hệ thống.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            targetSearch = txtTimKiem.Text.Trim();

            pageNumber = 1;

            LoadData();
        }
    }
}
