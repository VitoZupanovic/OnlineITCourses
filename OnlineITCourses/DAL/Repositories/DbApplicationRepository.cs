using DAL.Interfaces;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository
{
    public class DbApplicationRepository : IApplicationRepository
    {
        private readonly OnlineItcoursesContext _context;

        public DbApplicationRepository(OnlineItcoursesContext context)
        {
            _context = context;
        }

        public List<Application> RetrieveAll()
        {
            return _context.Applications
                .Include(a => a.Course)
                .Include(a => a.User)
                .ToList();
        }

        public List<Application> RetrieveForCourse(int courseId)
        {
            return _context.Applications
                .Include(a => a.Course)
                .Include(a => a.User)
                .Where(a => a.CourseId == courseId)
                .ToList();
        }

        public List<Application> RetrieveForUser(int userId)
        {
            return _context.Applications
                .Include(a => a.Course)
                .Include(a => a.User)
                .Where(a => a.UserId == userId)
                .ToList();
        }

        public Application? RetrieveById(int applicationId)
        {
            return _context.Applications
                .Include(a => a.Course)
                .Include(a => a.User)
                .FirstOrDefault(a => a.Id == applicationId);
        }

        public User? RetrieveUserByUsername(string username)
        {
            return _context.Users.FirstOrDefault(u => u.Username == username);
        }
        public Course? RetrieveCourseById(int courseId)
        {
            return _context.Courses.FirstOrDefault(c => c.Id == courseId);
        }
        public void Create(Application application)
        {
            _context.Applications.Add(application);
            _context.SaveChanges();
        }

        public void UpdateStatus(int applicationId, string status)
        {
            var application = _context.Applications.FirstOrDefault(a => a.Id == applicationId);

            if (application == null)
                return;

            application.Status = status;

            _context.SaveChanges();
        }
        public void DeleteApplication(int applicationId)
        {
            var application = _context.Applications.FirstOrDefault(a => a.Id == applicationId);

            if (application == null)
                return;

            _context.Applications.Remove(application);
            _context.SaveChanges();
        }

        public bool ApplicationExists(int applicationId)
        {
            return _context.Applications.Any(a => a.Id == applicationId);
        }

        public bool CourseExists(int courseId)
        {
            return _context.Courses.Any(c => c.Id == courseId);
        }

        public bool UserExists(int userId)
        {
            return _context.Users.Any(u => u.Id == userId);
        }

        public bool UserAlreadyApplied(int userId, int courseId)
        {
            return _context.Applications.Any(a =>
                a.UserId == userId &&
                a.CourseId == courseId);
        }
    }
}
