using Data.Exceptions;
using Data.Model;
using Microsoft.EntityFrameworkCore;


namespace Data.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ShoppingDbContext _context;

        public UserRepository(ShoppingDbContext context)
        {
            _context = context;
        }
        public async Task AddUser(User user)
        {
            if (!user.Type.HasValue)
            {
                user.Type = UserType.BUYER;
            }
            var existingUser = await _context.Users.Include(p => p.Products).FirstOrDefaultAsync(u => u.Name == user.Name);
            if (existingUser != null)
            {
                throw new DuplicateUserNameException($"User name '{user.Name}' already exists.");
            }
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User> GetUserById(int id)
        {
            var result = await _context.Users.Include(p => p.Products).FirstOrDefaultAsync(u => u.Id == id);
            if (result == null)
            {
                throw new KeyNotFoundException("The user doesn't exist.");
            }
            else
            {
                return result;
            }
        }
    }
}