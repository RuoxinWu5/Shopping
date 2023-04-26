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
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetUserById(int id)
        {
            var result = await _context.Users.Include(p => p.Products).FirstOrDefaultAsync(u => u.Id == id);
            return result;
        }

        public async Task<User?> GetUserByName(string name)
        {
            var result = await _context.Users.Include(p => p.Products).FirstOrDefaultAsync(u => u.Name == name);
            return result;
        }
    }
}