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
            var result = await _buyerRepository.AllProduct();//模型转换放在controller
            return result;
        }

        public async Task<BuyerProduct> GetProductByProductId(int productId)
        {
            var result = await _buyerRepository.GetProductByProductId(productId);
            return result;
        }
    }
}