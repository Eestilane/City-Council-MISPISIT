using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace VotingSystem.Tests
{
    public class EndToEndTests
    {
        [Fact]
        public async Task EtE_Login()
        {
            using var client = new TcpClient();

            await client.ConnectAsync("127.0.0.1", 8888);
            var stream = client.GetStream();

            var command = Encoding.UTF8.GetBytes("LOGIN|test|123\n");
            await stream.WriteAsync(command, 0, command.Length);

            var buffer = new byte[1024];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            var response = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

            Assert.Equal("SUCCESS", response);
        }

        [Fact]
        public async Task EtE_CreateProject()
        {
            using var client = new TcpClient();
            await client.ConnectAsync("127.0.0.1", 8888);
            var stream = client.GetStream();
            var buffer = new byte[4096];

            var loginCommand = Encoding.UTF8.GetBytes("LOGIN|test|123\n");
            await stream.WriteAsync(loginCommand, 0, loginCommand.Length);
            int loginBytes = await stream.ReadAsync(buffer, 0, buffer.Length);
            var loginResponse = Encoding.UTF8.GetString(buffer, 0, loginBytes).Trim();

            Assert.Equal("SUCCESS", loginResponse);

            var addCommand = Encoding.UTF8.GetBytes("ADD_PROJECT_ORM|1|Тестовый проект|Закон|Внесён\n");
            await stream.WriteAsync(addCommand, 0, addCommand.Length);
            int addBytes = await stream.ReadAsync(buffer, 0, buffer.Length);
            var addResponse = Encoding.UTF8.GetString(buffer, 0, addBytes).Trim();

            Assert.StartsWith("SUCCESS", addResponse);

            var getCommand = Encoding.UTF8.GetBytes("GET_PROJECTS\n");
            await stream.WriteAsync(getCommand, 0, getCommand.Length);
            int getBytes = await stream.ReadAsync(buffer, 0, buffer.Length);
            var getResponse = Encoding.UTF8.GetString(buffer, 0, getBytes).Trim();

            Assert.StartsWith("PROJECTS", getResponse);
            Assert.Contains("Тестовый проект", getResponse);
        }
    }
}