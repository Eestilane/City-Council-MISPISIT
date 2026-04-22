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

                //ORM
                case "LOGIN":
                    return Login(parts);
                case "GET_DEPUTIES":
                    return GetDeputies();
                case "GET_MEETINGS":
                    return GetMeetings();
                case "GET_PROJECTS":
                    return GetProjects();
                case "GET_VOTES":
                    return GetVotes();

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

        //ORM
        static string Login(string[] parts)
        {
            if (parts.Length < 3)
                return "ERROR|Недостаточно параметров. Формат: LOGIN|username|password";

            string username = parts[1];
            string password = parts[2];

            using (var db = new VotingDbContext())
            {
                bool exists = db.Users.Any(u => u.Username == username && u.Password == password);
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
    }
}
