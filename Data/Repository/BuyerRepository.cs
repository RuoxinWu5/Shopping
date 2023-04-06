using Data.Model;
using Microsoft.EntityFrameworkCore;

namespace Data.Repository
{
    public class BuyerRepository : IBuyerRepository
    {
        private readonly BuyerProductContext _context;
        private readonly ProductContext _productContext;
        private readonly UserContext _userContext;

        public BuyerRepository(BuyerProductContext context, ProductContext productContext, UserContext userContext)
        {
            _context = context;
            _productContext = productContext;
            _userContext = userContext;
        }

        public async Task<IEnumerable<string>> AllProduct()
        {
            var productResult = await _productContext.Products.ToListAsync();
            List<string> result = new List<string>();
            for (int i = 0; i < productResult.Count; i++)
            {
                if(productResult[i].quantity==0){
                    continue;
                }
                var product = productResult[i].name;
                result.Add(product);
            }
            return result;
        }

        public async Task<BuyerProduct> GetProductByProductId(int productId)
        {
            var productResult = await _productContext.Products.FindAsync(productId);
            if (productResult == null)
            {
                throw new KeyNotFoundException($"Product id '{productId}' doesn't exist.");
            }
            var seller = await _userContext.Users.FindAsync(productResult.sellerId);
            BuyerProduct result = new BuyerProduct();
            result.id = productResult.id;
            result.name = productResult.name;
            result.quantity = productResult.quantity;
            result.sellerName = seller.name;
            return result;
        }
    }
}