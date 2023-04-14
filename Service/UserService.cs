using Data.Model;
using Data.Repository;

namespace Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository repository)
        {
            _userRepository = repository;
        }

        public async Task AddUser(User user)
        {
            await _userRepository.AddUser(user);
        }

        public async Task<User> GetSellerById(int id)
        {
            var result = await _userRepository.GetUserById(id);
            if (result.Type == UserType.BUYER)
            {
                throw new KeyNotFoundException("The seller doesn't exist.");
            }
            else
            {
                return result;
            }
        }

        public async Task<User> GetBuyerById(int id)
        {
            var result = await _userRepository.GetUserById(id);
            if (result.Type == UserType.SELLER)
            {
                throw new KeyNotFoundException("The buyer doesn't exist.");
            }
            else
            {
                return result;
            }
        }

        public async Task<User> GetUserById(int userId)
        {
            return await _userRepository.GetUserById(userId);
        }
    }
}