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
    public partial class FlatDetailsForm : Form
    {
        public string LoggedInUsername { get; set; }
        public int LoggedInuserId { get; set; }
        private readonly int _flatId;
        private readonly string connectionString =
            "data source=LAPTOP-EA0M4NKO\\SQLEXPRESS; database=Home; integrated security=SSPI";

        public FlatDetailsForm(int flatId)
        {
            InitializeComponent();
            _flatId = flatId;
        }

        private void FlatDetailsForm_Load(object sender, EventArgs e)
        {
            LoadFlatDetails();
        }

        private void LoadFlatDetails()
        {
            string query = @"
        SELECT FlatID, LandlordID, Location, RentAmount, FlatType, Status, BachelorCategory
        FROM Flats
        WHERE FlatID = @FlatID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@FlatID", _flatId);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        FlatIdtextBox1.Text = reader["FlatID"].ToString();
                        LandlordIdtextBox2.Text = reader["LandlordID"].ToString();
                        LocationtextBox3.Text = reader["Location"].ToString();
                        RentAmounttextBox4.Text = reader["RentAmount"].ToString();
                        FlatTypetextBox5.Text = reader["FlatType"].ToString();
                        StatustextBox6.Text = reader["Status"].ToString();
                        BachelorCategorytextBox7.Text = reader["BachelorCategory"].ToString();
                    }
                    else
                    {
                        MessageBox.Show("No details found for this flat.", "Error",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Close();
                    }
                }
            }
        }

        public FlatDetailsForm()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form5 f5= new Form5();
            f5.LoggedInUsername = this.LoggedInUsername;
            f5.LoggedInuserId=this.LoggedInuserId;
            f5.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FlatIdtextBox1.Text))
            {
                MessageBox.Show("No Flat selected.", "Book Flat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!int.TryParse(FlatIdtextBox1.Text, out int flatId))
            {
                MessageBox.Show("Invalid FlatID.", "Book Flat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string flatType = FlatTypetextBox5.Text?.Trim();
            if (string.IsNullOrWhiteSpace(flatType))
            {
                MessageBox.Show("fllat type is missing.", "Book Flat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            
            if (flatType.Equals("Bachelor", StringComparison.OrdinalIgnoreCase))
            {
                var roomForm = new RoomBookForms(flatId);
                roomForm.LoggedInUsername = this.LoggedInUsername;
                roomForm.LoggedInuserId = this.LoggedInuserId;
                roomForm.Show();
                this.Hide();

            }

            
            int tenantId = 0;
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (var getTenant = new SqlCommand(
                    "SELECT TenantID FROM Tenants WHERE UserID = @UserID", conn))
                {
                    getTenant.Parameters.AddWithValue("@UserID", LoggedInuserId);
                    object t = getTenant.ExecuteScalar();
                    if (t == null || t == DBNull.Value)
                    {
                        MessageBox.Show("Your tenant profile was not found. Complete your tenant profile before booking.",
                                        "Book Flat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    tenantId = (int)t;
                }

               
                using (var dup = new SqlCommand(
                    "SELECT COUNT(*) FROM Bookings WHERE TenantID=@TID AND FlatID=@FID AND Status IN ('Pending','Approved')", conn))
                {
                    dup.Parameters.AddWithValue("@TID", tenantId);
                    dup.Parameters.AddWithValue("@FID", flatId);
                    int exists = (int)dup.ExecuteScalar();
                    if (exists > 0)
                    {
                        MessageBox.Show("You already have a booking for this flat (Pending/Approved).",
                                        "Book Flat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }

               
                using (var ins = new SqlCommand(
                    "INSERT INTO Bookings (TenantID, FlatID, BookingDate, Status) VALUES (@TID, @FID, GETDATE(), 'Pending')", conn))
                {
                    ins.Parameters.AddWithValue("@TID", tenantId);
                    ins.Parameters.AddWithValue("@FID", flatId);
                    int rows = ins.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        MessageBox.Show("Booking request submitted (Pending).",
                                        "Book Flat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Booking failed. Please try again.",
                                        "Book Flat", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Form1 f1=new Form1();
            f1.Show();
            this.Hide();
        }

        private void button5_Click(object sender, EventArgs e)
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
