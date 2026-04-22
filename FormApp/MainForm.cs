using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VotingSystemWinForms;

namespace FormApp
{
    public partial class MainForm : Form
    {
        private ServerClient server;

        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            using (var loginForm = new LoginForm())
            {
                if (loginForm.ShowDialog() != DialogResult.OK)
                {
                    Application.Exit();
                    return;
                }
                server = loginForm.Server;
            }
            MessageBox.Show("Авторизация успешна!", "Добро пожаловать");
        }

        //Тест
        private async void button1_Click(object sender, EventArgs e)
        {
            string response = await server.SendCommand("TEST");
            MessageBox.Show($"Ответ сервера: {response}");
        }

        //SQL запрос из textbox
        private async void buttonEnter_Click(object sender, EventArgs e)
        {
            string sql = textBox1.Text.Trim();
            if (string.IsNullOrEmpty(sql))
            {
                MessageBox.Show("Введите SQL-запрос");
                return;
            }

            string response = await server.SendCommand($"EXECUTE_SQL|{sql}");

            if (response.StartsWith("SQL_RESULT"))
            {
                // Парс результата SQL-запроса
                string[] parts = response.Split(new[] { "|ROW_SEP" }, StringSplitOptions.None);
                if (parts.Length > 0)
                {
                    string[] headers = parts[0].Split('|');
                    var dt = new DataTable();

                    //Колонки
                    for (int i = 1; i < headers.Length; i++)
                    {
                        dt.Columns.Add(headers[i]);
                    }

                    //Строки
                    for (int i = 1; i < parts.Length - 1; i++)
                    {
                        string[] rowData = parts[i].Split('|');
                        if (rowData.Length >= dt.Columns.Count)
                        {
                            DataRow row = dt.NewRow();
                            for (int j = 1; j < rowData.Length; j++)
                            {
                                row[j - 1] = rowData[j];
                            }
                            dt.Rows.Add(row);
                        }
                    }
                    dataGridView1.DataSource = dt;
                }
            }
            else if (response.StartsWith("SUCCESS"))
            {
                MessageBox.Show(response);
            }
            else
            {
                MessageBox.Show($"Ошибка: {response}");
            }
        }

        //ORM список депутатов
        private async void button2_Click(object sender, EventArgs e)
        {
            string response = await server.SendCommand("GET_DEPUTIES");

            if (response.StartsWith("DEPUTIES"))
            {
                string[] parts = response.Split('|');
                var deputies = new List<Deputy>();

                for (int i = 1; i < parts.Length; i += 7)
                {
                    if (i + 6 < parts.Length)
                    {
                        deputies.Add(new Deputy
                        {
                            Id = int.Parse(parts[i]),
                            LastName = parts[i + 1],
                            FirstName = parts[i + 2],
                            MiddleName = parts[i + 3],
                            District = parts[i + 4],
                            Party = parts[i + 5],
                            Status = parts[i + 6]
                        });
                    }
                }
                dataGridView1.DataSource = deputies;
            }

            else
            {
                MessageBox.Show($"Ошибка: {response}");
            }
        }

        //ORM список собраний
        private async void button3_Click(object sender, EventArgs e)
        {
            string response = await server.SendCommand("GET_MEETINGS");

            if (response.StartsWith("MEETINGS"))
            {
                string[] parts = response.Split('|');
                var meetings = new List<Meeting>();

                for (int i = 1; i < parts.Length; i += 5)
                {
                    if (i + 4 < parts.Length)
                    {
                        meetings.Add(new Meeting
                        {
                            Id = int.Parse(parts[i]),
                            Date = DateTime.Parse(parts[i + 1]),
                            StartTime = TimeSpan.Parse(parts[i + 2]),
                            Type = parts[i + 3],
                            Status = parts[i + 4]
                        });
                    }
                }
                dataGridView1.DataSource = meetings;
            }
            else
            {
                MessageBox.Show($"Ошибка: {response}");
            }
        }

        //ORM список проектов
        private async void button4_Click(object sender, EventArgs e)
        {
            string response = await server.SendCommand("GET_PROJECTS");

            if (response.StartsWith("PROJECTS"))
            {
                string[] parts = response.Split('|');
                var projects = new List<Project>();

                for (int i = 1; i < parts.Length; i += 5)
                {
                    if (i + 4 < parts.Length)
                    {
                        projects.Add(new Project
                        {
                            Id = int.Parse(parts[i]),
                            MeetingNumber = int.Parse(parts[i + 1]),
                            Title = parts[i + 2],
                            Type = parts[i + 3],
                            Status = parts[i + 4]
                        });
                    }
                }
                dataGridView1.DataSource = projects;
            }
            else
            {
                MessageBox.Show($"Ошибка: {response}");
            }
        }

        //ORM список голосований
        private async void button5_Click(object sender, EventArgs e)
        {
            string response = await server.SendCommand("GET_VOTES");

            if (response.StartsWith("VOTES"))
            {
                string[] parts = response.Split('|');
                var votes = new List<Vote>();

                for (int i = 1; i < parts.Length; i += 4)
                {
                    if (i + 3 < parts.Length)
                    {
                        votes.Add(new Vote
                        {
                            Id = int.Parse(parts[i]),
                            ProjectNumber = int.Parse(parts[i + 1]),
                            Deputy = int.Parse(parts[i + 2]),
                            Result = parts[i + 3]
                        });
                    }
                }
                dataGridView1.DataSource = votes;
            }
            else
            {
                MessageBox.Show($"Ошибка: {response}");
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            server?.Disconnect();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = null;
        }
    }
}
