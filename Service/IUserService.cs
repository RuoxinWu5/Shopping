using Data.Model;

namespace Service
{
    public interface IUserService
    {
        Task<string> AddUser(User user);
    }
}