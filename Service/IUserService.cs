using Data.Model;

namespace Service
{
    public interface IUserService
    {
        Task AddUser(User user);
        Task<User> GetSellerById(int id);
        Task<User> GetBuyerById(int id);
        Task<User> GetUserById(int userId);
        Task ValidateIfSellerExist(int id);
        Task<User> GetUserByUserNameAndPassword(string username, string password);
    }
}