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
    }
}