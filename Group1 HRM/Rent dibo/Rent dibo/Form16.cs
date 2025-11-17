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
    public partial class PaymentForm : Form
    {
       public string cs = "data source=LAPTOP-EA0M4NKO\\SQLEXPRESS; database=Home; integrated security=SSPI";

        public int LoggedInuserId { get; set; }
        private int tenantId;
        public string LoggedInUsername { get; set; }

        public PaymentForm() { InitializeComponent(); }

        private void PaymentForm_Load(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand("SELECT TenantID FROM Tenants WHERE UserID=@UID", conn))
            {
                cmd.Parameters.AddWithValue("@UID", LoggedInuserId);
                conn.Open();
                object t = cmd.ExecuteScalar();
                if (t == null || t == DBNull.Value)
                {
                    MessageBox.Show("No tenant profile found."); Close(); return;
                }
                tenantId = (int)t;
            }

            string q = @"SELECT b.BookingID,b.FlatID,b.RoomID, ISNULL(r.RoomNumber,'Family') AS RoomLabel,COALESCE(r.RentAmount, f.RentAmount) AS DueRent,
                     f.Location FROM Bookings b JOIN Flats f ON f.FlatID = b.FlatID LEFT JOIN Rooms r ON r.RoomID = b.RoomID
                     WHERE b.TenantID = @TID AND b.Status IN ('Approved','Occupied') ORDER BY b.BookingDate DESC;";

            using (SqlConnection conn = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand(q, conn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("@TID", tenantId);
                var dt = new DataTable();
                da.Fill(dt);

                dt.Columns.Add("Display", typeof(string),
                   "'BID:' + Convert(BookingID, 'System.String') + " +
                   "' | Flat:' + Convert(FlatID, 'System.String') + " +
                   "' | ' + RoomLabel + " +
                   "' | Rent:' + Convert(DueRent, 'System.String') + " +
                   "' | ' + Location");

                comboBookings.DisplayMember = "Display";
                comboBookings.ValueMember = "BookingID";
                comboBookings.DataSource = dt;
            }

            comboMethod.Items.Clear();
            comboMethod.Items.Add("Cash");
            comboMethod.Items.Add("Bank");
            comboMethod.Items.Add("Mobile");
            comboMethod.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBookings.SelectedItem == null)
            {
                MessageBox.Show("Select a booking."); return;
            }

            if (!decimal.TryParse(txtAmount.Text.Trim(), out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Enter a valid amount."); return;
            }

            string method = comboMethod.SelectedItem?.ToString();
            if (string.IsNullOrWhiteSpace(method))
            {
                MessageBox.Show("Select a method."); return;
            }

            var drv = (DataRowView)comboBookings.SelectedItem;
            int flatId = Convert.ToInt32(drv["FlatID"]);
            object roomObj = drv["RoomID"];
            object roomParam = (roomObj == DBNull.Value) ? (object)DBNull.Value : Convert.ToInt32(roomObj);

            string insert = @"INSERT INTO RentPayments (TenantID, FlatID, RoomID, AmountPaid, PaymentDate, PaymentMethod)VALUES (@TID, @FID, @RID, @Amount, GETDATE(), @Method);";

            using (var conn = new SqlConnection(cs))
            using (var cmd = new SqlCommand(insert, conn))
            {
                cmd.Parameters.AddWithValue("@TID", tenantId);
                cmd.Parameters.AddWithValue("@FID", flatId);
                cmd.Parameters.AddWithValue("@RID", roomParam);
                cmd.Parameters.AddWithValue("@Amount", amount);
                cmd.Parameters.AddWithValue("@Method", method);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Payment recorded successfully.");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            TenantDashboard f3 = new TenantDashboard();
            f3.LoggedInUsername = this.LoggedInUsername;
            f3.LoggedInuserId=this.LoggedInuserId;
            f3.Show();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to exit the application?", "Confirm Exit", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}
