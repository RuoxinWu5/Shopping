using Data.Model;

namespace Service
{
    public interface IUserService
    {
        Task<HttpResponseMessage> AddUser(User user);
    }
}