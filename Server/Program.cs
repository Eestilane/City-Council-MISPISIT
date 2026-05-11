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
using Org.BouncyCastle.Crypto.Generators;
using Server;

namespace Server
{
    public class Program
    {
        private static string connectionString;
        private static CommandProcessor _sqlProcessor;
        private static CommandProcessor _ormProcessor;

        static void Main(string[] args)
        {
            connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            _sqlProcessor = new CommandProcessor(connectionString);
            _ormProcessor = new CommandProcessor(new VotingDbContext());

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

        public static void RefreshORMCache()
        {
            var dbContext = _ormProcessor.GetDbContext();
            if (dbContext != null)
            {
                var entries = dbContext.ChangeTracker.Entries().ToList();

                foreach (var entry in entries)
                {
                    entry.State = EntityState.Detached;
                }
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
                    return _sqlProcessor.AddProjectSQL(parts);
                case "UPDATE_SQL":
                    return _sqlProcessor.UpdateRecordSQL(parts);
                case "DELETE_SQL":
                    return _sqlProcessor.DeleteRecordSQL(parts);
                case "SEARCH_SQL":
                    return _sqlProcessor.SearchSQL(parts);

                //ORM
                case "LOGIN":
                    return _ormProcessor.Login(parts);
                case "REGISTER_DEPUTY":
                    return _ormProcessor.RegisterDeputy(parts);
                case "GET_DEPUTIES":
                    return _ormProcessor.GetDeputies();
                case "GET_MEETINGS":
                    return _ormProcessor.GetMeetings();      
                case "GET_PROJECTS":
                    return _ormProcessor.GetProjects();      
                case "GET_VOTES":
                    return _ormProcessor.GetVotes();         
                case "ADD_PROJECT_ORM":
                    return _ormProcessor.AddProjectORM(parts);
                case "UPDATE_ORM":
                    return _ormProcessor.UpdateRecordORM(parts);
                case "DELETE_ORM":
                    return _ormProcessor.DeleteRecordORM(parts);
                case "SEARCH_ORM":
                    return _ormProcessor.SearchORM(parts);

                //Тест
                case "TEST":
                    return "OK|Команда получена";

                default:
                    return "ERROR|Неизвестная команда";
            }
        }

        public class CommandProcessor
        {
            private readonly VotingDbContext _dbContext;
            private readonly string _connectionString;

            public CommandProcessor(VotingDbContext dbContext)
            {
                _dbContext = dbContext;
            }

            public CommandProcessor(string connectionString)
            {
                _connectionString = connectionString;
            }

            public VotingDbContext GetDbContext()
            {
                return _dbContext;
            }

            //SQL
            public string AddProjectSQL(string[] parts)
            {
                int meetingId = int.Parse(parts[1]);
                string title = parts[2];
                string type = parts[3];
                string status = parts[4];

                using (SqlConnection conn = new SqlConnection(_connectionString))
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

            public string UpdateRecordSQL(string[] parts)
            {
                string tableName = parts[1].ToLower();

                if (!int.TryParse(parts[2], out int id))
                    return "ERROR|Некорректный ID";

                string columnName = parts[3];
                string newValue = parts[4];
                newValue = newValue.Replace("'", "''");

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string sql = $"UPDATE {tableName} SET {columnName} = N'{newValue}' WHERE Id = {id}";

                    try
                    {
                        int rows = new SqlCommand(sql, conn).ExecuteNonQuery();
                        if (rows > 0)
                        {
                            Program.RefreshORMCache();
                            return "SUCCESS|Запись обновлена (SQL)";
                        }

                        return "ERROR|Ничего не обновлено";
                    }
                    catch (Exception ex)
                    {
                        return $"ERROR|{ex.Message}";
                    }
                }
            }

            public string DeleteRecordSQL(string[] parts)
            {
                string tableName = parts[1].ToLower();

                if (!int.TryParse(parts[2], out int id))
                    return "ERROR|Некорректный ID";

                using (SqlConnection conn = new SqlConnection(_connectionString))
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

            public string SearchSQL(string[] parts)
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

                using (SqlConnection conn = new SqlConnection(_connectionString))
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

            public List<string> GetTableColumns(string tableName)
            {
                var columns = new List<string>();
                string sql = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'{tableName}' AND COLUMN_NAME != 'Id'";

                using (SqlConnection conn = new SqlConnection(_connectionString))
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
            public string Login(string[] parts)
            {
                if (parts.Length < 3)
                    return "ERROR|Недостаточно параметров. Формат: LOGIN|login|password";

                string login = parts[1];
                string password = parts[2];

                var user = _dbContext.Deputies.FirstOrDefault(d => d.Login == login);
                if (user == null)
                    return "ERROR|Неверный логин или пароль";

                bool isValid = BCrypt.Net.BCrypt.Verify(password, user.Password);

                return isValid ? "SUCCESS" : "ERROR|Неверный логин или пароль";
            }

            public string GetDeputies()
            {
                var deputies = _dbContext.Deputies.OrderBy(d => d.LastName).ToList();
                var result = new StringBuilder("DEPUTIES");

                foreach (var d in deputies)
                {
                    result.Append($"|{d.Id}|{d.LastName}|{d.FirstName}|{d.MiddleName}|{d.District}|{d.Party}|{d.Status}");
                }
                return result.ToString();
            }

            public string GetMeetings()
            {
                var meetings = _dbContext.Meetings.OrderBy(m => m.Date).ThenBy(m => m.StartTime).ToList();
                var result = new StringBuilder("MEETINGS");

                foreach (var m in meetings)
                {
                    result.Append($"|{m.Id}|{m.Date:yyyy-MM-dd}|{m.StartTime}|{m.Type}|{m.Status}");
                }
                return result.ToString();
            }

            public string GetProjects()
            {
                var projects = _dbContext.Projects.Include(p => p.Meeting).OrderBy(p => p.Id).ToList();
                var result = new StringBuilder("PROJECTS");

                foreach (var p in projects)
                {
                    result.Append($"|{p.Id}|{p.MeetingNumber}|{p.Title}|{p.Type}|{p.Status}");
                }
                return result.ToString();
            }

            public string GetVotes()
            {
                var votes = _dbContext.Votes.Include(v => v.Project).Include(v => v.DeputyNavigation).OrderBy(v => v.Id).ToList();
                var result = new StringBuilder("VOTES");

                foreach (var v in votes)
                {
                    result.Append($"|{v.Id}|{v.ProjectNumber}|{v.Deputy}|{v.Result}");
                }
                return result.ToString();
            }

            public string RegisterDeputy(string[] parts)
            {
                string login = parts[1];
                string password = parts[2];
                string lastName = parts[3];
                string firstName = parts[4];
                string middleName = string.IsNullOrEmpty(parts[5]) ? null : parts[5];
                string district = parts[6];
                string party = parts[7];

                if (_dbContext.Deputies.Any(d => d.Login == login))
                    return "ERROR|Депутат с таким логином уже существует";

                var newDeputy = new Deputy
                {
                    Login = login,
                    Password = BCrypt.Net.BCrypt.HashPassword(password),
                    LastName = lastName,
                    FirstName = firstName,
                    MiddleName = middleName,
                    District = district,
                    Party = party,
                    Status = "Действующий",
                    Role = "Депутат"
                };

                _dbContext.Deputies.Add(newDeputy);
                _dbContext.SaveChanges();

                return "SUCCESS|Депутат зарегистрирован";
            }

            public string AddProjectORM(string[] parts)
            {
                int meetingId = int.Parse(parts[1]);
                string title = parts[2];
                string type = parts[3];
                string status = parts[4];

                _dbContext.Projects.Add(new Project { MeetingNumber = meetingId, Title = title, Type = type, Status = status });
                _dbContext.SaveChanges();

                return "SUCCESS|Проект добавлен (ORM)";
            }

            public string UpdateRecordORM(string[] parts)
            {
                string tableName = parts[1].ToLower();

                if (!int.TryParse(parts[2], out int id))
                    return "ERROR|Некорректный ID";

                string columnName = parts[3];
                string newValue = parts[4];

                switch (tableName)
                {
                    case "deputies":
                        var deputy = _dbContext.Deputies.Find(id);
                        if (deputy == null) return "ERROR|Запись не найдена";
                        SetPropertyValue(deputy, columnName, newValue);
                        break;
                    case "meetings":
                        var meeting = _dbContext.Meetings.Find(id);
                        if (meeting == null) return "ERROR|Запись не найдена";
                        SetPropertyValue(meeting, columnName, newValue);
                        break;
                    case "projects":
                        var project = _dbContext.Projects.Find(id);
                        if (project == null) return "ERROR|Запись не найдена";
                        SetPropertyValue(project, columnName, newValue);
                        break;
                    case "votes":
                        var vote = _dbContext.Votes.Find(id);
                        if (vote == null) return "ERROR|Запись не найдена";
                        SetPropertyValue(vote, columnName, newValue);
                        break;
                    default:
                        return "ERROR|Неизвестная таблица";
                }

                _dbContext.SaveChanges();
                return $"SUCCESS|Запись из {tableName} обновлена (ORM)";
            }

            public static void SetPropertyValue(object obj, string propertyName, string value)
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

            public string DeleteRecordORM(string[] parts)
            {
                string tableName = parts[1].ToLower();

                if (!int.TryParse(parts[2], out int id))
                    return "ERROR|Некорректный ID";

                switch (tableName)
                {
                    case "deputies":
                        _dbContext.Votes.RemoveRange(_dbContext.Votes.Where(v => v.Deputy == id));
                        var deputy = _dbContext.Deputies.Find(id);
                        if (deputy != null) _dbContext.Deputies.Remove(deputy);
                        break;
                    case "meetings":
                        var projects = _dbContext.Projects.Where(p => p.MeetingNumber == id);
                        foreach (var p in projects)
                            _dbContext.Votes.RemoveRange(_dbContext.Votes.Where(v => v.ProjectNumber == p.Id));
                        _dbContext.Projects.RemoveRange(projects);
                        var meeting = _dbContext.Meetings.Find(id);
                        if (meeting != null) _dbContext.Meetings.Remove(meeting);
                        break;
                    case "projects":
                        _dbContext.Votes.RemoveRange(_dbContext.Votes.Where(v => v.ProjectNumber == id));
                        var project = _dbContext.Projects.Find(id);
                        if (project != null) _dbContext.Projects.Remove(project);
                        break;
                    case "votes":
                        var vote = _dbContext.Votes.Find(id);
                        if (vote != null) _dbContext.Votes.Remove(vote);
                        break;
                    default:
                        return "ERROR|Неизвестная таблица";
                }
                _dbContext.SaveChanges();
                return $"SUCCESS|Запись из {tableName} удалена (ORM)";
            }

            public string SearchORM(string[] parts)
            {
                string tableName = parts[1];
                string searchText = parts[2];
                var results = new List<object>();

                switch (tableName.ToLower())
                {
                    case "deputies":
                        var deputies = _dbContext.Deputies
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
                        var meetings = _dbContext.Meetings
                            .Where(m => m.Date.ToString().Contains(searchText) ||
                                        m.StartTime.ToString().Contains(searchText) ||
                                        m.Type.Contains(searchText) ||
                                        m.Status.Contains(searchText))
                            .ToList();
                        results.AddRange(meetings);
                        break;
                    case "projects":
                        var projects = _dbContext.Projects
                            .Where(p => p.Title.Contains(searchText) ||
                                        p.Type.Contains(searchText) ||
                                        p.Status.Contains(searchText))
                            .ToList();
                        results.AddRange(projects);
                        break;
                    case "votes":
                        var votes = _dbContext.Votes
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

        // Отдельный метод
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
    }
}