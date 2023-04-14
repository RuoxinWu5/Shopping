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
            var id = 1;
            // Act
            await _productService.GetProductListBySellerId(id);
            // Assert
            _productRepositoryMock.Verify(repository => repository.GetProductListBySellerId(id), Times.Once);
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

        [Fact]
        public async Task GetProductList_ShouldReturnProductList_WhenProductsIsfound()
        {
            var user = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var resultRepositoryItem = new List<Product>
            {
                new Product { Name = "Apple", Quantity = 100, User = user },
                new Product { Name = "Banana", Quantity = 0, User = user }
            };
            var resultItem = new List<Product>
            {
                new Product { Name = "Apple", Quantity = 100, User = user }
            };
            _productRepositoryMock.Setup(repository => repository.AllProduct()).ReturnsAsync(resultRepositoryItem);
            // Act
            var result = await _productService.AllProduct();
            // Assert
            Assert.Equal(resultItem[0].Name, result.First().Name);
            Assert.Equal(resultItem[0].Quantity, result.First().Quantity);
        }

        [Fact]
        public async Task GetProductById_ShouldCallGetProductByIdMethodOfRepository()
        {
            // Arrange
            var id = 1;
            // Act
            await _productService.GetProductById(id);
            // Assert
            _productRepositoryMock.Verify(repository => repository.GetProductById(id), Times.Once);
        }

    }
}