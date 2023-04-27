using Data.Exceptions;
using Data.Model;
using Data.Repository;

namespace Service
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly IUserService _userService;

        public ProductService(IProductRepository repository, IUserService userService)
        {
            _repository = repository;
            _userService = userService;
        }

        public async Task AddProduct(Product product)
        {
            var existingProduct = await _repository.GetProductByName(product.Name);
            if (existingProduct != null)
                throw new DuplicateUserNameException($"User name '{product.Name}' already exists.");
            await _repository.AddProduct(product);
        }

        public async Task<IEnumerable<Product>> GetProductListBySellerId(int sellerId)
        {
            await _userService.ValidateIfSellerExist(sellerId);
            var result = await _repository.GetProductListBySellerId(sellerId);
            return result;
        }

        public async Task<Product> GetProductById(int id)
        {
            var product = await _repository.GetProductById(id);
            if (product != null)
            {
                return product;
            }
            throw new ProductNotFoundException("The product doesn't exist.");
        }

        public async Task<IEnumerable<Product>> AllProduct()
        {
            var result = await _repository.AllProduct();
            var filteredResult = result.Where(p => p.Quantity != 0);
            return filteredResult;
        }
    }
}