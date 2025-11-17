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
    public partial class FlatPaymentsForm : Form
    {
        public string cs = "data source=LAPTOP-EA0M4NKO\\SQLEXPRESS; database=Home; integrated security=SSPI";
        public int LoggedInuserId { get; set; }
        public string LoggedInUsername { get; set; }
        public string LoggedInRole { get; set; }

        public FlatPaymentsForm() { InitializeComponent(); }

        private void FlatPaymentsForm_Load(object sender, EventArgs e)
        {
            if (string.Equals(LoggedInRole, "Landlord", StringComparison.OrdinalIgnoreCase))
                LoadForLandlord();
            else if (string.Equals(LoggedInRole, "Manager", StringComparison.OrdinalIgnoreCase))
                LoadForManager();
            else
                Close();
        }

        private void LoadForLandlord()
        {
            int landlordId;
            using (SqlConnection conn = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand("SELECT LandlordID FROM Landlords WHERE UserID=@UID", conn))
            {
                cmd.Parameters.AddWithValue("@UID", LoggedInuserId);
                conn.Open();
                object o = cmd.ExecuteScalar();
                if (o == null || o == DBNull.Value) { Close(); return; }
                landlordId = (int)o;
            }

            string q = @"SELECT p.PaymentID, p.PaymentDate, p.AmountPaid,  p.PaymentMethod, p.TenantID, t.FullName AS TenantName,  p.FlatID, f.Location, p.RoomID, r.RoomNumber
                        FROM RentPayments p JOIN Flats f ON f.FlatID = p.FlatID JOIN Tenants t ON t.TenantID = p.TenantID LEFT JOIN Rooms r ON r.RoomID = p.RoomID
                         WHERE f.LandlordID = @LID ORDER BY p.PaymentDate DESC;";

            using (SqlConnection conn = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand(q, conn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("@LID", landlordId);
                var dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
            }
        }

        private void LoadForManager()
        {
            string q = @"SELECT p.PaymentID,p.PaymentDate,p.AmountPaid, p.PaymentMethod,p.TenantID,t.FullName AS TenantName, p.FlatID,f.Location, p.RoomID, r.RoomNumber
                        FROM RentPayments p JOIN Flats f ON f.FlatID = p.FlatID JOIN Tenants t ON t.TenantID = p.TenantID LEFT JOIN Rooms r ON r.RoomID = p.RoomID JOIN BachelorFlatManager m ON m.FlatID = f.FlatID
                        WHERE m.UserID = @UID ORDER BY p.PaymentDate DESC;";

            using (SqlConnection conn = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand(q, conn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("@UID", LoggedInuserId);
                var dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
            }
        }

        private void button2_Click(object sender, EventArgs e)
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

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();

            if (this.LoggedInRole == "Manager")
            {
                ManagerDashboard manager = new ManagerDashboard();
                
                manager.LoggedInuserId = this.LoggedInuserId;
                manager.LoggedInUsername = this.LoggedInUsername;
                manager.Show();
            }
            else if (this.LoggedInRole == "Landlord")
            {
                LandlordDashboard landlord = new LandlordDashboard();
               
                landlord.LoggedInuserId = this.LoggedInuserId;
                landlord.LoggedInUsername = this.LoggedInUsername;
                landlord.Show();
            }
            else
            {
                MessageBox.Show("Unknown role. Redirecting to login.");
                Form1 login = new Form1();
                login.Show();
            }

            this.Close();
        }
    }
}
