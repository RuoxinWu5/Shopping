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

        public async Task<IEnumerable<string>> AllProduct()
        {
            var result = await _buyerRepository.AllProduct();
            return result;
        }

        public Task<BuyerProduct> GetProductByProductId(int productId)
        {
            throw new NotImplementedException();
        }
    }
}