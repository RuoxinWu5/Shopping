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
            var existingUser = await _userRepository.GetUserByName(user.Name);
            if (existingUser == null)
            {
                await _userRepository.AddUser(user);
                return user;
            }
            throw new DuplicateUserNameException($"User name '{user.Name}' already exists.");
        }

        public async Task<User> GetSellerById(int id)
        {
            var user = await _userRepository.GetUserById(id);
            if (user != null && user.Type == UserType.SELLER)
                return user;
            throw new SellerNotFoundException("The seller doesn't exist.");
        }

        public async Task<User> GetBuyerById(int id)
        {
            var user = await _userRepository.GetUserById(id);
            if (user != null && user.Type == UserType.BUYER)
                return user;
            throw new BuyerNotFoundException("The buyer doesn't exist.");
        }

        public async Task<User> GetUserById(int userId)
        {
            var user = await _userRepository.GetUserById(userId);
            if (user != null)
                return user;
            throw new UserNotFoundException("The user doesn't exist.");
        }

        public async Task ValidateIfSellerExist(int id)
        {
            var user = await _userRepository.GetUserById(id);
            if (user == null || user.Type != UserType.SELLER)
                throw new UserNotFoundException("The user doesn't exist.");
        }

        public async Task<User> GetUserByUserNameAndPassword(string username, string password)
        {
            var user = await _userRepository.GetUserByName(username);
            if (user == null || user.Password != password)
                throw new UserNotFoundException("The user doesn't exist.");
            return user;
        }
    }
}