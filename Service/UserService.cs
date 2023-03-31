using Data.Model;
using Data.Repository;
using Data.Exceptions;

namespace Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<string> AddUser(User user)
        {
            try{
                await _repository.AddUser(user);
                return "Registered successfully";
            }
            catch(DuplicateUserNameException exception){
                return exception.Message;
            }
            catch(ArgumentException exception){
                return exception.Message;
            }
        }
    }
}