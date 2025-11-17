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
    public partial class AssignManagerForm : Form
    {
      public string cs = "data source=LAPTOP-EA0M4NKO\\SQLEXPRESS; database=Home; integrated security=SSPI";

        public int LoggedInuserId { get; set; }
        public string LoggedInUsername { get; set; }

        public AssignManagerForm()
        {
            InitializeComponent();
        }

        private void AssignManagerForm_Load(object sender, EventArgs e)
        {
            LoadManagers();
            LoadFlats();
        }

        private void LoadManagers()
        {
            string q = "SELECT UserID, Username FROM Users WHERE Role = 'Manager'";

            using (SqlConnection conn = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand(q, conn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                var dt = new DataTable();
                da.Fill(dt);

                comboManagers.DisplayMember = "Username";
                comboManagers.ValueMember = "UserID";
                comboManagers.DataSource = dt;
            }
        }

        private void LoadFlats()
        {
            string q = "SELECT FlatID, Location FROM Flats WHERE LandlordID = (SELECT LandlordID FROM Landlords WHERE UserID = @UID)";

            using (SqlConnection conn = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand(q, conn))
            {
                cmd.Parameters.AddWithValue("@UID", LoggedInuserId);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);

                    comboFlats.DisplayMember = "Location";
                    comboFlats.ValueMember = "FlatID";
                    comboFlats.DataSource = dt;
                }
            }
        }

        private void btnAssign_Click(object sender, EventArgs e)
        {
            if (comboManagers.SelectedValue == null || comboFlats.SelectedValue == null)
            {
                MessageBox.Show("Select both a Manager and a Flat."); return;
            }

            int managerUserId = Convert.ToInt32(comboManagers.SelectedValue);
            int flatId = Convert.ToInt32(comboFlats.SelectedValue);
            string managerName = comboManagers.Text;

            string insert = @"INSERT INTO BachelorFlatManager (UserID, FlatID, ManagerName, AssignedDate) VALUES (@UserID, @FlatID, @ManagerName, GETDATE());";

            using (SqlConnection conn = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand(insert, conn))
            {
                cmd.Parameters.AddWithValue("@UserID", managerUserId);
                cmd.Parameters.AddWithValue("@FlatID", flatId);
                cmd.Parameters.AddWithValue("@ManagerName", managerName);

                conn.Open();
                int rows = cmd.ExecuteNonQuery();

                if (rows > 0)
                {
                    MessageBox.Show("Successful", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to assign manager.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LandlordDashboard ld2 = new LandlordDashboard();
            ld2.LoggedInuserId = this.LoggedInuserId;
            ld2.LoggedInUsername = this.LoggedInUsername;
            ld2.Show();
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 form1 = new Form1();
            form1.Show();
        }
    }
}

