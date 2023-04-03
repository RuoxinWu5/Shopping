using System.Net;
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
                new Product() { id = 1, name = "Apple",quantity=100, sellerId = 1 },
                new Product() { id = 2, name = "Banana",quantity=50, sellerId = 1 }
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
            var product = new Product { name = "Apple", quantity = 100, sellerId = 1 };
            Assert.NotNull(product);
            _productServiceMock
                .Setup(service => service.AddProduct(product))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Created));
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
            var product = new Product { name = "Apple", quantity = 100, sellerId = 1 };
            Assert.NotNull(product);
            _productServiceMock
                .Setup(service => service.AddProduct(product))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    Content = new StringContent($"Product name '{product.name}' already exists.")
                });
            // Act
            var result = await _productController.AddProduct(product);
            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal("User name 'test' already exists.", conflictResult.Value);
        }

        [Fact]
        public async Task AddProduct_ShouldReturnBadRequest_WhenProductNameIsEmpty()
        {
            // Arrange
            var product = new Product { name = "", quantity = 100, sellerId = 1 };
            Assert.NotNull(product);
            _productServiceMock
                .Setup(service => service.AddProduct(product))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Product name cannot be empty.")
                });
            // Act
            var result = await _productController.AddProduct(product);
            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Product name cannot be empty.", badRequestObjectResult.Value);
        }

        [Fact]
        public async Task AddProduct_ShouldReturnBadRequest_WhenQuantityLessThanZero()
        {
            // Arrange
            var product = new Product { name = "Apple", quantity = -1, sellerId = 1 };
            Assert.NotNull(product);
            _productServiceMock
                .Setup(service => service.AddProduct(product))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Quantity cannot be less than zero.")
                });
            // Act
            var result = await _productController.AddProduct(product);
            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Quantity cannot be less than zero.", badRequestObjectResult.Value);
        }

        [Fact]
        public async Task AddProduct_ShouldReturnNotFound_WhenSellerIdNotExists()
        {
            // Arrange
            var product = new Product { name = "Apple", quantity = 100, sellerId = 1 };
            Assert.NotNull(product);
            _productServiceMock
                .Setup(service => service.AddProduct(product))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("The seller doesn't exist.")
                });
            // Act
            var result = await _productController.AddProduct(product);
            // Assert
            var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("The seller doesn't exist.", notFoundObjectResult.Value);
        }
    }
}