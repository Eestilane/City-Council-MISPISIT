using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormApp
{
    public partial class RegisterForm : Form
    {
        public ServerClient Server { get; private set; }

        public RegisterForm(ServerClient server)
        {
            InitializeComponent();
            Server = server;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked) 
            { 
                label5.Enabled = false;
                textBoxMiddleName.Enabled = false;
                textBoxMiddleName.Text = "";
            }
            else
            {
                label5.Enabled = true;
                textBoxMiddleName.Enabled = true;
            }
        }

        private void RegisterForm_Load(object sender, EventArgs e)
        {

        }

        private async void buttonRegister_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxLogin.Text) ||
                string.IsNullOrEmpty(textBoxPassword.Text) ||
                string.IsNullOrEmpty(textBoxLastName.Text) ||
                string.IsNullOrEmpty(textBoxFirstName.Text) ||
                string.IsNullOrEmpty(textBoxDistrict.Text) ||
                string.IsNullOrEmpty(textBoxParty.Text))
            {
                MessageBox.Show("Заполните все обязательные поля");
                return;
            }

            if (!await Server.ConnectAsync())
            {
                MessageBox.Show("Не удалось подключиться к серверу");
                return;
            }

            string middleName = checkBox1.Checked ? "" : textBoxMiddleName.Text;
            string command = $"REGISTER_DEPUTY|{textBoxLogin.Text}|{textBoxPassword.Text}|{textBoxLastName.Text}|{textBoxFirstName.Text}|{middleName}|{textBoxDistrict.Text}|{textBoxParty.Text}";
            string response = await Server.SendCommand(command);

            if (response.StartsWith("SUCCESS"))
            {
                MessageBox.Show("Регистрация успешна!");
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show($"Ошибка: {response}");
            }
        }
    }
}
