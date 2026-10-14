using DAL.Models;

namespace DAL.Interfaces
{
    public interface ICourseGroupRepository
    {
        List<CourseGroup> RetrieveAll();

        CourseGroup? RetrieveById(int courseGroupId);

        void Create(CourseGroup courseGroup);

        void Update(int courseGroupId, CourseGroup courseGroup);

        void DeleteCourseGroup(int courseGroupId);

        bool CourseGroupExists(int courseGroupId);
        bool CanDeleteCourseGroup(int courseGroupId);
    }
}
