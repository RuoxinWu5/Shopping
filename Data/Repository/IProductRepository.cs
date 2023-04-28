using Data.Model;

namespace Data.Repository
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetProductListBySellerId(int sellerId);
        Task AddProduct(Product product);
        Task<Product?> GetProductById(int id);
        Task<Product?> GetProductByName(string name);
        Task<IEnumerable<Product>> AllProduct();
        Task UpdateProduct(Product product);
    }
}