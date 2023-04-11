using Data.Model;

namespace Service
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetProductListBySellerId(int sellerId);
        Task AddProduct(Product product);

        Product GetProductById(int id);
    }
}