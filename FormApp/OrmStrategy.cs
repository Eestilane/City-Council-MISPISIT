using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormApp
{
    public class OrmStrategy : IDataStrategy
    {
        private readonly ServerClient _server;

        public OrmStrategy(ServerClient server)
        {
            _server = server;
        }

        public async Task<string> AddProject(int meetingId, string title, string type, string status)
        {
            return await _server.SendCommand($"ADD_PROJECT_ORM|{meetingId}|{title}|{type}|{status}");
        }

        public async Task<string> UpdateRecord(string table, int id, string column, string value)
        {
            return await _server.SendCommand($"UPDATE_ORM|{table}|{id}|{column}|{value}");
        }

        public async Task<string> DeleteRecord(string table, int id)
        {
            return await _server.SendCommand($"DELETE_ORM|{table}|{id}");
        }

        public async Task<string> SearchRecord(string table, string text)
        {
            return await _server.SendCommand($"SEARCH_ORM|{table}|{text}");
        }
    }
}
