using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FormApp
{
    public interface IDataStrategy
    {
        Task<string> AddProject(int meetingId, string title, string type, string status);
        Task<string> UpdateRecord(string table, int id, string column, string value);
        Task<string> DeleteRecord(string table, int id);
        Task<string> SearchRecord(string table, string text);
    }
}