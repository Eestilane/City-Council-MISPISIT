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
                textBox3.Enabled = false;
                textBox3.Text = "";
            }
            else
            {
                label5.Enabled = true;
                textBox3.Enabled = true;
            }
        }

        private void RegisterForm_Load(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxLogin.Text) ||
                string.IsNullOrEmpty(textBoxPassword.Text) ||
                string.IsNullOrEmpty(textBox1.Text) ||
                string.IsNullOrEmpty(textBox2.Text) ||
                string.IsNullOrEmpty(textBox4.Text) ||
                string.IsNullOrEmpty(textBox5.Text))
            {
                MessageBox.Show("Заполните все обязательные поля");
                return;
            }

            if (!await Server.ConnectAsync())
            {
                MessageBox.Show("Не удалось подключиться к серверу");
                return;
            }

            string middleName = checkBox1.Checked ? "" : textBox3.Text;
            string command = $"REGISTER_DEPUTY|{textBoxLogin.Text}|{textBoxPassword.Text}|{textBox1.Text}|{textBox2.Text}|{middleName}|{textBox4.Text}|{textBox5.Text}";
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
