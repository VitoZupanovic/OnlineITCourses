using DAL.Interfaces;
using DAL.Models;

namespace DAL.Repository
{
    public class DbInstructorRepository : IInstructorRepository
    {
        private readonly OnlineItcoursesContext _context;

        public DbInstructorRepository(OnlineItcoursesContext context)
        {
            _context = context;
        }

        public List<Instructor> RetrieveAll()
        {
            return _context.Instructors.ToList();
        }

        public Instructor? RetrieveById(int instructorId)
        {
            return _context.Instructors.FirstOrDefault(i => i.Id == instructorId);
        }

        public void Create(Instructor instructor)
        {
            _context.Instructors.Add(instructor);
            _context.SaveChanges();
        }

        public void Update(int instructorId, Instructor updatedInstructor)
        {
            var instructor = _context.Instructors.FirstOrDefault(i => i.Id == instructorId);

            if (instructor == null)
                return;

            instructor.Name = updatedInstructor.Name;
            instructor.Email = updatedInstructor.Email;
            instructor.Biography = updatedInstructor.Biography;

            _context.SaveChanges();
        }

        public void DeleteInstructor(int instructorId)
        {
            var instructor = _context.Instructors.FirstOrDefault(i => i.Id == instructorId);

            if (instructor == null)
                return;

            _context.Instructors.Remove(instructor);
            _context.SaveChanges();
        }

        public bool InstructorExists(int instructorId)
        {
            return _context.Instructors.Any(i => i.Id == instructorId);
        }

        public bool CanDeleteInstructor(int instructorId)
        {
            return !_context.CourseInstructors.Any(ci => ci.InstructorId == instructorId);
        }
    }
}
