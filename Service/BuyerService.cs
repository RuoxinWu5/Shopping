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

        public async Task<IEnumerable<BuyerProduct>> AllProduct()
        {
            var result = await _buyerRepository.AllProduct();
            return result;
        }
    }
}