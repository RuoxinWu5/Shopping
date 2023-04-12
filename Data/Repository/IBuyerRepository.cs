using Data.Model;

namespace Data.Repository
{
    public interface IBuyerRepository
    {
        Task<IEnumerable<Product>> AllProduct();
        Task<BuyerProduct> GetProductByProductId(int productId);
    }
}