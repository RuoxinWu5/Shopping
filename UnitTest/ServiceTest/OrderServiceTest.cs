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
        public async Task AddOrder_ShouldCallAddOrderMethodOfRepository_WhenQuantityIsEnough()
        {
            // Arrange
            var user = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 100, User = user };
            var order = new Order { Quantity = 10, Type = OrderState.TO_BE_PAID, Product = product, User = user };
            // Act
            var result = await _orderService.AddOrder(order);
            // Assert
            Assert.Equal(product, result.Product);
            Assert.Equal(user, result.User);
            _orderRepositoryMock.Verify(repository => repository.AddOrder(result), Times.Once);
            _productRepositoryMock.Verify(x => x.ProductReduce(product, order.Quantity), Times.Once);
        }

        [Fact]
        public async Task AddOrder_ShouldCallAddOrderMethodOfRepository_WhenQuantityIsNotEnough()
        {
            // Arrange
            var user = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 50, User = user };
            var order = new Order { Quantity = 51, Type = OrderState.TO_BE_PAID, Product = product, User = user };
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await _orderService.AddOrder(order));
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
        public async Task PayOrder_ShouldCallPayOrderMethodOfRepository()
        {
            // Arrange
            var id = 1;
            // Act
            await _orderService.PayOrder(id);
            // Assert
            _orderRepositoryMock.Verify(repository => repository.PayOrder(id), Times.Once);
        }

        [Fact]
        public async Task ConfirmReceipt_ShouldCallConfirmReceiptMethodOfRepository()
        {
            // Arrange
            var id = 1;
            // Act
            await _orderService.ConfirmReceipt(id);
            // Assert
            _orderRepositoryMock.Verify(repository => repository.ConfirmReceipt(id), Times.Once);
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