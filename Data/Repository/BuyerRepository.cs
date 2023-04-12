using Data.Model;
using Microsoft.EntityFrameworkCore;

namespace Data.Repository
{
    public class BuyerRepository : IBuyerRepository
    {
        private readonly ShoppingDbContext _context;

        public BuyerRepository(ShoppingDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<string>> AllProduct()
        {
            var productResult = await _context.Products.Include(p => p.User).ToListAsync();
            List<string> result = new List<string>();
            for (int i = 0; i < _context.Products.Count(); i++)//LINQ
            {
                if (productResult[i].Quantity == 0)//service
                {
                    continue;
                }
                var product = productResult[i].Name;
                result.Add(product);
            }
            return result;
        }

        public async Task<BuyerProduct> GetProductByProductId(int productId)
        {
            var productResult = await _context.Products.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == productId);
            if (productResult == null)
            {
                throw new KeyNotFoundException($"Product id '{productId}' doesn't exist.");
            }
            var seller = await _context.Users.FindAsync(productResult.User.Id);
            BuyerProduct result = new BuyerProduct();
            result.Id = productResult.Id;
            result.Name = productResult.Name;
            result.Quantity = productResult.Quantity;
            result.SellerName = seller?.Name ?? "Unknown";
            return result;
        }
    }
}