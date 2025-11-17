using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rent_dibo
{
    public partial class RoomBookDetails : Form
    {
        private int _flatId;
        public string LoggedInUsername { get; set; }
        public int LoggedInuserId { get; set; }

        public RoomBookDetails(int flatId)
        {
            InitializeComponent();
            _flatId = flatId;
        }


        public RoomBookDetails(int roomId, int flatId, string roomNumber, string rentAmount, string status)
        {
            InitializeComponent();
            textBox1.Text = roomId.ToString();
            textBox2.Text = flatId.ToString();
            textBox3.Text = roomNumber;
            textBox4.Text = rentAmount;
            textBox5.Text = status;
        }

        public RoomBookDetails()
        {
            InitializeComponent();
        }

        private void RoomBookForm_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(textBox2.Text, out int flatId))
            {
                MessageBox.Show("Invalid FlatID.", "Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            RoomBookForms form = new RoomBookForms(flatId);
            form.LoggedInUsername = this.LoggedInUsername;
            form.LoggedInuserId = this.LoggedInuserId;
            form.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RoomBookForms form = new RoomBookForms(_flatId);
            form.LoggedInUsername = this.LoggedInUsername;
            form.LoggedInuserId = this.LoggedInuserId;
            form.Show();
            this.Hide();

        }

        private void button5_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 form1 = new Form1();
            form1.Show();
        }
    }
}
