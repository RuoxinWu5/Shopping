using Data.Model;
using Microsoft.EntityFrameworkCore;

namespace Data.Repository
{
    public class BuyerRepository : IBuyerRepository
    {
        private readonly MyDbContext _context;

        public BuyerRepository(MyDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<string>> AllProduct()
        {
            var productResult = await _context.Products.ToListAsync();
            List<string> result = new List<string>();
            for (int i = 0; i < _context.Products.Count(); i++)
            {
                if (productResult[i].Quantity == 0)
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
            var productResult = await _context.Products.FindAsync(productId);
            if (productResult == null)
            {
                throw new KeyNotFoundException($"Product id '{productId}' doesn't exist.");
            }
            var seller = await _context.Users.FindAsync(productResult.SellerId);
            BuyerProduct result = new BuyerProduct();
            result.id = productResult.Id;
            result.name = productResult.Name;
            result.quantity = productResult.Quantity;
            result.sellerName = seller?.Name;
            return result;
        }
    }
}