using Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace Data.Model
{
    public class ProductContext : DbContext
    {
        public ProductContext(DbContextOptions<ProductContext> options) : base(options)
        {
        }
        public DbSet<Product>? Products { get; set; }

        public static implicit operator ProductContext(ProductRepository v)
        {
            throw new NotImplementedException();
        }
    }
}