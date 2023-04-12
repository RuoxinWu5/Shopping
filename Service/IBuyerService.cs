using Data.Model;

namespace Service
{
    public interface IBuyerService
    {
        public Task<IEnumerable<Product>> AllProduct();
        public Task<BuyerProduct> GetProductByProductId(int productId);
    }
}