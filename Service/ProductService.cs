using Data.Exceptions;
using Data.Model;
using Data.Repository;
using System.Net;

namespace Service
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<HttpResponseMessage> AddProduct(Product product)
        {
            try{
                await _repository.AddProduct(product);
                return new HttpResponseMessage(HttpStatusCode.Created);
            }
            catch (DuplicateUserNameException exception)
            {
                return new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    Content = new StringContent(exception.Message.ToString())
                };
            }
            catch (DllNotFoundException exception)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(exception.Message.ToString())
                };
            }
        }

        public async Task<IEnumerable<Product>> GetProductListBySellerId(int sellerId)
        {
            var result = await _repository.GetProductListBySellerId(sellerId);
            return result;
        }
    }
}