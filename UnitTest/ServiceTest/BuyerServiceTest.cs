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
            var resultItem = new List<string> { "Apple", "Banana" };
            _buyerRepositoryMock.Setup(repository => repository.AllProduct()).ReturnsAsync(resultItem);
            // Act
            var result = await _buyerService.AllProduct();
            // Assert
            Assert.Equal(resultItem, result);
        }
    }
}