using Data.Exceptions;
using Data.Model;
using Data.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Service;
using Shopping.Controller;

namespace UnitTest.ControllerTest
{
    public class ProductControllerTest
    {
        private readonly ProductsController _productController;
        private readonly Mock<IProductService> _productServiceMock;
        private readonly Mock<IUserService> _userServiceMock;

        public ProductControllerTest()
        {
            _productServiceMock = new Mock<IProductService>();
            _userServiceMock = new Mock<IUserService>();
            _productController = new ProductsController(_productServiceMock.Object, _userServiceMock.Object);
        }

        [Fact]
        public async Task GetProductListBySellerId_ShouldReturnOk_WhenSellerExist()
        {
            // Arrange
            var user = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var resultItem = new List<Product>{
                new Product{ Name = "Apple", Quantity = 100, User = user },
                new Product{ Name = "Banana", Quantity = 50, User = user }
            };
            _productServiceMock
                .Setup(x => x.GetProductListBySellerId(It.IsAny<int>()))
                .ReturnsAsync(resultItem);

            // Act
            var result = await _productController.GetProductListBySellerId(1);

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(resultItem, okObjectResult.Value);
        }

        [Fact]
        public async Task GetProductListBySellerId_ShouldReturnOk_WhenSellerNotExist()
        {
            // Arrange
            var user = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var resultItem = new List<Product>{
                new Product{ Name = "Apple", Quantity = 100, User = user },
                new Product{ Name = "Banana", Quantity = 50, User = user }
            };
            _productServiceMock
                .Setup(x => x.GetProductListBySellerId(It.IsAny<int>()))
                .ThrowsAsync(new SellerNotFoundException("The seller doesn't exist."));

            // Act
            var result = await _productController.GetProductListBySellerId(1);

            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("The seller doesn't exist.", badRequestObjectResult.Value);
        }

        [Fact]
        public async Task AddProduct_ShouldReturnOk_WhenProductIsValid()
        {
            // Arrange
            var user = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 100, User = user };
            _userServiceMock
                .Setup(service => service.GetSellerById(It.IsAny<int>()))
                .ReturnsAsync(user);

            var productViewModel = new AddProductRequestModel { Name = "Apple", Quantity = 100, SellerId = 1 };

            // Act
            var result = await _productController.AddProduct(productViewModel);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.NotNull(createdResult.Value);
            Assert.Equivalent(product, createdResult.Value);
        }

        [Fact]
        public async Task AddProduct_ShouldReturnNotFound_WhenSellerIdNotExists()
        {
            // Arrange
            _userServiceMock
                .Setup(service => service.GetSellerById(It.IsAny<int>()))
                .Throws(new SellerNotFoundException("The seller doesn't exist."));

            var productViewModel = new AddProductRequestModel { Name = "Apple", Quantity = 100, SellerId = 1 };

            // Act
            var result = await _productController.AddProduct(productViewModel);

            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("The seller doesn't exist.", badRequestObjectResult.Value);
        }

        [Fact]
        public async Task AddProduct_ShouldReturnBadRequest_WhenProductNameExists()
        {
            // Arrange
            var user = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            _userServiceMock
                .Setup(service => service.GetSellerById(It.IsAny<int>()))
                .ReturnsAsync(user);

            var product = new Product { Name = "Apple", Quantity = 100, User = user };
            _productServiceMock
                .Setup(service => service.AddProduct(It.IsAny<Product>()))
                .Throws(new DuplicateUserNameException($"Product name '{product.Name}' already exists."));

            var productViewModel = new AddProductRequestModel { Name = "Apple", Quantity = 100, SellerId = 1 };

            // Act
            var result = await _productController.AddProduct(productViewModel);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal("Product name 'Apple' already exists.", conflictResult.Value);
        }

        [Fact]
        public async Task GetProductById_ShouldReturnOk_WhenProductIsExist()
        {
            // Arrange
            var user = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 100, User = user };
            _productServiceMock
                .Setup(service => service.GetProductById(It.IsAny<int>()))
                .ReturnsAsync(product);

            // Act
            var result = await _productController.GetProductById(1);

            // Assert
            var createdResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(product, createdResult.Value);
        }

        [Fact]
        public async Task GetProductById_ShouldReturnBadRequest_WhenProductIsNotExists()
        {
            // Arrange
            _productServiceMock
                .Setup(service => service.GetProductById(It.IsAny<int>()))
                .Throws(new ProductNotFoundException("The product doesn't exist."));

            // Act
            var result = await _productController.GetProductById(1);

            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("The product doesn't exist.", badRequestObjectResult.Value);
        }
    }
}