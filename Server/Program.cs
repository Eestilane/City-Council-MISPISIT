using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Server;

namespace Server
{
    class Program
    {
        private static string connectionString;

        static void Main(string[] args)
        {
            connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            Console.WriteLine("Сервер запущен");
            TcpListener server = new TcpListener(IPAddress.Parse("127.0.0.1"), 8888);
            server.Start();
            Console.WriteLine("Сервер слушает порт 8888...");

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Клиент подключился!");
                Task.Run(() => HandleClient(client));
            }
        }

        static void HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[4096];

            try
            {
                while (true)
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string command = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                    Console.WriteLine($"Получено: {command}");

                    string response = ProcessCommand(command);

                    byte[] responseData = Encoding.UTF8.GetBytes(response + "\n");
                    stream.Write(responseData, 0, responseData.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
            finally
            {
                client.Close();
                Console.WriteLine("Клиент отключился");
            }
        }

        static string ProcessCommand(string command)
        {
            Console.WriteLine($"Обработка команды: {command}");
            string[] parts = command.Split('|');
            string cmd = parts[0].ToUpper();

            switch (cmd)
            {
                //SQL
                case "EXECUTE_SQL":
                    return HandleExecuteSQL(parts);
                case "ADD_PROJECT_SQL":
                    return AddProjectSQL(parts);
                case "UPDATE_SQL":
                    return UpdateRecordSQL(parts);
                case "DELETE_SQL":
                    return DeleteRecordSQL(parts);
                case "SEARCH_SQL":
                    return SearchSQL(parts);

                //ORM
                case "LOGIN":
                    return Login(parts);
                case "REGISTER_DEPUTY":
                    return RegisterDeputy(parts);
                case "GET_DEPUTIES":
                    return GetDeputies();
                case "GET_MEETINGS":
                    return GetMeetings();
                case "GET_PROJECTS":
                    return GetProjects();
                case "ADD_PROJECT_ORM":
                    return AddProjectORM(parts);
                case "GET_VOTES":
                    return GetVotes();
                case "UPDATE_ORM":
                    return UpdateRecordORM(parts);
                case "DELETE_ORM":
                    return DeleteRecordORM(parts);
                case "SEARCH_ORM":
                    return SearchORM(parts);

                //Тест
                case "TEST":
                    return "OK|Команда получена";

                default:
                    return "ERROR|Неизвестная команда";
            }
        }

        //SQL
        static string HandleExecuteSQL(string[] parts)
        {
            if (parts.Length < 2)
                return "ERROR|Не указан SQL-запрос";

            string sql = parts[1];

            // Определение типа запроса
            bool isSelect = sql.TrimStart().StartsWith("SELECT", StringComparison.OrdinalIgnoreCase);

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        if (isSelect)
                        {
                            //SELECT
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                var result = new StringBuilder("SQL_RESULT");

                                //Заголовки столбцов
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    result.Append($"|{reader.GetName(i)}");
                                }
                                result.Append("|ROW_SEP");

                                //Данные
                                while (reader.Read())
                                {
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        result.Append($"|{reader[i]?.ToString() ?? "NULL"}");
                                    }
                                    result.Append("|ROW_SEP");
                                }
                                return result.ToString();
                            }
                        }
                        else
                        {
                            //INSERT, UPDATE, DELETE
                            int rows = cmd.ExecuteNonQuery();
                            return $"SUCCESS|Затронуто строк: {rows}";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return $"ERROR|{ex.Message}";
            }
        }

        static string AddProjectSQL(string[] parts)
        {
            int meetingId = int.Parse(parts[1]);
            string title = parts[2];
            string type = parts[3];
            string status = parts[4];

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = "INSERT INTO Projects (MeetingNumber, Title, Type, Status) VALUES (@mid, @t, @typ, @s)";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@mid", meetingId);
                    cmd.Parameters.AddWithValue("@t", title);
                    cmd.Parameters.AddWithValue("@typ", type);
                    cmd.Parameters.AddWithValue("@s", status);
                    cmd.ExecuteNonQuery();
                }
            }
            return "SUCCESS|Проект добавлен (SQL)";
        }

        static string UpdateRecordSQL(string[] parts)
        {
            string tableName = parts[1].ToLower();

            if (!int.TryParse(parts[2], out int id))
                return "ERROR|Некорректный ID";

            string columnName = parts[3];
            string newValue = parts[4];

            newValue = newValue.Replace("'", "''");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = $"UPDATE {tableName} SET {columnName} = N'{newValue}' WHERE Id = {id}";

                try
                {
                    int rows = new SqlCommand(sql, conn).ExecuteNonQuery();
                    return rows > 0 ? "SUCCESS|Запись обновлена (SQL)" : "ERROR|Ничего не обновлено";
                }
                catch (Exception ex)
                {
                    return $"ERROR|{ex.Message}";
                }
            }
        }

        static string DeleteRecordSQL(string[] parts)
        {
            string tableName = parts[1].ToLower();

            if (!int.TryParse(parts[2], out int id))
                return "ERROR|Некорректный ID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                switch (tableName)
                {
                    case "deputies":
                        new SqlCommand($"DELETE FROM Votes WHERE Deputy = {id}", conn).ExecuteNonQuery();
                        new SqlCommand($"DELETE FROM Deputies WHERE Id = {id}", conn).ExecuteNonQuery();
                        break;
                    case "meetings":
                        new SqlCommand($"DELETE FROM Votes WHERE ProjectNumber IN (SELECT Id FROM Projects WHERE MeetingNumber = {id})", conn).ExecuteNonQuery();
                        new SqlCommand($"DELETE FROM Projects WHERE MeetingNumber = {id}", conn).ExecuteNonQuery();
                        new SqlCommand($"DELETE FROM Meetings WHERE Id = {id}", conn).ExecuteNonQuery();
                        break;
                    case "projects":
                        new SqlCommand($"DELETE FROM Votes WHERE ProjectNumber = {id}", conn).ExecuteNonQuery();
                        new SqlCommand($"DELETE FROM Projects WHERE Id = {id}", conn).ExecuteNonQuery();
                        break;
                    case "votes":
                        new SqlCommand($"DELETE FROM Votes WHERE Id = {id}", conn).ExecuteNonQuery();
                        break;
                    default:
                        return "ERROR|Неизвестная таблица";
                }
            }
            return $"SUCCESS|Запись из {tableName} удалена (SQL)";
        }

        static string SearchSQL(string[] parts)
        {
            string tableName = parts[1];
            string searchText = parts[2];

            List<string> columns = GetTableColumns(tableName);

            List<string> conditions = new List<string>();
            foreach (string col in columns)
            {
                conditions.Add($"{col} LIKE N'%{searchText}%'");
            }
            string whereClause = string.Join(" OR ", conditions);
            string sql = $"SELECT * FROM {tableName} WHERE {whereClause}";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    var result = new StringBuilder("SEARCH_RESULT");
                    var columnNames = new List<string>();

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        columnNames.Add(reader.GetName(i));
                    }
                    result.Append($"|{string.Join(",", columnNames)}");

                    while (reader.Read())
                    {
                        var rowData = new List<string>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            rowData.Add(reader[i]?.ToString() ?? "");
                        }
                        result.Append($"|{string.Join(",", rowData)}");
                    }

                    return result.ToString();
                }
            }
        }

        static List<string> GetTableColumns(string tableName)
        {
            var columns = new List<string>();
            string sql = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'{tableName}' AND COLUMN_NAME != 'Id'";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        columns.Add(reader[0].ToString());
                    }
                }
            }
            return columns;
        }

        //ORM
        static string Login(string[] parts)
        {
            if (parts.Length < 3)
                return "ERROR|Недостаточно параметров. Формат: LOGIN|login|password";

            string login = parts[1];
            string password = parts[2];

            using (var db = new VotingDbContext())
            {
                bool exists = db.Deputies.Any(d => d.Login == login && d.Password == password);
                return exists ? "SUCCESS" : "ERROR|Неверный логин или пароль";
            }
        }

        static string GetDeputies()
        {
            using (var db = new VotingDbContext())
            {
                var deputies = db.Deputies.OrderBy(d => d.LastName).ToList();
                var result = new StringBuilder("DEPUTIES");

                foreach (var d in deputies)
                {
                    result.Append($"|{d.Id}|{d.LastName}|{d.FirstName}|{d.MiddleName}|{d.District}|{d.Party}|{d.Status}");
                }
                return result.ToString();
            }
        }

        static string GetMeetings()
        {
            using (var db = new VotingDbContext())
            {
                var meetings = db.Meetings.OrderBy(m => m.Date).ThenBy(m => m.StartTime).ToList();
                var result = new StringBuilder("MEETINGS");

                foreach (var m in meetings)
                {
                    result.Append($"|{m.Id}|{m.Date:yyyy-MM-dd}|{m.StartTime}|{m.Type}|{m.Status}");
                }
                return result.ToString();
            }
        }

        static string GetProjects()
        {
            using (var db = new VotingDbContext())
            {
                var projects = db.Projects.Include(p => p.Meeting).OrderBy(p => p.Id).ToList();
                var result = new StringBuilder("PROJECTS");

                foreach (var p in projects)
                {
                    result.Append($"|{p.Id}|{p.MeetingNumber}|{p.Title}|{p.Type}|{p.Status}");
                }
                return result.ToString();
            }
        }

        static string GetVotes()
        {
            using (var db = new VotingDbContext())
            {
                var votes = db.Votes.Include(v => v.Project).Include(v => v.DeputyNavigation).OrderBy(v => v.Id).ToList();
                var result = new StringBuilder("VOTES");

                foreach (var v in votes)
                {
                    result.Append($"|{v.Id}|{v.ProjectNumber}|{v.Deputy}|{v.Result}");
                }
                return result.ToString();
            }
        }

        static string RegisterDeputy(string[] parts)
        {
            string login = parts[1];
            string password = parts[2];
            string lastName = parts[3];
            string firstName = parts[4];
            string middleName = string.IsNullOrEmpty(parts[5]) ? null : parts[5];
            string district = parts[6];
            string party = parts[7];

            using (var db = new VotingDbContext())
            {
                if (db.Deputies.Any(d => d.Login == login))
                    return "ERROR|Депутат с таким логином уже существует";

                var newDeputy = new Deputy
                {
                    Login = login,
                    Password = password,
                    LastName = lastName,
                    FirstName = firstName,
                    MiddleName = middleName,
                    District = district,
                    Party = party,
                    Status = "Действующий",
                    Role = "Депутат"
                };

                db.Deputies.Add(newDeputy);
                db.SaveChanges();

                return "SUCCESS|Депутат зарегистрирован";
            }
        }

        static string AddProjectORM(string[] parts)
        {
            int meetingId = int.Parse(parts[1]);
            string title = parts[2];
            string type = parts[3];
            string status = parts[4];

            using (var db = new VotingDbContext())
            {
                db.Projects.Add(new Project { MeetingNumber = meetingId, Title = title, Type = type, Status = status });
                db.SaveChanges();
            }
            return "SUCCESS|Проект добавлен (ORM)";
        }

        static string UpdateRecordORM(string[] parts)
        {
            string tableName = parts[1].ToLower();

            if (!int.TryParse(parts[2], out int id))
                return "ERROR|Некорректный ID";

            string columnName = parts[3];
            string newValue = parts[4];

            using (var db = new VotingDbContext())
            {
                switch (tableName)
                {
                    case "deputies":
                        var deputy = db.Deputies.Find(id);
                        if (deputy == null) return "ERROR|Запись не найдена";
                        SetPropertyValue(deputy, columnName, newValue);
                        break;

                    case "meetings":
                        var meeting = db.Meetings.Find(id);
                        if (meeting == null) return "ERROR|Запись не найдена";
                        SetPropertyValue(meeting, columnName, newValue);
                        break;

                    case "projects":
                        var project = db.Projects.Find(id);
                        if (project == null) return "ERROR|Запись не найдена";
                        SetPropertyValue(project, columnName, newValue);
                        break;

                    case "votes":
                        var vote = db.Votes.Find(id);
                        if (vote == null) return "ERROR|Запись не найдена";
                        SetPropertyValue(vote, columnName, newValue);
                        break;

                    default:
                        return "ERROR|Неизвестная таблица";
                }

                db.SaveChanges();
            }

            return $"SUCCESS|Запись из {tableName} обновлена (ORM)";
        }

        private static void SetPropertyValue(object obj, string propertyName, string value)
        {
            var prop = obj.GetType().GetProperty(propertyName);
            if (prop == null) return;

            if (prop.PropertyType == typeof(DateTime))
                prop.SetValue(obj, DateTime.Parse(value));
            else if (prop.PropertyType == typeof(TimeSpan))
                prop.SetValue(obj, TimeSpan.Parse(value));
            else
                prop.SetValue(obj, value);
        }

        static string DeleteRecordORM(string[] parts)
        {
            string tableName = parts[1].ToLower();

            if (!int.TryParse(parts[2], out int id))
                return "ERROR|Некорректный ID";

            using (var db = new VotingDbContext())
            {
                switch (tableName)
                {
                    case "deputies":
                        db.Votes.RemoveRange(db.Votes.Where(v => v.Deputy == id));
                        var deputy = db.Deputies.Find(id);

                        if (deputy != null) db.Deputies.Remove(deputy);
                        break;

                    case "meetings":
                        var projects = db.Projects.Where(p => p.MeetingNumber == id);
                        foreach (var p in projects)
                            db.Votes.RemoveRange(db.Votes.Where(v => v.ProjectNumber == p.Id));
                            db.Projects.RemoveRange(projects);
                        var meeting = db.Meetings.Find(id);

                        if (meeting != null) db.Meetings.Remove(meeting);
                        break;

                    case "projects":
                        db.Votes.RemoveRange(db.Votes.Where(v => v.ProjectNumber == id));
                        var project = db.Projects.Find(id);

                        if (project != null) db.Projects.Remove(project);
                        break;

                    case "votes":
                        var vote = db.Votes.Find(id);
                        if (vote != null) db.Votes.Remove(vote);
                        break;

                    default:
                        return "ERROR|Неизвестная таблица";
                }
                db.SaveChanges();
            }
            return $"SUCCESS|Запись из {tableName} удалена (ORM)";
        }

        static string SearchORM(string[] parts)
        {
            string tableName = parts[1];
            string searchText = parts[2];

            using (var db = new VotingDbContext())
            {
                var results = new List<object>();

                switch (tableName.ToLower())
                {
                    case "deputies":
                        var deputies = db.Deputies
                            .Where(d => d.LastName.Contains(searchText) ||
                                        d.FirstName.Contains(searchText) ||
                                        (d.MiddleName != null && d.MiddleName.Contains(searchText)) ||
                                        d.District.Contains(searchText) ||
                                        d.Party.Contains(searchText) ||
                                        d.Status.Contains(searchText))
                            .ToList();
                        results.AddRange(deputies);
                        break;

                    case "meetings":
                        var meetings = db.Meetings
                            .Where(m => m.Date.ToString().Contains(searchText) ||
                                        m.StartTime.ToString().Contains(searchText) ||
                                        m.Type.Contains(searchText) ||
                                        m.Status.Contains(searchText))
                            .ToList();
                        results.AddRange(meetings);
                        break;

                    case "projects":
                        var projects = db.Projects
                            .Where(p => p.Title.Contains(searchText) ||
                                        p.Type.Contains(searchText) ||
                                        p.Status.Contains(searchText))
                            .ToList();
                        results.AddRange(projects);
                        break;

                    case "votes":
                        var votes = db.Votes
                            .Where(v => v.Result.Contains(searchText))
                            .ToList();
                        results.AddRange(votes);
                        break;

                    default:
                        return "ERROR|Неизвестная таблица";
                }

                var result = new StringBuilder("SEARCH_RESULT");

                if (results.Count > 0)
                {
                    var props = results[0].GetType().GetProperties();
                    var columnNames = props.Select(p => p.Name).ToList();
                    result.Append($"|{string.Join(",", columnNames)}");

                    foreach (var item in results)
                    {
                        var rowData = props.Select(p => p.GetValue(item)?.ToString() ?? "").ToList();
                        result.Append($"|{string.Join(",", rowData)}");
                    }
                }
                return result.ToString();
            }
        }
    }
}