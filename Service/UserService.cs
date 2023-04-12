using Data.Model;
using Data.Repository;

namespace Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task AddUser(User user)
        {
            await _repository.AddUser(user);
        }

        public async Task<User> GetSellerById(int id)
        {
            var result = await _repository.GetUserById(id);
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
            var result = await _repository.GetUserById(id);
            if (result.Type == UserType.BUYER)
            {
                throw new KeyNotFoundException("The buyer doesn't exist.");
            }
            else
            {
                return result;
            }
        }
    }
}