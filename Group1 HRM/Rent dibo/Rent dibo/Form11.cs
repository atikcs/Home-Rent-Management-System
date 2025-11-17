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
    public partial class RentPostForm : Form
    {
        public string LoggedInUsername { get; set; }
        public int LoggedInuserId { get; set; }
        public string cs = "data source=LAPTOP-EA0M4NKO\\SQLEXPRESS; database=Home; integrated security=SSPI";

     
        private int landlordId;

        public RentPostForm()
        {
            InitializeComponent();

        }

        private void RentPostForm_Load(object sender, EventArgs e)
        {
            ResolveLandlordId();

            textBox1.ReadOnly = true;

            textBox2.Text = landlordId.ToString();
            textBox2.ReadOnly = true;

            comboBox1.Items.Clear();
            comboBox1.Items.Add("Family");
            comboBox1.Items.Add("Bachelor");
            comboBox1.SelectedIndex = 0;

            radioButton1.Checked = false;
            radioButton2.Checked = false;

           
            textBox4.ReadOnly = true;
            textBox4.Text = "Vacant";

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void ResolveLandlordId()
        {
            using (var conn = new SqlConnection(cs))
            using (var cmd = new SqlCommand("SELECT LandlordID FROM Landlords WHERE UserID=@UID", conn))
            {
                cmd.Parameters.AddWithValue("@UID", LoggedInuserId);
                conn.Open();
                object o = cmd.ExecuteScalar();
                if (o == null || o == DBNull.Value)
                {
                    MessageBox.Show("No landlord profile found for this user.","Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                    return;
                }
                landlordId = (int)o;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string location = richTextBox1.Text.Trim();
            if (string.IsNullOrWhiteSpace(location))
            {
                MessageBox.Show("Please enter a location."); 
                return;
            }

            if (!decimal.TryParse(textBox3.Text.Trim(), out decimal rentAmount) || rentAmount <= 0)
            {
                MessageBox.Show("Invalid rent amount."); 
                return;
            }

            string flatType = comboBox1.SelectedItem?.ToString();
            if (string.IsNullOrWhiteSpace(flatType))
            {
                MessageBox.Show("Select a flat type."); 
                return;
            }

            string status = "Vacant";
            object bachelorCategoryParam = DBNull.Value;

            if (flatType == "Bachelor")
            {
                if (radioButton1.Checked)
                    bachelorCategoryParam = "Male";
                else if (radioButton2.Checked)
                    bachelorCategoryParam = "Female";
                else
                {
                    MessageBox.Show("Select a Bachelor Category (Male/Female)."); 
                    return;
                }
            }

            string sql = @"INSERT INTO Flats (LandlordID, Location, RentAmount, FlatType, Status, BachelorCategory) OUTPUT INSERTED.FlatID
                           VALUES (@LandlordID, @Location, @RentAmount, @FlatType, @Status, @BachelorCategory);";

            int newFlatId;
            using (var conn = new SqlConnection(cs))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@LandlordID", landlordId);
                cmd.Parameters.AddWithValue("@Location", location);
                cmd.Parameters.AddWithValue("@RentAmount", rentAmount);
                cmd.Parameters.AddWithValue("@FlatType", flatType);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@BachelorCategory", bachelorCategoryParam);

                conn.Open();
                newFlatId = (int)cmd.ExecuteScalar();

                string notifySql = @" INSERT INTO Notifications (FlatID, LandlordID, Message, CreatedAt) VALUES (@FID, @LID, @Msg, GETDATE());";

                using (var notifyCmd = new SqlCommand(notifySql, conn))
                {
                    notifyCmd.Parameters.AddWithValue("@FID", newFlatId);
                    notifyCmd.Parameters.AddWithValue("@LID", landlordId);
                    notifyCmd.Parameters.AddWithValue("@Msg", "A new flat has been posted and is available for booking.");
                    notifyCmd.ExecuteNonQuery();
                }
            }

            textBox1.Text = newFlatId.ToString();
            MessageBox.Show("Succussful","Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            var dash = new LandlordDashboard();
            dash.LoggedInUsername = this.LoggedInUsername;
            dash.LoggedInuserId = this.LoggedInuserId;
            dash.Show();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            LandlordDashboard ld2= new LandlordDashboard();
            ld2.LoggedInuserId=this.LoggedInuserId;
            ld2.LoggedInUsername=this.LoggedInUsername;
            ld2.Show();
            this.Hide();
        }

        private void button6_Click(object sender, EventArgs e)
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
