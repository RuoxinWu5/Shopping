using Data.Exceptions;
using Data.Model;
using Microsoft.EntityFrameworkCore;


namespace Data.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserContext _context;

        public UserRepository(UserContext context)
        {
            _context = context;
        }
        public async Task AddUser(User user)
        {
            throw new NotImplementedException();
        }
    }
}