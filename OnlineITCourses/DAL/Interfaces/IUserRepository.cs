using DAL.Models;

namespace DAL.Interfaces
{
    public interface IUserRepository
    {
        User? RetrieveByUsername(string username);

        bool UsernameExists(string username);

        void Create(User user);

        void Update(User user);
    }
}
