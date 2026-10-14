using DAL.Models;

namespace DAL.Interfaces
{
    public interface ILogRepository
    {
        void AddLog(AppLogLevel level, string message, string? errorText = null);
        IEnumerable<Log> GetLast(int n);
        int Count();
    }

}
