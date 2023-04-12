using Microsoft.EntityFrameworkCore;

namespace Data.Model
{
    public class ShoppingDbContext : DbContext
    {
        public ShoppingDbContext(DbContextOptions<ShoppingDbContext> options)
            : base(options)
        {
        }
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<BuyerProduct> BuyerProducts { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
    }
}