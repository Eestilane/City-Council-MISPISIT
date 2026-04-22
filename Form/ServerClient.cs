using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Forms
{
    public class ServerClient
    {
        private TcpClient client;
        private NetworkStream stream;

        public async Task<string> SendCommand(string command)
        {
            try
            {
                // Подключаемся к серверу
                client = new TcpClient();
                await client.ConnectAsync("127.0.0.1", 8888);

                // Отправляем команду
                stream = client.GetStream();
                byte[] data = Encoding.UTF8.GetBytes(command);
                await stream.WriteAsync(data, 0, data.Length);

                // Получаем ответ
                byte[] buffer = new byte[1024];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                // Закрываем соединение
                stream.Close();
                client.Close();

                return response;
            }
            catch (Exception ex)
            {
                return $"Ошибка: {ex.Message}";
            }
        }
    }
}
