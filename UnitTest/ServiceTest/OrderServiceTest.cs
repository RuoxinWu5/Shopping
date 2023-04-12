using Data.Model;
using Data.Repository;
using Data.ViewModel;
using Moq;
using Service;

namespace UnitTest.ServiceTest
{
    public class OrderServiceTest
    {
        private readonly OrderService _orderService;
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;

        public OrderServiceTest()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _productRepositoryMock = new Mock<IProductRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _orderService = new OrderService(_orderRepositoryMock.Object, _productRepositoryMock.Object, _userRepositoryMock.Object);
        }

        [Fact]
        public async Task AddOder_ShouldCallAddOrderMethodOfRepository_WhenQuantityIsEnough()
        {
            // Arrange
            var orderRequestModel = new OrderRequestModel { ProductId = 1, Quantity = 10, BuyerId = 1 };
            var product = new Product { Name = "Apple", Quantity = 100, SellerId = 1 };
            _productRepositoryMock.Setup(repository => repository.GetProductById(orderRequestModel.ProductId)).Returns(product);
            var user = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            _userRepositoryMock.Setup(repository => repository.GetUserById(orderRequestModel.BuyerId)).Returns(user);
            // Act
            var result = await _orderService.AddOrder(orderRequestModel);
            // Assert
            Assert.Equal(product, result.Product);
            Assert.Equal(user, result.User);
            _orderRepositoryMock.Verify(repository => repository.AddOrder(result), Times.Once);
        }

        [Fact]
        public async Task AddOder_ShouldCallAddOrderMethodOfRepository_WhenQuantityIsNotEnough()
        {
            // Arrange
            var orderRequestModel = new OrderRequestModel { ProductId = 1, Quantity = 10, BuyerId = 1 };
            var product = new Product { Name = "Apple", Quantity = 9, SellerId = 1 };
            _productRepositoryMock.Setup(repository => repository.GetProductById(orderRequestModel.ProductId)).Returns(product);
            var user = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            _userRepositoryMock.Setup(repository => repository.GetUserById(orderRequestModel.BuyerId)).Returns(user);
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await _orderService.AddOrder(orderRequestModel));
        }
    }
}