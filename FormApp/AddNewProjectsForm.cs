using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace FormApp
{
    public partial class AddNewProjectsForm : Form
    {
        private ServerClient Server;

        public AddNewProjectsForm(ServerClient server)
        {
            InitializeComponent();
            Server = server;
        }

        private void AddNewProjectsForm_Load(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string command = $"ADD_PROJECT_SQL|{textBox1.Text}|{textBox2.Text}|{textBox3.Text}|{textBox4.Text}";
            string response = await Server.SendCommand(command);
            MessageBox.Show(response);
            if (response.StartsWith("SUCCESS")) this.Close();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            string command = $"ADD_PROJECT_ORM|{textBox1.Text}|{textBox2.Text}|{textBox3.Text}|{textBox4.Text}";
            string response = await Server.SendCommand(command);
            MessageBox.Show(response);
            if (response.StartsWith("SUCCESS")) this.Close();
        }
    }
}
