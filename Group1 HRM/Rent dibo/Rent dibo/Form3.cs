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
    public partial class TenantDashboard : Form
    {
        string connectionString = "data source=LAPTOP-EA0M4NKO\\SQLEXPRESS; database=Home; integrated security=SSPI";
        public string LoggedInUsername { get; set; }
        public int LoggedInuserId { get; set; }

        public TenantDashboard()
        {
            InitializeComponent();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void button7_Click(object sender, EventArgs e)
        {
            // Confirm exit
            DialogResult result = MessageBox.Show("Are you sure you want to exit the application?","Confirm Exit",MessageBoxButtons.YesNo,MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            
            Form1 f1 = new Form1();
            f1.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            var f4 = new Form4(LoggedInUsername);
            f4.LoggedInUsername =this.LoggedInUsername;
            f4.LoggedInuserId = LoggedInuserId;   
            f4.StartPosition = FormStartPosition.CenterParent;
            f4.Show();               

        }

        private void button3_Click(object sender, EventArgs e)
        {
           
            Form5 f5 = new Form5();
            f5.LoggedInUsername = LoggedInUsername;
            f5.LoggedInuserId = LoggedInuserId;
            f5.Show();
            this.Hide();
        }

        private void TenantDashboard_Load(object sender, EventArgs e)
        {

        }
        private void button5_Click(object sender, EventArgs e)
        {
            var payment1 = new PaymentForm();
            payment1.LoggedInUsername = LoggedInUsername; 
            payment1.LoggedInuserId = LoggedInuserId; 
            payment1.Show();
            this.Hide();
        }
        private void button4_Click(object sender, EventArgs e)
        {
            Form5 f5 = new Form5();
            f5.LoggedInUsername = LoggedInUsername;
            f5.LoggedInuserId = LoggedInuserId;
            f5.Show();
            this.Hide();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            var mypay =new MyPaymentsForm();
            mypay.LoggedInuserId=this.LoggedInuserId;
            mypay.LoggedInUsername = this.LoggedInUsername;
            mypay.Show();
            this.Hide();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Notice notice = new Notice();
            notice.Show();
        }
    }
}
