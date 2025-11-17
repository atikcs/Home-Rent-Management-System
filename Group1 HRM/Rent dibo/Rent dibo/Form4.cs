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
    public partial class Form4 : Form
    {
        string connectionString = "data source=LAPTOP-EA0M4NKO\\SQLEXPRESS; database=Home; integrated security=SSPI";
        public string LoggedInUsername { get; set; }
        public int LoggedInuserId { get; set; }
        private string username;

        
        public Form4(string loggedInUsername)
        {
            InitializeComponent();
            username = loggedInUsername;
        }

        
        public Form4()
        {
            InitializeComponent();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            comboBox1.Items.Add("Male");
            comboBox1.Items.Add("Female");

            
            if (string.IsNullOrWhiteSpace(username))
                username = LoggedInUsername;

            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Username not provided. Please log in again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            LoadDetails();
        }
        private void LoadDetails()
        {
            const string q = @" SELECT t.TenantID, t.UserID, t.FullName, t.Phone, t.Email, t.Address, t.Gender
            FROM Tenants t
            INNER JOIN Users u ON u.UserID = t.UserID
            WHERE u.Username = @Username";

            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(q, conn))
            {
                cmd.Parameters.AddWithValue("@Username", username);   
                conn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        textBox1.Text = r["TenantID"].ToString();
                        textBox2.Text = r["UserID"].ToString();
                        textBox3.Text = r["FullName"]?.ToString();
                        textBox4.Text = r["Phone"]?.ToString();
                        textBox5.Text = r["Email"]?.ToString();
                        textBox6.Text = r["Address"]?.ToString();

                        var g = r["Gender"]?.ToString();
                        comboBox1.SelectedItem = (g == "Male" || g == "Female") ? g : null;
                    }
                    else
                    {
                        textBox1.Clear(); textBox2.Clear(); textBox3.Clear();
                        textBox4.Clear(); textBox5.Clear(); textBox6.Clear();
                        comboBox1.SelectedItem = null;
                    }
                }
            }
        }


        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            TenantDashboard td=new TenantDashboard();
            td.LoggedInUsername=this.LoggedInUsername;
            td.LoggedInuserId = this.LoggedInuserId;
            td.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 form1 = new Form1();
            form1.Show();
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

        private void button1_Click(object sender, EventArgs e)
        {
            string newFullName = textBox3.Text.Trim();
            string newPhone = textBox4.Text.Trim();
            string newEmail = textBox5.Text.Trim();
            string newAddress = textBox6.Text.Trim();
            string newGender = comboBox1.SelectedItem?.ToString();

            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("TenantID is missing. Load the profile first.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(newFullName) ||
                string.IsNullOrWhiteSpace(newPhone) ||
                string.IsNullOrWhiteSpace(newEmail) ||
                string.IsNullOrWhiteSpace(newAddress) ||
                string.IsNullOrWhiteSpace(newGender))
            {
                MessageBox.Show("All fields must be filled out.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = @"UPDATE Tenants SET FullName = @FullName,Phone = @Phone,Email  = @Email,Address  = @Address, Gender   = @Gender WHERE TenantID = @TenantID;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@TenantID", textBox1.Text);   
                command.Parameters.AddWithValue("@FullName", newFullName);
                command.Parameters.AddWithValue("@Phone", newPhone);
                command.Parameters.AddWithValue("@Email", newEmail);
                command.Parameters.AddWithValue("@Address", newAddress);
                command.Parameters.AddWithValue("@Gender", newGender);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Tenant updated successfully!","Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("No record was updated. Please check .","Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }                
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("TenantID is missing. Load the profile first.", "Validation Error",MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("Are you sure you want to delete this profile?","Confirm Deletion",MessageBoxButtons.YesNo,MessageBoxIcon.Warning);

            if (result != DialogResult.Yes) 
                return;

            string query = "DELETE FROM Tenants WHERE TenantID = @TenantID;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@TenantID", textBox1.Text);   

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Profile deleted successfully!","Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("No profile found with this TenantID.","Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

       
    }
}
