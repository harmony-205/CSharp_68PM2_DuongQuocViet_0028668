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
    public partial class UC_QLLH : UserControl
    {
        databaseDataContext db = new databaseDataContext(); 
        public UC_QLLH()
        {
            InitializeComponent();
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dgv_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void UC_QLLH_Load(object sender, EventArgs e)
        {
            List<tbl_lophoc> dSSV = db.tbl_lophocs.ToList();
            dgv_DSSV.DataSource = dSSV;
        }
    }
}
