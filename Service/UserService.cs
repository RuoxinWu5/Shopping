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
    }
}