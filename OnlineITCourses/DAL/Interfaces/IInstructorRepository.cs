using DAL.Models;

namespace DAL.Interfaces
{
    public interface IInstructorRepository
    {
        List<Instructor> RetrieveAll();

        Instructor? RetrieveById(int instructorId);

        void Create(Instructor instructor);

        void Update(int instructorId, Instructor instructor);

        void DeleteInstructor(int instructorId);

        bool InstructorExists(int instructorId);

        bool CanDeleteInstructor(int instructorId);
    }
}
