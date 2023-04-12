using Data.Model;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Service;
using Shopping.Controller;

namespace UnitTest.ControllerTest
{
    public class BuyerControllerTest
    {
        private readonly BuyerController _buyerController;
        private readonly Mock<IBuyerService> _buyerServiceMock;

        public BuyerControllerTest()
        {
            _buyerServiceMock = new Mock<IBuyerService>();
            _buyerController = new BuyerController(_buyerServiceMock.Object);
        }

        [Fact]
        public async Task GetProductList_ShouldReturnOk_WhenProductsIsfound()
        {
            var user = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var resultServiceItem = new List<Product>{
                new Product{ Name = "Apple", Quantity = 100, User = user },
                new Product{ Name = "Banana", Quantity = 50, User = user }
                };
            var resultItem = new List<String> { "Apple", "Banana" };
            _buyerServiceMock.Setup(x => x.AllProduct()).ReturnsAsync(resultServiceItem);
            // Act
            var result = await _buyerController.AllProduct();
            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(resultItem, okObjectResult.Value);
        }

        [Fact]
        public async Task GetProductByProductId_ShouldReturnOk_WhenProductsIsfound()
        {
            var resultItem = new BuyerProduct() { Id = 1, Name = "Apple", Quantity = 100, SellerName = "Lisa" };
            _buyerServiceMock.Setup(x => x.GetProductByProductId(It.IsAny<int>())).ReturnsAsync(resultItem);
            // Act
            var result = await _buyerController.GetProductByProductId(1);
            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(resultItem, okObjectResult.Value);
        }

        [Fact]
        public async Task GetProductByProductId_ShouldReturnNotFoundException_WhenProductsIsNotfound()
        {
            _buyerServiceMock.Setup(x => x.GetProductByProductId(It.IsAny<int>())).ThrowsAsync(new KeyNotFoundException());
            // Act
            var result = await _buyerController.GetProductByProductId(1);
            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}