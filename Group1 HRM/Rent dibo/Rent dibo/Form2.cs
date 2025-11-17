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
    public partial class Form2 : Form
    {
        string connectionString = "data source=LAPTOP-EA0M4NKO\\SQLEXPRESS; database=Home; integrated security=SSPI";
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {          

            string username = UserNametextBox2.Text.Trim();
            string password = PasswordtextBox3.Text.Trim();
            string confirm = RPasswordtextBox4.Text.Trim();
            string role = RolecomboBox1.SelectedItem == null ? "" : RolecomboBox1.SelectedItem.ToString();

            bool hasUpper = password.Any(char.IsUpper);
            bool hasLower = password.Any(char.IsLower);

           
            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Please enter a username.");
                return;
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter a password.");
                return;
            }
            if (password != confirm)
            {
                MessageBox.Show("Passwords do not match.");
                return;
            }
            if (string.IsNullOrWhiteSpace(role))
            {
                MessageBox.Show("Please select a role.");
                return;
            }
            if (password.Length < 8 || !hasUpper || !hasLower)
            {
                MessageBox.Show("Password must have at least 8 characters, one uppercase, and one lowercase.");
                return;
            }
            if (!chkAgreeTerms.Checked)
            {
                MessageBox.Show("You must agree to the Terms and Conditions before signing up.","Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                
                string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, connection))
                {
                    checkCmd.Parameters.AddWithValue("@Username", username);
                    int exists = (int)checkCmd.ExecuteScalar();
                    if (exists > 0)
                    {
                        MessageBox.Show("This username is already taken."); return;
                    }
                }

                
                string insertQuery = "INSERT INTO Users (Username, Password, Role) OUTPUT INSERTED.UserID VALUES (@Username, @Password, @Role)";
                int newUserId = 0;
                using (SqlCommand cmd = new SqlCommand(insertQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password);
                    cmd.Parameters.AddWithValue("@Role", role);

                    newUserId = (int)cmd.ExecuteScalar(); 
                }

            
                if (role == "Tenant")
                {
                    string insertTenant = "INSERT INTO Tenants (UserID, FullName, Gender, Phone, Email, Address) VALUES (@UserID, @FullName, @Gender, @Phone, @Email, @Address)";
                    using (SqlCommand tenantCmd = new SqlCommand(insertTenant, connection))
                    {
                        tenantCmd.Parameters.AddWithValue("@UserID", newUserId);
                        tenantCmd.Parameters.AddWithValue("@FullName", username); 
                        tenantCmd.Parameters.AddWithValue("@Gender", DBNull.Value); 
                        tenantCmd.Parameters.AddWithValue("@Phone", DBNull.Value);
                        tenantCmd.Parameters.AddWithValue("@Email", DBNull.Value);
                        tenantCmd.Parameters.AddWithValue("@Address", DBNull.Value);

                        tenantCmd.ExecuteNonQuery();
                    }
                }
                else if (role == "Landlord")
                {
                    string insertLandlord = "INSERT INTO Landlords (UserID, FullName, Phone, Email, Address) VALUES (@UserID, @FullName, @Phone, @Email, @Address)";
                    using (SqlCommand landlordCmd = new SqlCommand(insertLandlord, connection))
                    {
                        landlordCmd.Parameters.AddWithValue("@UserID", newUserId);
                        landlordCmd.Parameters.AddWithValue("@FullName", username);
                        landlordCmd.Parameters.AddWithValue("@Phone", DBNull.Value);
                        landlordCmd.Parameters.AddWithValue("@Email", DBNull.Value);
                        landlordCmd.Parameters.AddWithValue("@Address", DBNull.Value);

                        landlordCmd.ExecuteNonQuery();
                    }
                }
                else if (role == "Manager")
                {

                    string insertManager = @"INSERT INTO BachelorFlatManager (UserID, FlatID, ManagerName, Phone, Email, AssignedDate) VALUES (@UserID, NULL, @ManagerName, NULL, NULL, GETDATE());";

                    using (SqlCommand managerCmd = new SqlCommand(insertManager, connection))
                    {
                        managerCmd.Parameters.AddWithValue("@UserID", newUserId); 
                        managerCmd.Parameters.AddWithValue("@ManagerName", username); 
                        managerCmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Signup successful!");
                this.Hide();
                Form1 login = new Form1();
                login.Show();

                connection.Close();
            }

        }
        

        private void Form2_Load(object sender, EventArgs e)
        {
            RolecomboBox1.Items.Clear();
            RolecomboBox1.Items.Add("Manager");
            RolecomboBox1.Items.Add("Landlord");
            RolecomboBox1.Items.Add("Tenant");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 form1 = new Form1();
            form1.Show();
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
    }
}

