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
            var result = await _context.Products.Where(product => product.SellerId == sellerId).ToListAsync();
            return result;
        }

        public async Task AddProduct(Product product)
        {
            var existingProduct = await _context.Products.FirstOrDefaultAsync(u => u.Name == product.Name);
            if (existingProduct != null)
            {
                throw new DuplicateUserNameException($"User name '{product.Name}' already exists.");
            }
            var existingSeller = await _context.Users.FirstOrDefaultAsync(u => u.Id == product.SellerId);
            if (existingSeller == null || existingSeller.Type == UserType.BUYER)
            {
                throw new KeyNotFoundException("The seller doesn't exist.");
            }
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public Product GetProductById(int id)
        {
            return _context.Products.SingleOrDefault(p => p.Id == id) ?? throw new ArgumentException("Product not found");
        }
    }
}