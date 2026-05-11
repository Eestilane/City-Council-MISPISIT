using Microsoft.EntityFrameworkCore;
using Server;
using System;
using System.Linq;
using Xunit;
using static Server.Program;

namespace VotingSystem.Tests
{
    public class CommandProcessorTests : IDisposable
    {
        private readonly VotingDbContext _context;
        private readonly CommandProcessor _processor;

        public CommandProcessorTests()
        {
            var options = new DbContextOptionsBuilder<VotingDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new VotingDbContext(options);
            _processor = new CommandProcessor(_context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public void Login()
        {
            _context.Deputies.Add(new Deputy
            {
                Login = "testuser",
                Password = "12345",
                LastName = "Test",
                FirstName = "User"
            });
            _context.SaveChanges();

            var parts = new[] { "LOGIN", "testuser", "12345" };
            var result = _processor.Login(parts);

            Assert.Equal("SUCCESS", result);
        }

        [Fact]
        public void Login_PasswordErorr()
        {
            _context.Deputies.Add(new Deputy { Login = "testuser", Password = "12345" });
            _context.SaveChanges();
            var parts = new[] { "LOGIN", "testuser", "wrong" };
            var result = _processor.Login(parts);

            Assert.Equal("ERROR|Неверный логин или пароль", result);
        }

        [Fact]
        public void Login_UserDoesNotExist()
        {
            var parts = new[] { "LOGIN", "nonexistent", "pass" };
            var result = _processor.Login(parts);

            Assert.Equal("ERROR|Неверный логин или пароль", result);
        }

        [Fact]
        public void RegisterDeputy()
        {
            var parts = new[] { "REGISTER_DEPUTY", "newuser", "pass123", "Ivanov", "Ivan", "Ivanovich", "District 1", "Party A" };
            var result = _processor.RegisterDeputy(parts);

            Assert.Equal("SUCCESS|Депутат зарегистрирован", result);

            var userInDb = _context.Deputies.FirstOrDefault(d => d.Login == "newuser");
            Assert.NotNull(userInDb);
            Assert.Equal("Ivanov", userInDb.LastName);
        }

        [Fact]
        public void RegisterDeputy_LoginExists()
        {
            _context.Deputies.Add(new Deputy { Login = "existing", Password = "123" });
            _context.SaveChanges();

            var parts = new[] { "REGISTER_DEPUTY", "existing", "pass", "Last", "First", "", "District", "Party" };
            var result = _processor.RegisterDeputy(parts);

            Assert.Equal("ERROR|Депутат с таким логином уже существует", result);
        }

        [Fact]
        public void GetDeputies()
        {
            _context.Deputies.Add(new Deputy
            {
                Id = 1,
                LastName = "A",
                FirstName = "B",
                District = "X",
                Party = "Y",
                Status = "Active"
            });
            _context.Deputies.Add(new Deputy
            {
                Id = 2,
                LastName = "C",
                FirstName = "D",
                District = "X",
                Party = "Y",
                Status = "Active"
            });
            _context.SaveChanges();

            var result = _processor.GetDeputies();

            Assert.StartsWith("DEPUTIES", result);
            Assert.Contains("|1|A|B||X|Y|Active", result);
            Assert.Contains("|2|C|D||X|Y|Active", result);
        }
    }
}