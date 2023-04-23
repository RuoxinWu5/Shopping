using Data.Exceptions;
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

        public async Task<Product> AddProduct(Product product)
        {
            try
            {
                var existingProduct = await _repository.GetProductByName(product.Name);
                if (existingProduct != null)
                {
                    throw new DuplicateUserNameException($"User name '{product.Name}' already exists.");
                }
            }
            catch (KeyNotFoundException)
            {
                await _repository.AddProduct(product);
            }
            return product;
        }

        public async Task<IEnumerable<Product>> GetProductListBySellerId(int sellerId)
        {
            var result = await _repository.GetProductListBySellerId(sellerId);
            return result;
        }

        public async Task<Product> GetProductById(int id)
        {
            return await _repository.GetProductById(id);
        }

        public async Task<IEnumerable<Product>> AllProduct()
        {
            var result = await _repository.AllProduct();
            var filteredResult = result.Where(p => p.Quantity != 0);
            return filteredResult;
        }
    }
}