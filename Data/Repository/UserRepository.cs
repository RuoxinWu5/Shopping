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
            if (!user.type.HasValue)
            {
                user.type = UserType.BUYER;
            }
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.name == user.name);
            if (existingUser != null)
            {
                throw new DuplicateUserNameException($"User name '{user.name}' already exists.");
            }
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
    }
}