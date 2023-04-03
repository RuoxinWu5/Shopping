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
            var sellerId = 1;
            var resultItem = new List<Product>{
                new Product() { id = 1, name = "Apple",quantity=100, sellerId = 1 },
                new Product() { id = 2, name = "Banana",quantity=50, sellerId = 1 }
            };
            _productRepositoryMock.Setup(repository => repository.GetProductListBySellerId(1)).ReturnsAsync(resultItem);
            // Act
            var result = await _productService.GetProductListBySellerId(sellerId);
            // Assert
            Assert.Equal(resultItem, result);
        }
    }
}