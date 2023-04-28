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
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IProductRepository> _productRepositoryMock;

        public ProductServiceTest()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _userServiceMock = new Mock<IUserService>();
            _productService = new ProductService(_productRepositoryMock.Object, _userServiceMock.Object);
        }

        [Fact]
        public async Task AddProduct_ShouldAddProductSuccessfully_WhenProductNameNotExists()
        {
            // Arrange
            var user = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Banana", Quantity = 100, User = user };
            Product? nullProduct = null;
            _productRepositoryMock.Setup(repository => repository.GetProductByName(It.IsAny<string>())).ReturnsAsync(nullProduct);
            // Act
            await _productService.AddProduct(product);
            // Assert
            _productRepositoryMock.Verify(repository => repository.AddProduct(product), Times.Once);
        }

        [Fact]
        public async Task AddProduct_ShouldThrowDuplicateUserNameException_WhenProductNameExists()
        {
            // Arrange
            var user = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Banana", Quantity = 100, User = user };
            _productRepositoryMock.Setup(repository => repository.GetProductByName(It.IsAny<string>())).ReturnsAsync(product);
            // Act & Assert
            await Assert.ThrowsAsync<DuplicateUserNameException>(async () => await _productService.AddProduct(product));
        }

        [Fact]
        public async Task GetProductListBySellerId_ShouldReturnProductList_WhenSellerExist()
        {
            // Arrange
            var id = 1;
            // Act
            await _productService.GetProductListBySellerId(id);
            // Assert
            _productRepositoryMock.Verify(repository => repository.GetProductListBySellerId(id), Times.Once);
        }

        [Fact]
        public async Task GetProductListBySellerId_ShouldThrowSellerNotFoundException_WhenSellerNotExist()
        {
            // Arrange
            var id = 1;
            _userServiceMock.Setup(service => service.ValidateIfSellerExist(It.IsAny<int>())).ThrowsAsync(new SellerNotFoundException("The seller doesn't exist."));
            // Act & Assert
            await Assert.ThrowsAsync<SellerNotFoundException>(async () => await _productService.GetProductListBySellerId(id));
        }

        [Fact]
        public async Task GetProductById_ShouldReturnProduct_WhenProductIsfound()
        {
            // Arrange
            var id = 1;
            var user = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var productResult = new Product { Name = "Apple", Quantity = 100, User = user };
            _productRepositoryMock.Setup(repository => repository.GetProductById(It.IsAny<int>())).ReturnsAsync(productResult);
            // Act
            var product = await _productService.GetProductById(id);
            // Assert
            Assert.Equal(productResult, product);
        }

        [Fact]
        public async Task GetProductById_ShouldReturnProductNotFoundException_WhenProductIsfound()
        {
            // Arrange
            var id = 1;
            _productRepositoryMock.Setup(repository => repository.GetProductById(It.IsAny<int>())).ThrowsAsync(new ProductNotFoundException("The product doesn't exist."));
            // Act & Assert
            await Assert.ThrowsAsync<ProductNotFoundException>(async () => await _productService.GetProductById(id));
        }

        [Fact]
        public async Task AllProduct_ShouldReturnProductList_WhenProductsIsfound()
        {
            // Arrange
            var user = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var resultItem = new List<Product>
            {
                new Product { Name = "Apple", Quantity = 100, User = user },
                new Product { Name = "Banana", Quantity = 0, User = user }
            };
            _productRepositoryMock.Setup(repository => repository.AllProduct()).ReturnsAsync(resultItem);
            // Act
            var result = await _productService.AllProduct();
            // Assert
            Assert.Equal(resultItem[0].Name, result.First().Name);
            Assert.Equal(resultItem[0].Quantity, result.First().Quantity);
        }

        [Fact]
        public async Task ReduceProductQuantity_ShouldCallUpdateProductMethodOfRepository_WhenQuantityIsValidate()
        {
            // Arrange
            var user = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Banana", Quantity = 100, User = user };
            var quantity = 10;
            // Act
            await _productService.ReduceProductQuantity(product, quantity);
            // Assert
            _productRepositoryMock.Verify(repository => repository.UpdateProduct(product), Times.Once);
        }

        [Fact]
        public async Task ReduceProductQuantity_ShouldThrowArgumentException_WhenQuantityIsNotValidate()
        {
            // Arrange
            var user = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Banana", Quantity = 100, User = user };
            var quantity = 1000;
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await _productService.ReduceProductQuantity(product, quantity));
        }
    }
}