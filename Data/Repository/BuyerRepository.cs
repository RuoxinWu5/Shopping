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

        public async Task<IEnumerable<BuyerProduct>> AllProduct()
        {
            var productResult = await _productContext.Products.ToListAsync();
            List<BuyerProduct> result = new List<BuyerProduct>();
            for (int i = 0; i < productResult.Count; i++)
            {
                BuyerProduct product = new BuyerProduct();
                var sellerId = productResult[i].sellerId;
                var seller = await _userContext.Users.Where(user => user.id == sellerId).ToListAsync();
                product.id = productResult[i].id;
                product.name = productResult[i].name;
                product.quantity = productResult[i].quantity;
                product.sellerName = seller.First().name;
                result.Add(product);
            }
            return result;
        }
    }
}