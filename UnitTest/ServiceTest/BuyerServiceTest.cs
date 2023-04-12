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
            var user = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var resultRepositoryItem = new List<Product>
            {
                new Product { Name = "Apple", Quantity = 100, User = user },
                new Product { Name = "Banana", Quantity = 0, User = user }
            };
            var resultItem = new List<Product>
            {
                new Product { Name = "Apple", Quantity = 100, User = user }
            };
            _buyerRepositoryMock.Setup(repository => repository.AllProduct()).ReturnsAsync(resultRepositoryItem);
            // Act
            var result = await _buyerService.AllProduct();
            // Assert
            //Assert.Equal(resultItem.Count(), result[0].Count());
            Assert.Equal(resultItem[0].Name, result.First().Name);
            Assert.Equal(resultItem[0].Quantity, result.First().Quantity);
        }

        [Fact]
        public async Task GetProductByProductId_ShouldReturnOk_WhenProductsIsfound()
        {
            var resultItem = new BuyerProduct() { Id = 1, Name = "Apple", Quantity = 100, SellerName = "Lisa" };
            _buyerRepositoryMock.Setup(x => x.GetProductByProductId(It.IsAny<int>())).ReturnsAsync(resultItem);
            // Act
            var result = await _buyerService.GetProductByProductId(1);
            // Assert
            Assert.Equal(resultItem, result);
        }
    }
}