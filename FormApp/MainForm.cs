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
        private ServerClient Server;
        private string currentTable = "";
        private int editingRowId = -1;
        private string editingColumn = "";
        private string newValue = "";
        private IDataStrategy _strategy;

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
                Server = loginForm.Server;
            }
            MessageBox.Show("Авторизация успешна!", "Добро пожаловать");

            _strategy = new SqlStrategy(Server);
            radioButtonSQL.Checked = true;

            radioButtonSQL.CheckedChanged += (s, ev) =>
            {
                if (radioButtonSQL.Checked)
                    _strategy = new SqlStrategy(Server);
            };

            radioButtonORM.CheckedChanged += (s, ev) =>
            {
                if (radioButtonORM.Checked)
                    _strategy = new OrmStrategy(Server);
            };
        }

        private int GetSelectedId()
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выделите строку");
                return -1;
            }

            object idValue = dataGridView1.SelectedRows[0].Cells[0].Value;

            if (idValue == null || !int.TryParse(idValue.ToString(), out int id))
            {
                MessageBox.Show("Не удалось получить ID");
                return -1;
            }

            return id;
        }

        //Тест
        private async void buttonTest_Click(object sender, EventArgs e)
        {
            string response = await Server.SendCommand("TEST");
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

            string response = await Server.SendCommand($"EXECUTE_SQL|{sql}");

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
        private async void buttonDeputies_Click(object sender, EventArgs e)
        {
            string response = await Server.SendCommand("GET_DEPUTIES");

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
            currentTable = "deputies";
        }

        //ORM список собраний
        private async void buttonMeetings_Click(object sender, EventArgs e)
        {
            string response = await Server.SendCommand("GET_MEETINGS");

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
            currentTable = "meetings";
        }

        //ORM список проектов
        private async void buttonProjects_Click(object sender, EventArgs e)
        {
            string response = await Server.SendCommand("GET_PROJECTS");

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
            currentTable = "projects";
        }

        //ORM список голосований
        private async void buttonVotes_Click(object sender, EventArgs e)
        {
            string response = await Server.SendCommand("GET_VOTES");

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
            currentTable = "votes";
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Server?.Disconnect();
        }

        private void buttonClearDataGrid_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = null;
        }

        private void buttonAddProject_Click(object sender, EventArgs e)
        {
            Form form = new AddNewProjectsForm(_strategy);
            form.Show();
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            editingRowId = (int)dataGridView1.Rows[e.RowIndex].Cells[0].Value;
            editingColumn = dataGridView1.Columns[e.ColumnIndex].Name;
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            newValue = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString() ?? "";
        }

        private async void buttonDelete_Click(object sender, EventArgs e)
        {
            int id = GetSelectedId();
            if (id == -1) return;

            if (string.IsNullOrEmpty(currentTable))
            {
                MessageBox.Show("Сначала загрузите таблицу");
                return;
            }

            string response = await _strategy.DeleteRecord(currentTable, id);
            MessageBox.Show(response);
        }

        private async void buttonSave_Click(object sender, EventArgs e)
        {
            if (editingRowId == -1)
            {
                MessageBox.Show("Сначала отредактируйте ячейку");
                return;
            }

            string response = await _strategy.UpdateRecord(currentTable, editingRowId, editingColumn, newValue);
            MessageBox.Show(response);

            editingRowId = -1;
        }

        private async void buttonSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentTable))
            {
                MessageBox.Show("Сначала загрузите таблицу");
                return;
            }

            string response = await _strategy.SearchRecord(currentTable, textBox1.Text);
            DisplaySearchResults(response);
        }

        private void DisplaySearchResults(string response)
        {
            if (response.StartsWith("SEARCH_RESULT"))
            {
                string[] parts = response.Split('|');
                var dt = new System.Data.DataTable();

                if (parts.Length < 2)
                {
                    dataGridView1.DataSource = null;
                    return;
                }

                string[] columnNames = parts[1].Split(',');
                foreach (string colName in columnNames)
                {
                    dt.Columns.Add(colName);
                }

                for (int i = 2; i < parts.Length; i++)
                {
                    string[] rowData = parts[i].Split(',');
                    if (rowData.Length == columnNames.Length)
                    {
                        dt.Rows.Add(rowData);
                    }
                }

                dataGridView1.DataSource = dt;

                if (dt.Rows.Count == 0)
                    MessageBox.Show("Ничего не найдено");
            }
            else
            {
                MessageBox.Show($"Ошибка: {response}");
            }
        }
    }
}
