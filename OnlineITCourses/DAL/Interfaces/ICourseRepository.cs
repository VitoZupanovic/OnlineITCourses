using DAL.Models;

namespace DAL.Interfaces
{
    public interface ICourseRepository
    {
        List<Course> RetrieveAll();

        Course? RetrieveById(int courseId);

        List<Course> Search(string? text, int page, int count);

        List<CourseInstructor> RetrieveInstructorsForCourse(int courseId);

        void Create(Course course);

        void Update(int courseId, Course course);

        void Delete(int courseId);

        void AddInstructorToCourse(int courseId, int instructorId);

        void DeleteCourseInstructorEntry(int courseId, int instructorId);

        bool CourseExists(int courseId);

        bool InstructorExists(int instructorId);

        bool CourseInstructorExists(int courseId, int instructorId);

        bool CanDeleteCourse(int courseId);
    }
}
