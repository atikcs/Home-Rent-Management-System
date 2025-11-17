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
    public partial class Notice : Form
    {
        private readonly string cs =
            "data source=LAPTOP-EA0M4NKO\\SQLEXPRESS; database=Home; integrated security=SSPI";

        public Notice()
        {
            InitializeComponent();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Notice_Load(object sender, EventArgs e)
        {
            LoadNotices();
        }
        private void LoadNotices()
        {
            const string q = @"SELECT n.NotificationID, n.FlatID, f.Location, f.FlatType,f.RentAmount,n.LandlordID, n.Message, n.CreatedAt
                             FROM dbo.Notifications n
                              LEFT JOIN dbo.Flats f ON f.FlatID = n.FlatID
                              ORDER BY n.CreatedAt DESC;";

            using (SqlConnection conn = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand(q, conn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                var dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;

                dataGridView1.ReadOnly = true;
                dataGridView1.MultiSelect = false;
                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
        }
    }
}
