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
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Name == user.Name);
            if (existingUser != null)
            {
                throw new DuplicateUserNameException($"User name '{user.Name}' already exists.");
            }
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
        public User GetUserById(int id)
        {
            return _context.Users.SingleOrDefault(u => u.Id == id) ?? throw new ArgumentException("User not found");
        }
    }
}