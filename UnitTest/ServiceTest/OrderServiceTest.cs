using Data.Exceptions;
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
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IProductService> _productServiceMock;

        public OrderServiceTest()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _productRepositoryMock = new Mock<IProductRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _userServiceMock = new Mock<IUserService>();
            _productServiceMock = new Mock<IProductService>();
            _orderService = new OrderService(_orderRepositoryMock.Object, _productRepositoryMock.Object, _userRepositoryMock.Object, _userServiceMock.Object, _productServiceMock.Object);
        }

        [Fact]
        public async Task AddOrderAndReduceProductQuantity_ShouldCallAddOrderMethodOfRepository_WhenQuantityIsEnough()
        {
            // Arrange
            var user = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 100, User = user };
            var order = new Order { Quantity = 10, Status = OrderStatus.TO_BE_PAID, Product = product, User = user };
            // Act
            await _orderService.AddOrderAndReduceProductQuantity(order);
            // Assert
            _productServiceMock.Verify(x => x.ReduceProductQuantity(product, order.Quantity), Times.Once);
            _orderRepositoryMock.Verify(repository => repository.AddOrder(order), Times.Once);
        }

        [Fact]
        public async Task AddOrderAndReduceProductQuantity_ShouldCallAddOrderMethodOfRepository_WhenQuantityIsNotEnough()
        {
            // Arrange
            var user = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 50, User = user };
            var order = new Order { Quantity = 51, Status = OrderStatus.TO_BE_PAID, Product = product, User = user };
            _productServiceMock.Setup(repository => repository.ReduceProductQuantity(It.IsAny<Product>(), It.IsAny<int>())).ThrowsAsync(new ArgumentException("Quantity not sufficient. Order creation failed."));
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await _orderService.AddOrderAndReduceProductQuantity(order));
        }

        [Fact]
        public async Task GetOrderById_ShouldReturnOrder_WhenOrderExist()
        {
            // Arrange
            var id = 1;
            var user = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 50, User = user };
            var order = new Order { Quantity = 51, Status = OrderStatus.TO_BE_PAID, Product = product, User = user };
            _orderRepositoryMock.Setup(repository => repository.GetOrderById(It.IsAny<int>())).ReturnsAsync(order);
            // Act
            var result = await _orderService.GetOrderById(id);
            // Assert
            Assert.Equal(order, result);
        }

        [Fact]
        public async Task GetOrderById_ShouldThrowOrderNotFoundException_WhenOrderNotExist()
        {
            // Arrange
            var id = 1;
            Order? nullOrder = null;
            _orderRepositoryMock.Setup(repository => repository.GetOrderById(It.IsAny<int>())).ReturnsAsync(nullOrder);
            // Act & Assert
            await Assert.ThrowsAsync<OrderNotFoundException>(async () => await _orderService.GetOrderById(id));
        }

        [Fact]
        public async Task UpdateOrderState_ShouldCallUpdateOrderStateMethodOfRepository()
        {
            // Arrange
            var id = 1;
            var user = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 50, User = user };
            var order = new Order { Quantity = 51, Status = OrderStatus.TO_BE_PAID, Product = product, User = user };
            _orderRepositoryMock.Setup(repository => repository.GetOrderById(It.IsAny<int>())).ReturnsAsync(order);
            // Act
            await _orderService.UpdateOrderState(id, OrderStatus.PAID);
            // Assert
            _orderRepositoryMock.Verify(repository => repository.UpdateOrder(order), Times.Once);
        }

        [Fact]
        public async Task GetOrderListBySellerId_ShouldReturnOrderLists_WhenSellerExist()
        {
            // Arrange
            var id = 1;
            var user = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 50, User = user };
            var orderLists = new List<Order>{
               new Order { Quantity = 10, Status = OrderStatus.TO_BE_PAID, Product = product, User = user }
            };
            _orderRepositoryMock.Setup(repository => repository.GetOrderListBySellerId(It.IsAny<int>())).ReturnsAsync(orderLists);
            // Act
            var result = await _orderService.GetOrderListBySellerId(id);
            // Assert
            Assert.Equal(orderLists, result);
        }

        [Fact]
        public async Task GetOrderListBySellerId_ShouldThrowSellerNotFoundException_WhenSellerNotExist()
        {
            // Arrange
            var id = 1;
            _userServiceMock.Setup(service => service.ValidateIfSellerExist(It.IsAny<int>())).ThrowsAsync(new SellerNotFoundException("The seller doesn't exist."));
            // Act & Assert
            await Assert.ThrowsAsync<SellerNotFoundException>(async () => await _orderService.GetOrderListBySellerId(id));
        }

        [Fact]
        public async Task IsOrderOwnedByUser_ShouldReturnTrue_WhenThisOrderBelongsToThisUser()
        {
            // Arrange
            var orderId = 1;
            var userId = 1;
            var user = new User { Id = 1, Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 50, User = user };
            var order = new Order { Id = 1, Quantity = 10, Status = OrderStatus.TO_BE_PAID, Product = product, User = user };
            _orderRepositoryMock.Setup(repository => repository.GetOrderById(It.IsAny<int>())).ReturnsAsync(order);
            // Act
            var result = await _orderService.IsOrderOwnedByUser(orderId, userId);
            // Assert
            Assert.Equal(true, result);
        }

        [Fact]
        public async Task IsOrderOwnedByUser_ShouldReturnFalse_WhenThisOrderNotBelongsToThisUser()
        {
            // Arrange
            var orderId = 1;
            var userId = 2;
            var user = new User { Id = 1, Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 50, User = user };
            var order = new Order { Id = 1, Quantity = 10, Status = OrderStatus.TO_BE_PAID, Product = product, User = user };
            _orderRepositoryMock.Setup(repository => repository.GetOrderById(It.IsAny<int>())).ReturnsAsync(order);
            // Act
            var result = await _orderService.IsOrderOwnedByUser(orderId, userId);
            // Assert
            Assert.Equal(false, result);
        }

        [Fact]
        public async Task IsOrderOwnedByUser_ShouldThrowOrderNotFoundException_WhenOrderNotExist()
        {
            // Arrange
            var orderId = 1;
            var userId = 1;
            Order? nullOrder = null;
            _orderRepositoryMock.Setup(repository => repository.GetOrderById(It.IsAny<int>())).ReturnsAsync(nullOrder);
            // Act & Assert
            await Assert.ThrowsAsync<OrderNotFoundException>(async () => await _orderService.IsOrderOwnedByUser(orderId, userId));
        }

        [Fact]
        public async Task IsExpectedOrderStatus_ShouldReturnTrue_WhenThisOrderIsTheStatus()
        {
            // Arrange
            var orderId = 1;
            var expectStatus = OrderStatus.TO_BE_PAID;
            var user = new User { Id = 1, Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 50, User = user };
            var order = new Order { Id = 1, Quantity = 10, Status = OrderStatus.TO_BE_PAID, Product = product, User = user };
            _orderRepositoryMock.Setup(repository => repository.GetOrderById(It.IsAny<int>())).ReturnsAsync(order);
            // Act
            var result = await _orderService.IsExpectedOrderStatus(orderId, expectStatus);
            // Assert
            Assert.Equal(true, result);
        }

        [Fact]
        public async Task IsExpectedOrderStatus_ShouldReturnFalse_WhenThisOrderIsNotTheStatus()
        {
            // Arrange
            var orderId = 1;
            var expectStatus = OrderStatus.PAID;
            var user = new User { Id = 1, Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 50, User = user };
            var order = new Order { Id = 1, Quantity = 10, Status = OrderStatus.TO_BE_PAID, Product = product, User = user };
            _orderRepositoryMock.Setup(repository => repository.GetOrderById(It.IsAny<int>())).ReturnsAsync(order);
            // Act
            var result = await _orderService.IsExpectedOrderStatus(orderId, expectStatus);
            // Assert
            Assert.Equal(false, result);
        }

        [Fact]
        public async Task IsExpectedOrderStatus_ShouldThrowOrderNotFoundException_WhenOrderNotExistr()
        {
            // Arrange
            var orderId = 1;
            var expectStatus = OrderStatus.TO_BE_PAID;
            Order? nullOrder = null;
            _orderRepositoryMock.Setup(repository => repository.GetOrderById(It.IsAny<int>())).ReturnsAsync(nullOrder);
            // Act & Assert
            await Assert.ThrowsAsync<OrderNotFoundException>(async () => await _orderService.IsExpectedOrderStatus(orderId, expectStatus));
        }
    }
}