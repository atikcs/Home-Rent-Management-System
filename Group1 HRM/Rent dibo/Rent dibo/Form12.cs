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
    public partial class Approve1 : Form
    {

        public string LoggedInUsername { get; set; }
        public int LoggedInuserId { get; set; }

        private int landlordId;
        public  string cs = "data source=LAPTOP-EA0M4NKO\\SQLEXPRESS; database=Home; integrated security=SSPI";

        public Approve1()
        {
            InitializeComponent();
        }

        private void Approve1_Load(object sender, EventArgs e)
        {
            if (!TryResolveLandlordId())
            {
                MessageBox.Show("Could not show Landlord profile for this user.", "Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }

            LoadPendingRequests();
        }

        private bool TryResolveLandlordId()
        {
            using (SqlConnection conn = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand("SELECT LandlordID FROM Landlords WHERE UserID = @UID", conn))
            {
                cmd.Parameters.AddWithValue("@UID", LoggedInuserId);
                conn.Open();
                object o = cmd.ExecuteScalar();
                if (o == null || o == DBNull.Value) return false;
                landlordId = (int)o;
                return true;
            }
        }

        private void LoadPendingRequests()
        {
            string q = @" SELECT b.BookingID, t.FullName AS TenantName,b.TenantID,b.FlatID,f.Location,f.RentAmount,f.FlatType,b.BookingDate,b.Status
                          FROM dbo.Bookings AS b
                          JOIN dbo.Flats AS f ON f.FlatID = b.FlatID
                          JOIN dbo.Tenants AS t ON t.TenantID = b.TenantID
                          WHERE f.LandlordID = @LID
                          AND f.FlatType   = 'Family'
                          AND b.Status     = 'Pending'
                          ORDER BY b.BookingDate DESC;";

            using (SqlConnection conn = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand(q, conn))
            {
                cmd.Parameters.AddWithValue("@LID", landlordId);
                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Please select a booking row.", "Approve", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int bookingId = Convert.ToInt32(dataGridView1.CurrentRow.Cells["BookingID"].Value);
            int flatId = Convert.ToInt32(dataGridView1.CurrentRow.Cells["FlatID"].Value);

            string sqlUpdateFlat = "UPDATE Flats    SET Status = 'Occupied' WHERE FlatID = @FID;";
            string sqlUpdateBooking = "UPDATE Bookings SET Status = 'Occupied' WHERE BookingID = @BID;";

            using (var conn = new SqlConnection(cs))
            {
                conn.Open();

                using (var cmd1 = new SqlCommand(sqlUpdateFlat, conn))
                {
                    cmd1.Parameters.AddWithValue("@FID", flatId);
                    cmd1.ExecuteNonQuery();
                }

                using (var cmd2 = new SqlCommand(sqlUpdateBooking, conn))
                {
                    cmd2.Parameters.AddWithValue("@BID", bookingId);
                    cmd2.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Booking approved: Flat and booking set to 'Occupied'.","Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            LoadPendingRequests(); // refresh grid
        }

        private void button2_Click(object sender, EventArgs e)
        {
            LandlordDashboard ld2 = new LandlordDashboard();
            ld2.LoggedInuserId = this.LoggedInuserId;
            ld2.LoggedInUsername = this.LoggedInUsername;
            ld2.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
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

        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 form1 = new Form1();
            form1.Show();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}