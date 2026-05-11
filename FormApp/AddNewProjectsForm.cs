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
using VotingSystemWinForms;

namespace FormApp
{
    public partial class AddNewProjectsForm : Form
    {
        private readonly IDataStrategy _strategy;

        public AddNewProjectsForm(IDataStrategy strategy)
        {
            InitializeComponent();
            _strategy = strategy;
        }

        private void AddNewProjectsForm_Load(object sender, EventArgs e)
        {

        }

        private async void buttonAddForm_Click(object sender, EventArgs e)
        {
            string response = await _strategy.AddProject(int.Parse(textBoxMeetingNumber.Text), textBoxTitle.Text, textBoxType.Text, textBoxStatus.Text);
            MessageBox.Show(response);
            if (response.StartsWith("SUCCESS")) this.Close();
        }
    }
}
