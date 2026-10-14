using DAL.Interfaces;
using DAL.Models;

namespace DAL.Repository
{
    public class DbLogRepository : ILogRepository
    {
        private readonly OnlineItcoursesContext _context;

        public DbLogRepository(OnlineItcoursesContext context)
        {
            _context = context;
        }

        public void AddLog(AppLogLevel level, string message, string? errorText = null)
        {
            try
            {
                var log = new Log
                {
                    Timestamp = DateTime.Now,
                    Level = (int)level,
                    Message = message,
                    ErrorText = errorText
                };

                _context.Logs.Add(log);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Logging failed: {ex.Message}");
            }
        }

        public IEnumerable<Log> GetLast(int n)
        {
            return _context.Logs
                .OrderByDescending(l => l.Timestamp)
                .Take(n)
                .ToList();
        }

        public int Count()
        {
            return _context.Logs.Count();
        }
    }
}
