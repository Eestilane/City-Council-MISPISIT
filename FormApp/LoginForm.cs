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
    public partial class LoginForm : Form
    {
        public ServerClient Server { get; private set; }
        public bool IsAuthenticated { get; private set; }

        public LoginForm()
        {
            InitializeComponent();
            Server = new ServerClient();
            IsAuthenticated = false;
        }

        private async void buttonConnect_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxLogin.Text) || string.IsNullOrEmpty(textBoxPassword.Text))
            {
                MessageBox.Show("Введите логин и пароль");
                return;
            }

            buttonConnect.Enabled = false;
            label3.Text = "Подключение к серверу...";

            if (!await Server.ConnectAsync())
            {
                label3.Text = "Ошибка подключения к серверу";
                buttonConnect.Enabled = true;
                return;
            }

            label3.Text = "Отправка данных авторизации...";

            string response = await Server.SendCommand($"LOGIN|{textBoxLogin.Text}|{textBoxPassword.Text}");

            if (response == "SUCCESS")
            {
                IsAuthenticated = true;
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                label3.Text = "Ошибка: " + response;
                buttonConnect.Enabled = true;
                Server.Disconnect();
            }
        }

        private void buttonRegister_Click(object sender, EventArgs e)
        {
            using (var registerForm = new RegisterForm(Server))
            {
                if (registerForm.ShowDialog() != DialogResult.OK)
                {
                    Application.Exit();
                    return;
                }
            }
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
