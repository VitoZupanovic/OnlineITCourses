using DAL.Models;

namespace DAL.Interfaces
{
    public interface IApplicationRepository
    {
        List<Application> RetrieveAll();

        List<Application> RetrieveForCourse(int courseId);

        List<Application> RetrieveForUser(int userId);

        Application? RetrieveById(int applicationId);
        User? RetrieveUserByUsername(string username);
        Course? RetrieveCourseById(int courseId);

        void Create(Application application);
        void UpdateStatus(int applicationId, string status);

        void DeleteApplication(int applicationId);

        bool ApplicationExists(int applicationId);

        bool CourseExists(int courseId);

        bool UserExists(int userId);

        bool UserAlreadyApplied(int userId, int courseId);
    }
}
