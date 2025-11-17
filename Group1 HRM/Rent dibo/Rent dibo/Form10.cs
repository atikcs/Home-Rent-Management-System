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

    public partial class RoomBookForms : Form
    {
       
        private readonly int _flatId;

        
        private readonly string cs ="data source=LAPTOP-EA0M4NKO\\SQLEXPRESS; database=Home; integrated security=SSPI";

        public string LoggedInUsername { get; set; }
        public int LoggedInuserId { get; set; }

        public RoomBookForms(int flatId)
        {
            InitializeComponent();
            _flatId = flatId; 
            
            this.Load += RoomBookForms_Load;
        }

        private void RoomBookForms_Load(object sender, EventArgs e)
        {
            LoadRooms();
        }

        private void LoadRooms()
        {
            const string sql = @"SELECT RoomID, FlatID, RoomNumber, RentAmount, Status
                   FROM dbo.Rooms
                   WHERE FlatID = @FID
                   ORDER BY RoomNumber;";

            using (var conn = new SqlConnection(cs))
            using (var cmd = new SqlCommand(sql, conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("@FID", _flatId);

                var dt = new DataTable();
                da.Fill(dt);

                dataGridViewRooms.AutoGenerateColumns = true;  
                dataGridViewRooms.DataSource = dt;

                
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show($"No rooms found for FlatID = {_flatId}.",
                                    "RoomBookForms", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void bookthisRoom_Click(object sender, EventArgs e)
        {
            if (dataGridViewRooms.CurrentRow == null)
            {
                MessageBox.Show("Please select a room first.", "Book Room", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

         
            object val = dataGridViewRooms.CurrentRow.Cells["RoomID"].Value;
            if (val == null || val == DBNull.Value || !int.TryParse(val.ToString(), out int roomId))
            {
                MessageBox.Show("Invalid RoomID on the selected row.", "Book Room", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            
            int tenantId;
            string tenantGender = null;
            using (var conn = new SqlConnection(cs))
            using (var cmd = new SqlCommand("SELECT TenantID, Gender FROM Tenants WHERE UserID = @UID", conn))
            {
                cmd.Parameters.AddWithValue("@UID", LoggedInuserId);
                conn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    if (!r.Read())
                    {
                        MessageBox.Show("No tenant profile found for this user. Complete your tenant profile first.",
                                        "Book Room", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    tenantId = (int)r["TenantID"];
                    tenantGender = r["Gender"]?.ToString();
                }
            }

            
            string flatType = null, bachelorCategory = null;
            using (var conn = new SqlConnection(cs))
            using (var cmd = new SqlCommand("SELECT FlatType, BachelorCategory FROM Flats WHERE FlatID = @FID", conn))
            {
                cmd.Parameters.AddWithValue("@FID", _flatId);
                conn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        flatType = r["FlatType"].ToString();
                        bachelorCategory = r["BachelorCategory"]?.ToString();
                    }
                }
            }

            if (string.Equals(flatType, "Bachelor", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.Equals(tenantGender, bachelorCategory, StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show(
                        $"This Bachelor flat is restricted to {bachelorCategory} tenants only.",
                        "Booking Restricted", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            using (var conn = new SqlConnection(cs))
            {
                conn.Open();
                SqlTransaction tx = conn.BeginTransaction();

                
                const string updateSql = @"UPDATE Rooms
               SET Status = 'Pending'
               WHERE RoomID = @RoomID AND Status = 'Vacant';";

                int rows;
                using (var cmd = new SqlCommand(updateSql, conn, tx))
                {
                    cmd.Parameters.AddWithValue("@RoomID", roomId);
                    rows = cmd.ExecuteNonQuery();
                }

                if (rows == 0)
                {
                    MessageBox.Show("This room is not available (already Pending/Occupied).",
                                    "Book Room", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    tx.Rollback();
                    return;
                }

                
                const string insertSql = @"INSERT INTO Bookings (TenantID, FlatID, RoomID, BookingDate, Status)
                  VALUES (@TID, @FID, @RID, GETDATE(), 'Pending');";

                using (var cmd = new SqlCommand(insertSql, conn, tx))
                {
                    cmd.Parameters.AddWithValue("@TID", tenantId);
                    cmd.Parameters.AddWithValue("@FID", _flatId);
                    cmd.Parameters.AddWithValue("@RID", roomId);
                    cmd.ExecuteNonQuery();
                }

                tx.Commit();

                MessageBox.Show("Room booking request submitted Successfully ..please wait for confirm from manager.",
                                "Book Room", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

           
           
        }

        private void dataGridViewRooms_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return; 

            DataGridViewRow row = dataGridViewRooms.Rows[e.RowIndex];

            int roomId = Convert.ToInt32(row.Cells["RoomID"].Value);
            int flatId = Convert.ToInt32(row.Cells["FlatID"].Value);
            string roomNumber = row.Cells["RoomNumber"].Value.ToString();
            string rentAmount = row.Cells["RentAmount"].Value.ToString();
            string status = row.Cells["Status"].Value.ToString();

           
            var detailsForm = new RoomBookDetails(roomId, flatId, roomNumber, rentAmount, status);
            detailsForm.Show(this);
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
           FlatDetailsForm form = new FlatDetailsForm();
            form.LoggedInuserId=this.LoggedInuserId;
            form.LoggedInUsername=this.LoggedInUsername;
            form.Show();
            this.Hide();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Form1 form = new Form1();
            form.Show();
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

        private void RoomBookForms_Load_1(object sender, EventArgs e)
        {

        }
    }

}