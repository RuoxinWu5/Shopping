using Data.Model;

namespace Data.Repository
{
    public interface IUserRepository
    {
        Task AddUser(User user);
        Task<User?> GetUserById(int id);
        Task<User?> GetUserByName(string name);
    }
}