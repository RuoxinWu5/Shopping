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
                var product = productResult[i].name;
                result.Add(product);
            }
            return result;
        }

        public Task<BuyerProduct> GetProductByProductId(int productId)
        {
            throw new NotImplementedException();
        }
    }
}