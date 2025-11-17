using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rent_dibo
{
    public partial class MyPaymentsForm : Form
    {
        public string LoggedInUsername { get; set; }
        
        public  string cs = "data source=LAPTOP-EA0M4NKO\\SQLEXPRESS; database=Home; integrated security=SSPI";
        public int LoggedInuserId { get; set; }
        private int tenantId;

        public MyPaymentsForm() { InitializeComponent(); }

        private void MyPaymentsForm_Load(object sender, EventArgs e)
        {
            using (var conn = new SqlConnection(cs))
            using (var cmd = new SqlCommand("SELECT TenantID FROM Tenants WHERE UserID=@UID", conn))
            {
                cmd.Parameters.AddWithValue("@UID", LoggedInuserId);
                conn.Open();
                object t = cmd.ExecuteScalar();
                if (t == null || t == DBNull.Value) { Close(); return; }
                tenantId = (int)t;
            }

            string q = @"SELECT  p.PaymentID, p.PaymentDate, p.AmountPaid, p.PaymentMethod, p.FlatID, f.Location, p.RoomID, r.RoomNumber
                      FROM RentPayments p JOIN Flats f ON f.FlatID = p.FlatID LEFT JOIN Rooms r ON r.RoomID = p.RoomID
                      WHERE p.TenantID = @TID ORDER BY p.PaymentDate DESC;";

            using (SqlConnection conn = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand(q, conn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("@TID", tenantId);
                var dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TenantDashboard ten = new TenantDashboard();
            ten.LoggedInuserId = LoggedInuserId;
            ten.LoggedInUsername = LoggedInUsername;
            ten.Show();
            this.Hide();
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            //ext
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
    }
}
