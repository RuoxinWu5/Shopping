using Data.Model;

namespace Data.Repository
{
    public interface IUserRepository
    {
        Task AddUser(User user);
    }
}