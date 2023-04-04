using Microsoft.EntityFrameworkCore;

namespace Data.Model
{
    public class BuyerProductContext : DbContext
    {
        public BuyerProductContext(DbContextOptions<BuyerProductContext> options) : base(options)
        {
        }
        public DbSet<BuyerProduct>? BuyerProducts { get; set; }
    }
}