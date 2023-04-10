using Data.Model;

namespace Service
{
    public interface IUserService
    {
        Task AddUser(User user);
    }
}