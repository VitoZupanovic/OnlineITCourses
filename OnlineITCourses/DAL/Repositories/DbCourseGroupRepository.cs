using DAL.Interfaces;
using DAL.Models;

namespace DAL.Repository
{
    public class DbCourseGroupRepository : ICourseGroupRepository
    {
        private readonly OnlineItcoursesContext _context;

        public DbCourseGroupRepository(OnlineItcoursesContext context)
        {
            _context = context;
        }

        public List<CourseGroup> RetrieveAll()
        {
            return _context.CourseGroups.ToList();
        }

        public CourseGroup? RetrieveById(int courseGroupId)
        {
            return _context.CourseGroups.FirstOrDefault(cg => cg.Id == courseGroupId);
        }

        public void Create(CourseGroup courseGroup)
        {
            _context.CourseGroups.Add(courseGroup);
            _context.SaveChanges();
        }

        public void Update(int courseGroupId, CourseGroup updatedCourseGroup)
        {
            var courseGroup = _context.CourseGroups.FirstOrDefault(cg => cg.Id == courseGroupId);

            if (courseGroup == null)
                return;

            courseGroup.Name = updatedCourseGroup.Name;
            courseGroup.Description = updatedCourseGroup.Description;

            _context.SaveChanges();
        }

        public void DeleteCourseGroup(int courseGroupId)
        {
            var courseGroup = _context.CourseGroups.FirstOrDefault(cg => cg.Id == courseGroupId);

            if (courseGroup == null)
                return;

            _context.CourseGroups.Remove(courseGroup);
            _context.SaveChanges();
        }

        public bool CourseGroupExists(int courseGroupId)
        {
            return _context.CourseGroups.Any(cg => cg.Id == courseGroupId);
        }

        public bool CanDeleteCourseGroup(int courseGroupId)
        {
            return !_context.Courses.Any(c => c.CourseGroupId == courseGroupId);
        }
    }
}
