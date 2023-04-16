using Data.Model;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Service;
using Shopping.Controller;

namespace UnitTest.ControllerTest
{
    public class ProductSummaryControllerTest
    {
        private readonly ProductSummaryController _buyerController;
        private readonly Mock<IProductService> _productServiceMock;
        private readonly Mock<IUserService> _userServiceMock;

        public ProductSummaryControllerTest()
        {
            _productServiceMock = new Mock<IProductService>();
            _userServiceMock = new Mock<IUserService>();
            _buyerController = new ProductSummaryController(_productServiceMock.Object, _userServiceMock.Object);
        }

        [Fact]
        public async Task GetProductList_ShouldReturnOk_WhenProductsIsfound()
        {
            // Arrange
            var user = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var resultServiceItem = new List<Product>{
                new Product{ Name = "Apple", Quantity = 100, User = user },
                new Product{ Name = "Banana", Quantity = 50, User = user }
            };
            var resultItem = new List<String> { "Apple", "Banana" };
            _productServiceMock.Setup(x => x.AllProduct()).ReturnsAsync(resultServiceItem);
            // Act
            var result = await _buyerController.AllProduct();
            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(resultItem, okObjectResult.Value);
        }

        [Fact]
        public async Task GetProductByProductId_ShouldReturnOk_WhenProductsIsfound()
        {
            // Arrange
            var user = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 100, User = user };
            var resultItem = new BuyerProduct() { Id = 1, Name = "Apple", Quantity = 100, SellerName = "Jack" };
            _productServiceMock.Setup(x => x.GetProductById(It.IsAny<int>())).ReturnsAsync(product);
            // Act
            var result = await _buyerController.GetProductByProductId(1);
            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okObjectResult.Value);
            Assert.Equal(resultItem.ToString(), okObjectResult.Value.ToString());
        }

        [Fact]
        public async Task GetProductByProductId_ShouldReturnNotFoundException_WhenProductsIsNotfound()
        {
            // Arrange
            _productServiceMock.Setup(x => x.GetProductById(It.IsAny<int>())).ThrowsAsync(new KeyNotFoundException());
            // Act
            var result = await _buyerController.GetProductByProductId(1);
            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}