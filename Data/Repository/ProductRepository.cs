using Data.Exceptions;
using Data.Model;
using Microsoft.EntityFrameworkCore;

namespace Data.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ShoppingDbContext _context;

        public ProductRepository(ShoppingDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetProductListBySellerId(int sellerId)
        {
            var result = await _context.Products.Include(p => p.User).Where(product => product.User.Id == sellerId).ToListAsync();
            return result;
        }

        public async Task AddProduct(Product product)
        {
            var existingProduct = await _context.Products.Include(p => p.User).FirstOrDefaultAsync(u => u.Name == product.Name);
            if (existingProduct != null)
            {
                throw new DuplicateUserNameException($"User name '{product.Name}' already exists.");
            }
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task<Product> GetProductById(int id)
        {
            var result = await _context.Products.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == id);
            if (result == null)
            {
                throw new KeyNotFoundException("The product doesn't exist.");
            }
            else
            {
                return result;
            }
        }
    }
}