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
    public partial class Approve2 : Form
    {
        public string LoggedInUsername { get; set; }
        public int LoggedInuserId { get; set; }

        public string connectionString = "data source=LAPTOP-EA0M4NKO\\SQLEXPRESS; database=Home; integrated security=SSPI";

        public Approve2()
        {
            InitializeComponent();
        }

        private void Approve2_Load(object sender, EventArgs e)
        {
            LoadPendingRequests();
        }

        private void LoadPendingRequests()
        {
            string query = @"SELECT b.BookingID,b.TenantID,t.FullName AS TenantName,b.FlatID,f.Location,b.RoomID,r.RoomNumber,r.RentAmount AS RoomRent,b.BookingDate,b.Status
                             FROM Bookings b INNER JOIN Tenants t ON b.TenantID = t.TenantID INNER JOIN Flats f   ON b.FlatID = f.FlatID INNER JOIN Rooms r   ON b.RoomID = r.RoomID INNER JOIN BachelorFlatManager m ON f.FlatID = m.FlatID
                              WHERE b.Status = 'Pending' AND b.RoomID IS NOT NULL AND m.UserID = @UserID ORDER BY b.BookingDate DESC;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@UserID", LoggedInuserId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dataGridView1.DataSource = dt;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Please select a booking row first.","Approve Booking", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            object bVal = dataGridView1.CurrentRow.Cells["BookingID"].Value;
            object rVal = dataGridView1.CurrentRow.Cells["RoomID"].Value;

            if (bVal == null || rVal == null ||
                !int.TryParse(bVal.ToString(), out int bookingId) ||
                !int.TryParse(rVal.ToString(), out int roomId))
            {
                MessageBox.Show("Invalid BookingID or RoomID.", "Approve Booking", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlTransaction tx = conn.BeginTransaction();

                string updateBooking = "UPDATE Bookings SET Status = 'Occupied' WHERE BookingID = @BID";
                using (var cmd = new SqlCommand(updateBooking, conn, tx))
                {
                    cmd.Parameters.AddWithValue("@BID", bookingId);
                    cmd.ExecuteNonQuery();
                }

                string updateRoom = "UPDATE Rooms SET Status = 'Occupied' WHERE RoomID = @RID";
                using (var cmd = new SqlCommand(updateRoom, conn, tx))
                {
                    cmd.Parameters.AddWithValue("@RID", roomId);
                    cmd.ExecuteNonQuery();
                }

                tx.Commit();
            }

            MessageBox.Show("Booking approved. Room status updated to Occupied.", "Approve Booking", MessageBoxButtons.OK, MessageBoxIcon.Information);

            LoadPendingRequests();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ManagerDashboard m1=new ManagerDashboard();
            m1.LoggedInuserId = this.LoggedInuserId;
            m1.LoggedInUsername=this.LoggedInUsername;
            m1.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
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

        private void profileBackButton_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();
        }
    }
}
