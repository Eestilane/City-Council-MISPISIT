using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FormApp
{
    public class ServerClient
    {
        private TcpClient client;
        private NetworkStream stream;
        public bool IsConnected { get; private set; }

        public async Task<bool> ConnectAsync()
        {
            try
            {
                client = new TcpClient();
                await client.ConnectAsync("127.0.0.1", 8888);
                stream = client.GetStream();
                IsConnected = true;
                return true;
            }
            catch
            {
                IsConnected = false;
                return false;
            }
        }

        public async Task<string> SendCommand(string command)
        {
            if (!IsConnected)
                return "Ошибка: нет подключения к серверу";

            try
            {
                byte[] data = Encoding.UTF8.GetBytes(command + "\n");
                await stream.WriteAsync(data, 0, data.Length);

                using (var ms = new MemoryStream())
                {
                    byte[] buffer = new byte[65536];
                    int bytesRead;

                    do
                    {
                        bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                        ms.Write(buffer, 0, bytesRead);
                    }
                    while (stream.DataAvailable);

                    return Encoding.UTF8.GetString(ms.ToArray()).Trim();
                }
            }
            catch (Exception ex)
            {
                return $"Ошибка: {ex.Message}";
            }
        }

        public void Disconnect()
        {
            stream?.Close();
            client?.Close();
            IsConnected = false;
        }
    }
}
