using System.Net;
using Data.Exceptions;
using Data.Model;
using Data.Repository;
using Moq;
using Service;

namespace UnitTest.ServiceTest
{
    public class ProductServiceTest
    {
        private readonly ProductService _productService;
        private readonly Mock<IProductRepository> _productRepositoryMock;

        public ProductServiceTest()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _productService = new ProductService(_productRepositoryMock.Object);
        }

        [Fact]
        public async Task GetProductListBySellerId_ShouldReturnProductList_WhenProductsIsfound()
        {
            // Arrange
            var SellerId = 1;
            var resultItem = new List<Product>{
                new Product ("Apple", 100, 1 ),
            new Product ("Banana", 0, 1 )
            };
            _productRepositoryMock.Setup(repository => repository.GetProductListBySellerId(1)).ReturnsAsync(resultItem);
            // Act
            var result = await _productService.GetProductListBySellerId(SellerId);
            // Assert
            Assert.Equal(resultItem, result);
        }

        [Fact]
        public async Task AddProduct_ShouldReturnSuccessMessage_WhenProductIsValId()
        {
            // Arrange
            var product = new Product ("Apple", 100, 1 );
            _productRepositoryMock
                .Setup(repository => repository.AddProduct(product))
                .Returns(Task.CompletedTask);
            // Act
            var result = await _productService.AddProduct(product);
            // Assert
            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
        }

        [Fact]
        public async Task AddProduct_ShouldReturnDuplicateUserNameExceptionMessage_WhenProductNameExists()
        {
            // Arrange
            var product = new Product ("Apple", 100, 1 );
            _productRepositoryMock
                .Setup(repository => repository.AddProduct(product))
                .Throws(new DuplicateUserNameException("Product Name 'Apple' already exists."));
            // Act
            var result = await _productService.AddProduct(product);
            // Assert
            Assert.Equal(HttpStatusCode.Conflict, result.StatusCode);
        }

        [Fact]
        public async Task AddProduct_ShouldReturnNotFoundExceptionMessage_WhenSellerIdNotExists()
        {
            // Arrange
            var product = new Product ("Apple", 100, 1 );
            _productRepositoryMock
                .Setup(repository => repository.AddProduct(product))
                .Throws(new DllNotFoundException("The seller doesn't exist."));
            // Act
            var result = await _productService.AddProduct(product);
            // Assert
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }
    }
}