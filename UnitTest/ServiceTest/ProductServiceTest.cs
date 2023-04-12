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
            var user = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var resultItem = new List<Product>{
                new Product { Name = "Apple", Quantity = 100, User = user },
                new Product { Name = "Apple", Quantity = 0, User = user }
            };
            _productRepositoryMock.Setup(repository => repository.GetProductListBySellerId(1)).ReturnsAsync(resultItem);
            // Act
            var result = await _productService.GetProductListBySellerId(SellerId);
            // Assert
            Assert.Equal(resultItem, result);
        }

        [Fact]
        public async Task AddProduct_ShouldCallAddProductMethodOfRepository()
        {
            // Arrange
            var user = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Banana", Quantity = 100, User = user };
            // Act
            await _productService.AddProduct(product);
            // Assert
            _productRepositoryMock.Verify(repository => repository.AddProduct(product), Times.Once);
        }

    }
}