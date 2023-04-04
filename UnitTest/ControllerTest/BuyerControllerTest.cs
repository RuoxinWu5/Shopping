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
            var resultItem = new List<BuyerProduct>{
                new BuyerProduct() { id = 1, name = "Apple",quantity=100, sellerName = "Lisa" },
                new BuyerProduct() { id = 2, name = "Banana",quantity=50, sellerName = "Lisa" }
            };
            _buyerServiceMock.Setup(x => x.AllProduct()).ReturnsAsync(resultItem);
            // Act
            var result = await _buyerController.AllProduct();
            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(resultItem, okObjectResult.Value);
        }
    }
}