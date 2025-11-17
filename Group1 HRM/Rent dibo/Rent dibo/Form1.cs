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
    public partial class Form1 : Form
    {
        string connectstring = "data source=LAPTOP-EA0M4NKO\\SQLEXPRESS; database=Home; integrated security=SSPI";

        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string username = UsernametextBox1.Text.Trim();
            string password = PasswordtextBox2.Text.Trim();

            
            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Please enter username.", "Validation error!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter password", "Validation error!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            using (SqlConnection connection = new SqlConnection(connectstring))
            {
                connection.Open();

                
                string query = "SELECT UserId, Role FROM Users WHERE Username=@Username AND Password=@Password";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int userId = Convert.ToInt32(reader["UserId"]);
                            string role = reader["Role"].ToString();

                            MessageBox.Show("Login Successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Hide();

                            if (role == "Manager")
                            {
                                ManagerDashboard managerForm = new ManagerDashboard();
                                managerForm.LoggedInUsername = username;
                                managerForm.LoggedInuserId = userId;
                                managerForm.Show();
                            }
                            else if (role == "Landlord")
                            {
                                LandlordDashboard landlordForm = new LandlordDashboard();
                                landlordForm.LoggedInUsername = username;
                                landlordForm.LoggedInuserId = userId;
                                landlordForm.Show();
                            }
                            else if (role == "Tenant")
                            {
                                TenantDashboard tenantForm = new TenantDashboard();
                                tenantForm.LoggedInUsername = username;
                                tenantForm.LoggedInuserId = userId;
                                tenantForm.Show();
                                this.Hide();
                            }
                            else
                            {
                                MessageBox.Show("Unknown role!", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Invalid username or password!", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form2 form2 = new Form2();
            form2.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            PasswordtextBox2.PasswordChar = '*';
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            PasswordtextBox2.PasswordChar = checkBox1.Checked ? '\0' : '*';
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}

