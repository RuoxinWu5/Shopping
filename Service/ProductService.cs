using Data.Model;
using Data.Repository;

namespace Service
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Product>> GetProductListBySellerId(int sellerId)
        {
            var result = await _repository.GetProductListBySellerId(sellerId);
            return result;
        }
    }
}