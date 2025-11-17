using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rent_dibo
{
    public partial class LandlordDashboard : Form
    {

        string connectionString = "data source=LAPTOP-EA0M4NKO\\SQLEXPRESS; database=Home; integrated security=SSPI";
        public string LoggedInUsername { get; set; }
        public int LoggedInuserId { get; set; }
        public LandlordDashboard()
        {
            InitializeComponent();
        }

        private void LandlordDashboard_Load(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            // Confirm exit
            DialogResult result = MessageBox.Show(
                "Are you sure you want to exit the application?",
                "Confirm Exit",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 f1 = new Form1();
            f1.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form7 form7 = new Form7(LoggedInUsername);
            form7.LoggedInUsername=this.LoggedInUsername;
            form7.LoggedInuserId = LoggedInuserId;
            form7.StartPosition = FormStartPosition.CenterParent;
            form7.ShowDialog(this);
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            RentPostForm rentpostf = new RentPostForm();
            rentpostf.LoggedInUsername=LoggedInUsername;
            rentpostf.LoggedInuserId=LoggedInuserId;
            rentpostf.ShowDialog(this);
            this.Hide();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Approve1 appr = new Approve1();
            appr.LoggedInUsername=LoggedInUsername;
            appr.LoggedInuserId=LoggedInuserId;
            appr.ShowDialog(this);
            this.Hide();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            AssignManagerForm assignManagerForm = new AssignManagerForm();
            assignManagerForm.LoggedInuserId=this.LoggedInuserId;
            assignManagerForm.ShowDialog(this);
            this.Hide();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Notice notice = new Notice();
            notice.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            FlatPaymentsForm form = new FlatPaymentsForm();
            form.LoggedInuserId = this.LoggedInuserId;
            form.LoggedInRole = "Landlord"; 
            form.LoggedInUsername=this.LoggedInUsername;
            form.Show();
            this.Hide();
        }
    }
}
