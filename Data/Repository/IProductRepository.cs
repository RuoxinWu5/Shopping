using Data.Model;

namespace Data.Repository
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetProductListBySellerId(int sellerId);
        Task AddProduct(Product product);
        Task<Product> GetProductById(int id);
        Task<IEnumerable<Product>> AllProduct();
        Task ProductReduce(Product product, int quantity);
    }
}