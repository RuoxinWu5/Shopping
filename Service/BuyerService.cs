using Data.Model;
using Data.Repository;

namespace Service
{
    public class BuyerService : IBuyerService
    {
        private readonly IBuyerRepository _buyerRepository;

        public BuyerService(IBuyerRepository buyerRepository)
        {
            _buyerRepository = buyerRepository;
        }

        public async Task<IEnumerable<Product>> AllProduct()
        {
            var result = await _buyerRepository.AllProduct();
            var filteredResult = result.Where(p => p.Quantity != 0);
            return filteredResult;
        }

        public async Task<BuyerProduct> GetProductByProductId(int productId)
        {
            var result = await _buyerRepository.GetProductByProductId(productId);
            return result;
        }
    }
}