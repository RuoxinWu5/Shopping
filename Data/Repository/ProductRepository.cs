using Data.Exceptions;
using Data.Model;
using Microsoft.EntityFrameworkCore;

namespace Data.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductContext _context;
        private readonly UserContext _userContext;

        public ProductRepository(ProductContext context, UserContext userContext)
        {
            _context = context;
            _userContext = userContext;
        }

        public async Task<IEnumerable<Product>> GetProductListBySellerId(int sellerId)
        {
            var result = await _context.Products.Where(product => product.sellerId == sellerId).ToListAsync();
            return result;
        }

        public async Task AddProduct(Product product)
        {
            var existingProduct = await _context.Products.FirstOrDefaultAsync(u => u.name == product.name);
            if (existingProduct != null)
            {
                throw new DuplicateUserNameException($"User name '{product.name}' already exists.");
            }
            var existingSeller = await _userContext.Users.FirstOrDefaultAsync(u => u.id == product.sellerId);
            if (existingSeller == null || existingSeller.type == UserType.BUYER)
            {
                throw new DllNotFoundException("The seller doesn't exist.");
            }
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }
    }
}