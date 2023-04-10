using System.Net;
using Data.Exceptions;
using Data.Model;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Service;
using Shopping.Controller;

namespace UnitTest.ControllerTest
{
    public class ProductControllerTest
    {
        private readonly ProductController _productController;
        private readonly Mock<IProductService> _productServiceMock;

        public ProductControllerTest()
        {
            _productServiceMock = new Mock<IProductService>();
            _productController = new ProductController(_productServiceMock.Object);
        }

        [Fact]
        public async Task GetProductListBySellerId_ShouldReturnOk_WhenProductsIsfound()
        {
            // Arrange
            var resultItem = new List<Product>{
                new Product{ Name = "Apple", Quantity = 100, SellerId = 1},
                new Product{ Name = "Banana", Quantity = 50, SellerId = 1}
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
            var product = new Product { Name = "Apple", Quantity = 100, SellerId = 1 };
            Assert.NotNull(product);
            _productServiceMock
                .Setup(service => service.AddProduct(product))
                .Returns(Task.CompletedTask);
            // Act
            var result = await _productController.AddProduct(product);
            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("Create product successfully.", createdResult.Value);
        }

        [Fact]
        public async Task AddProduct_ShouldReturnBadRequest_WhenProductNameExists()
        {
            // Arrange
            var product = new Product { Name = "Apple", Quantity = 100, SellerId = 1 };
            Assert.NotNull(product);
            _productServiceMock
                .Setup(service => service.AddProduct(product))
                .Throws(new DuplicateUserNameException($"Product name '{product.Name}' already exists."));
            // Act
            var result = await _productController.AddProduct(product);
            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal("Product name 'Apple' already exists.", conflictResult.Value);
        }

        [Fact]
        public async Task AddProduct_ShouldReturnNotFound_WhenSellerIdNotExists()
        {
            // Arrange
            var product = new Product { Name = "Apple", Quantity = 100, SellerId = 1 };
            Assert.NotNull(product);
            _productServiceMock
                .Setup(service => service.AddProduct(product))
                .Throws(new KeyNotFoundException("The seller doesn't exist."));
            // Act
            var result = await _productController.AddProduct(product);
            // Assert
            var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("The seller doesn't exist.", notFoundObjectResult.Value);
        }
    }
}