using Data.Exceptions;
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

        public async Task<User> AddUser(User user)
        {
            if (!user.Type.HasValue)
            {
                user.Type = UserType.BUYER;
            }
            try
            {
                var existingUser = await _userRepository.GetUserByName(user.Name);
                if (existingUser != null)
                {
                    throw new DuplicateUserNameException($"User name '{user.Name}' already exists.");
                }
            }
            catch (KeyNotFoundException)
            {
                await _userRepository.AddUser(user);
            }
            return user;
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

        public async Task<User> GetUserByUserNameAndPassword(string username, string password)
        {
            var user = await _userRepository.GetUserByName(username);
            if (user.Password != password)
            {
                throw new KeyNotFoundException("The user doesn't exist.");
            }
            return user;
        }
    }
}