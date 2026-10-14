using DAL.Interfaces;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository
{
    public class DbUserRepository : IUserRepository
    {
        private readonly OnlineItcoursesContext _context;

        public DbUserRepository(OnlineItcoursesContext context)
        {
            _context = context;
        }

        public User? RetrieveByUsername(string username)
        {
            return _context.Users.Include(u => u.Role).FirstOrDefault(u => u.Username == username);
        }

        public bool UsernameExists(string username)
        {
            return _context.Users.Any(u => u.Username == username);
        }

        public void Create(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public void Update(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }
    }
}
