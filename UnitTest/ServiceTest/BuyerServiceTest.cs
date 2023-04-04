using Data.Model;
using Data.Repository;
using Moq;
using Service;

namespace UnitTest.ServiceTest
{
    public class BuyerServiceTest
    {
        private readonly BuyerService _buyerService;
        private readonly Mock<IBuyerRepository> _buyerRepositoryMock;

        public BuyerServiceTest()
        {
            _buyerRepositoryMock = new Mock<IBuyerRepository>();
            _buyerService = new BuyerService(_buyerRepositoryMock.Object);
        }

        [Fact]
        public async Task GetProductList_ShouldReturnProductList_WhenProductsIsfound()
        {
            var resultItem = new List<BuyerProduct>{
                new BuyerProduct() { id = 1, name = "Apple",quantity=100, sellerName = "Lisa" },
                new BuyerProduct() { id = 2, name = "Banana",quantity=50, sellerName = "Lisa" }
            };
            _buyerRepositoryMock.Setup(repository => repository.AllProduct()).ReturnsAsync(resultItem);
            // Act
            var result = await _buyerService.AllProduct();
            // Assert
            Assert.Equal(resultItem, result);
        }
    }
}