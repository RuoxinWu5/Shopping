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
        public async Task GetProductListBySellerId_ShouldReturnOk_WhenProductsIsfound()
        {
            // Arrange
            var user = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var resultItem = new List<Product>{
                new Product{ Name = "Apple", Quantity = 100, User = user },
                new Product{ Name = "Banana", Quantity = 50, User = user }
                };
            _productServiceMock.Setup(x => x.GetProductListBySellerId(It.IsAny<int>())).ReturnsAsync(resultItem);
            // Act
            var result = await _productController.GetProductListBySellerId(1);
            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(resultItem, okObjectResult.Value);
        }

        [Fact]
        public async Task AddProduct_ShouldReturnOk_WhenProductIsValid()
        {
            // Arrange
            var user = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 100, User = user };
            var productViewModel = new AddProductRequestModel { Name = "Apple", Quantity = 100, SellerId = 1 };
            _productServiceMock
                .Setup(service => service.AddProduct(It.IsAny<Product>()))
                .ReturnsAsync(product);
            _userServiceMock
                .Setup(service => service.GetSellerById(productViewModel.SellerId))
                .ReturnsAsync(user);
            // Act
            var result = await _productController.AddProduct(productViewModel);
            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(product, createdResult.Value);
        }

        [Fact]
        public async Task AddProduct_ShouldReturnBadRequest_WhenProductNameExists()
        {
            // Arrange
            var user = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 100, User = user };
            var productViewModel = new AddProductRequestModel { Name = "Apple", Quantity = 100, SellerId = 1 };
            Assert.NotNull(product);
            _productServiceMock
                .Setup(service => service.AddProduct(It.Is<Product>(product => product.User.Name == "Jack" && product.Name == "Apple")))
                .Throws(new DuplicateUserNameException($"Product name '{product.Name}' already exists."));
            _userServiceMock
                .Setup(service => service.GetSellerById(productViewModel.SellerId))
                .ReturnsAsync(user);
            // Act
            var result = await _productController.AddProduct(productViewModel);
            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal("Product name 'Apple' already exists.", conflictResult.Value);
        }

        [Fact]
        public async Task AddProduct_ShouldReturnNotFound_WhenSellerIdNotExists()
        {
            // Arrange
            var user = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 100, User = user };
            var productViewModel = new AddProductRequestModel { Name = "Apple", Quantity = 100, SellerId = 1 };
            Assert.NotNull(product);
            _productServiceMock
                .Setup(service => service.AddProduct(It.Is<Product>(product => product.User.Name == "Jack" && product.Name == "Apple")))
                .Throws(new KeyNotFoundException("The seller doesn't exist."));
            _userServiceMock
                .Setup(service => service.GetSellerById(productViewModel.SellerId))
                .ReturnsAsync(user);
            // Act
            var result = await _productController.AddProduct(productViewModel);
            // Assert
            var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("The seller doesn't exist.", notFoundObjectResult.Value);
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
                .Throws(new KeyNotFoundException("The product doesn't exist."));
            // Act
            var result = await _productController.GetProductById(1);
            // Assert
            var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("The product doesn't exist.", notFoundObjectResult.Value);
        }
    }
}