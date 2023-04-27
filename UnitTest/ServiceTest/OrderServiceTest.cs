using Data.Model;
using Data.Repository;
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
        public async Task AddOrderAndReduceProductQuantity_ShouldCallAddOrderMethodOfRepository_WhenQuantityIsEnough()
        {
            // Arrange
            var user = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 100, User = user };
            var order = new Order { Quantity = 10, Status = OrderState.TO_BE_PAID, Product = product, User = user };
            _productRepositoryMock.Setup(repository => repository.GetProductById(It.IsAny<int>())).ReturnsAsync(product);
            // Act
            await _orderService.AddOrderAndReduceProductQuantity(order);
            // Assert
            _orderRepositoryMock.Verify(repository => repository.AddOrder(order), Times.Once);
            _productRepositoryMock.Verify(x => x.ReduceProductQuantity(product, order.Quantity), Times.Once);
        }

        [Fact]
        public async Task AddOrderAndReduceProductQuantity_ShouldCallAddOrderMethodOfRepository_WhenQuantityIsNotEnough()
        {
            // Arrange
            var user = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 50, User = user };
            var order = new Order { Quantity = 51, Status = OrderState.TO_BE_PAID, Product = product, User = user };
            _productRepositoryMock.Setup(repository => repository.GetProductById(It.IsAny<int>())).ReturnsAsync(product);
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await _orderService.AddOrderAndReduceProductQuantity(order));
        }

        [Fact]
        public async Task GetOrderById_ShouldCallGetOrderByIdMethodOfRepository()
        {
            // Arrange
            var id = 1;
            // Act
            await _orderService.GetOrderById(id);
            // Assert
            _orderRepositoryMock.Verify(repository => repository.GetOrderById(id), Times.Once);
        }

        [Fact]
        public async Task UpdateOrderState_ShouldCallUpdateOrderStateMethodOfRepository()
        {
            // Arrange
            var id = 1;
            // Act
            await _orderService.UpdateOrderState(id, OrderState.TO_BE_PAID);
            // Assert
            _orderRepositoryMock.Verify(repository => repository.UpdateOrderState(id, OrderState.TO_BE_PAID), Times.Once);
        }

        [Fact]
        public async Task GetOrderListBySellerId_ShouldCallGetOrderListBySellerIdMethodOfRepository()
        {
            // Arrange
            var id = 1;
            // Act
            await _orderService.GetOrderListBySellerId(id);
            // Assert
            _orderRepositoryMock.Verify(repository => repository.GetOrderListBySellerId(id), Times.Once);
        }
    }
}