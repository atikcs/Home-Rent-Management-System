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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Rent_dibo
{
    public partial class Form5 : Form
    {
        public string LoggedInUsername { get; set; }
        public int LoggedInuserId { get; set; }
        string connectionString = "data source=LAPTOP-EA0M4NKO\\SQLEXPRESS; database=Home; integrated security=SSPI";
        public Form5()
        {
            InitializeComponent();

            InitFlatTypeCombo();

            //  available show
            string startQuery = @"SELECT f.FlatID,f.Location,f.FlatType,f.RentAmount,f.Status
            FROM dbo.Flats AS f
            WHERE f.Status = 'Vacant'
            ORDER BY f.FlatID DESC;";
            FillDataGridView(startQuery, null);
           
        }

        private void InitFlatTypeCombo()
        {
            comboBox1.Items.Clear();
            comboBox1.Items.Add("All");
            comboBox1.Items.Add("Family");
            comboBox1.Items.Add("Bachelor");
            comboBox1.SelectedIndex = 0; 
        }

        private void FillDataGridView(string query, Action<SqlCommand> bindParams)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                bindParams?.Invoke(cmd);
                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    dataGridView1.DataSource = dt;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string location = textBox1.Text.Trim();
            string flatType = comboBox1.SelectedItem == null ? "All" : comboBox1.SelectedItem.ToString();

          
            StringBuilder sb = new StringBuilder(@"SELECT  f.FlatID,f.Location,f.FlatType,f.RentAmount,f.Status
            FROM dbo.Flats AS f
            WHERE f.Status = 'Vacant'");

            if (!string.IsNullOrWhiteSpace(location))
            {
                sb.Append(" AND f.Location LIKE @loc");
            }

            if (!string.Equals(flatType, "All", StringComparison.OrdinalIgnoreCase))
            {
                sb.Append(" AND f.FlatType = @ft"); 
            }

            sb.Append(" ORDER BY f.FlatID DESC;");

            string query = sb.ToString();

            FillDataGridView(query, cmd =>
            {
                if (!string.IsNullOrWhiteSpace(location))
                    cmd.Parameters.AddWithValue("@loc", "%" + location + "%");

                if (!string.Equals(flatType, "All", StringComparison.OrdinalIgnoreCase))
                    cmd.Parameters.AddWithValue("@ft", flatType);
            });

            if (dataGridView1.DataSource is DataTable dt && dt.Rows.Count == 0)
            {
                MessageBox.Show("No available flats found with the selected filters.",
                    "Search Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return; 

            object val = dataGridView1.Rows[e.RowIndex].Cells["FlatID"].Value;
            if (val == null || val == DBNull.Value)
            {
                MessageBox.Show("Selected row has no FlatID.", "Open Flat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(val.ToString(), out int flatId))
            {
                MessageBox.Show("Invalid FlatID.", "Open Flat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            
            FlatDetailsForm f = new FlatDetailsForm(flatId);
            f.LoggedInUsername = this.LoggedInUsername;   
            f.LoggedInuserId = this.LoggedInuserId;
            f.StartPosition = FormStartPosition.CenterParent;
            f.Show();
            this.Hide();


        }

        private void Form5_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            TenantDashboard form3 = new TenantDashboard();
            form3.LoggedInUsername = LoggedInUsername; 
            form3.LoggedInuserId = LoggedInuserId;   
            form3.StartPosition = FormStartPosition.CenterScreen;
            form3.Show();
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
            PaymentForm paymentForm = new PaymentForm();
            paymentForm.LoggedInUsername = this.LoggedInUsername;
            paymentForm.LoggedInuserId=this.LoggedInuserId;
            paymentForm.Show();
            this.Hide();
        }
    }
}