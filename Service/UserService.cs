using Data.Model;
using Data.Repository;
using Data.Exceptions;
using System.Net;

namespace Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<HttpResponseMessage> AddUser(User user)
        {
            try
            {
                await _repository.AddUser(user);
                return new HttpResponseMessage(HttpStatusCode.Created);
            }
            catch (DuplicateUserNameException exception)
            {
                return new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    Content = new StringContent(exception.Message.ToString())
                };
            }
        }
    }
}