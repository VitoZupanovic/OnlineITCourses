using DAL.Interfaces;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository
{
    public class DbCourseRepository : ICourseRepository
    {
        private readonly OnlineItcoursesContext _context;

        public DbCourseRepository(OnlineItcoursesContext context)
        {
            _context = context;
        }

        public List<Course> RetrieveAll()
        {
            return _context.Courses.Include(c => c.CourseGroup).ToList();
        }

        public Course? RetrieveById(int courseId)
        {
            return _context.Courses.Include(c => c.CourseGroup).FirstOrDefault(c => c.Id == courseId);
        }

        public List<Course> Search(string? text, int page, int count)
        {
            var query = _context.Courses.Include(c => c.CourseGroup).AsQueryable();

            if (!string.IsNullOrWhiteSpace(text))
            {
                query = query.Where(c =>
                    c.Name.Contains(text) ||
                    c.Description.Contains(text) ||
                    c.Level.Contains(text));
            }

            return query
                .OrderBy(c => c.Id)
                .Skip((page - 1) * count)
                .Take(count)
                .ToList();
        }

        public List<CourseInstructor> RetrieveInstructorsForCourse(int courseId)
        {
            return _context.CourseInstructors
                .Include(ci => ci.Instructor)
                .Where(ci => ci.CourseId == courseId)
                .ToList();
        }

        public void Create(Course course)
        {
            _context.Courses.Add(course);
            _context.SaveChanges();
        }

        public void Update(int courseId, Course updatedCourse)
        {
            var course = _context.Courses.FirstOrDefault(c => c.Id == courseId);

            if (course == null)
                return;

            course.Name = updatedCourse.Name;
            course.Description = updatedCourse.Description;
            course.DurationHours = updatedCourse.DurationHours;
            course.Level = updatedCourse.Level;
            course.Price = updatedCourse.Price;
            course.CourseGroupId = updatedCourse.CourseGroupId;

            _context.SaveChanges();
        }

        public void Delete(int courseId)
        {
            var course = _context.Courses.FirstOrDefault(c => c.Id == courseId);

            if (course == null)
                return;

            _context.Courses.Remove(course);
            _context.SaveChanges();
        }

        public void AddInstructorToCourse(int courseId, int instructorId)
        {
            var entry = new CourseInstructor
            {
                CourseId = courseId,
                InstructorId = instructorId
            };

            _context.CourseInstructors.Add(entry);
            _context.SaveChanges();
        }

        public void DeleteCourseInstructorEntry(int courseId, int instructorId)
        {
            var entry = _context.CourseInstructors
                .FirstOrDefault(ci => ci.CourseId == courseId && ci.InstructorId == instructorId);

            if (entry == null)
                return;

            _context.CourseInstructors.Remove(entry);
            _context.SaveChanges();
        }
        public bool CourseExists(int courseId)
        {
            return _context.Courses.Any(c => c.Id == courseId);
        }

        public bool InstructorExists(int instructorId)
        {
            return _context.Instructors.Any(i => i.Id == instructorId);
        }

        public bool CourseInstructorExists(int courseId, int instructorId)
        {
            return _context.CourseInstructors
                .Any(ci => ci.CourseId == courseId && ci.InstructorId == instructorId);
        }

        public bool CanDeleteCourse(int courseId)
        {
            bool hasInstructors = _context.CourseInstructors
                .Any(ci => ci.CourseId == courseId);

            bool hasApplications = _context.Applications
                .Any(a => a.CourseId == courseId);

            return !hasInstructors && !hasApplications;
        }
    }
}
