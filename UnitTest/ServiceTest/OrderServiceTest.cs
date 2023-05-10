using Data.Exceptions;
using Data.Model;
using Data.Repository;
using Data.RequestModel;
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
        private readonly Mock<ICartItemService> _cartItemServiceMock;

        public OrderServiceTest()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _productRepositoryMock = new Mock<IProductRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _userServiceMock = new Mock<IUserService>();
            _productServiceMock = new Mock<IProductService>();
            _cartItemServiceMock = new Mock<ICartItemService>();
            _orderService = new OrderService(_orderRepositoryMock.Object, _productRepositoryMock.Object, _userRepositoryMock.Object, _userServiceMock.Object, _productServiceMock.Object, _cartItemServiceMock.Object);
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

        [Theory]
        [InlineData(OrderStatus.TO_BE_PAID, OrderStatus.PAID)]
        [InlineData(OrderStatus.PAID, OrderStatus.SHIPPED)]
        [InlineData(OrderStatus.SHIPPED, OrderStatus.RECEIVED)]
        public async Task UpdateOrderState_ShouldUpdateStatusToExpectedStatus(OrderStatus initialStatus, OrderStatus expectedStatus)
        {
            // Arrange
            var user = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 50, User = user };
            var order = new Order { Quantity = 10, Status = initialStatus, Product = product, User = user };
            // Act
            await _orderService.UpdateOrderState(order);
            // Assert
            Assert.Equal(expectedStatus, order.Status);
            _orderRepositoryMock.Verify(repository => repository.UpdateOrder(order), Times.Once);
        }

        [Fact]
        public async Task PayOrder_UpdatesOrderState_WhenOrderIsOwnedByUserAndIsPayable()
        {
            // Arrange
            var orderId = 1;
            var userId = 2;
            var user = new User { Id = 2, Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 50, User = user };
            var order = new Order { Quantity = 10, Status = OrderStatus.TO_BE_PAID, Product = product, User = user };
            _orderRepositoryMock.Setup(x => x.GetOrderById(It.IsAny<int>())).ReturnsAsync(order);
            // Act
            await _orderService.PayOrder(orderId, userId);
            // Assert
            _orderRepositoryMock.Verify(repository => repository.UpdateOrder(order), Times.Once);
        }

        [Fact]
        public async Task PayOrder_ThrowsOrderOwnershipException_WhenOrderIsNotBelongToUser()
        {
            // Arrange
            var orderId = 1;
            var userId = 2;
            var user = new User { Id = 3, Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 50, User = user };
            var order = new Order { Quantity = 10, Status = OrderStatus.TO_BE_PAID, Product = product, User = user };
            _orderRepositoryMock.Setup(x => x.GetOrderById(It.IsAny<int>())).ReturnsAsync(order);
            // Act & Assert
            await Assert.ThrowsAsync<OrderOwnershipException>(() => _orderService.PayOrder(orderId, userId));
        }

        [Fact]
        public async Task PayOrder_ThrowsOrderStatusModificationException_WhenOrderIsNotPayable()
        {
            // Arrange
            var orderId = 1;
            var userId = 2;
            var user = new User { Id = 2, Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 50, User = user };
            var order = new Order { Quantity = 10, Status = OrderStatus.PAID, Product = product, User = user };
            _orderRepositoryMock.Setup(x => x.GetOrderById(It.IsAny<int>())).ReturnsAsync(order);
            // Act & Assert
            await Assert.ThrowsAsync<OrderStatusModificationException>(() => _orderService.ConfirmReceipt(orderId, userId));
        }

        [Fact]
        public async Task ConfirmReceipt_UpdatesOrderState_WhenOrderIsOwnedByUserAndIsReceivable()
        {
            // Arrange
            var orderId = 1;
            var userId = 2;
            var user = new User { Id = 2, Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 50, User = user };
            var order = new Order { Quantity = 10, Status = OrderStatus.SHIPPED, Product = product, User = user };
            _orderRepositoryMock.Setup(x => x.GetOrderById(It.IsAny<int>())).ReturnsAsync(order);
            // Act
            await _orderService.ConfirmReceipt(orderId, userId);
            // Assert
            _orderRepositoryMock.Verify(repository => repository.UpdateOrder(order), Times.Once);
        }

        [Fact]
        public async Task ConfirmReceipt_ThrowsOrderOwnershipException_WhenOrderIsNotBelongToUser()
        {
            // Arrange
            var orderId = 1;
            var userId = 2;
            var user = new User { Id = 3, Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 50, User = user };
            var order = new Order { Quantity = 10, Status = OrderStatus.SHIPPED, Product = product, User = user };
            _orderRepositoryMock.Setup(x => x.GetOrderById(It.IsAny<int>())).ReturnsAsync(order);
            // Act & Assert
            await Assert.ThrowsAsync<OrderOwnershipException>(() => _orderService.ConfirmReceipt(orderId, userId));
        }

        [Fact]
        public async Task ConfirmReceipt_ThrowsOrderStatusModificationException_WhenOrderIsNotReceivable()
        {
            // Arrange
            var orderId = 1;
            var userId = 2;
            var user = new User { Id = 2, Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 50, User = user };
            var order = new Order { Quantity = 10, Status = OrderStatus.PAID, Product = product, User = user };
            _orderRepositoryMock.Setup(x => x.GetOrderById(It.IsAny<int>())).ReturnsAsync(order);
            // Act & Assert
            await Assert.ThrowsAsync<OrderStatusModificationException>(() => _orderService.PayOrder(orderId, userId));
        }

        [Fact]
        public async Task ShipOrder_UpdatesOrderState_WhenOrderIsOwnedByUserAndIsShippable()
        {
            // Arrange
            var orderId = 1;
            var userId = 2;
            var user = new User { Id = 2, Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 50, User = user };
            var order = new Order { Quantity = 10, Status = OrderStatus.PAID, Product = product, User = user };
            _orderRepositoryMock.Setup(x => x.GetOrderById(It.IsAny<int>())).ReturnsAsync(order);
            // Act
            await _orderService.ShipOrder(orderId, userId);
            // Assert
            _orderRepositoryMock.Verify(repository => repository.UpdateOrder(order), Times.Once);
        }

        [Fact]
        public async Task ShipOrder_ThrowsOrderOwnershipException_WhenOrderIsNotBelongToUser()
        {
            // Arrange
            var orderId = 1;
            var userId = 2;
            var user = new User { Id = 3, Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 50, User = user };
            var order = new Order { Quantity = 10, Status = OrderStatus.PAID, Product = product, User = user };
            _orderRepositoryMock.Setup(x => x.GetOrderById(It.IsAny<int>())).ReturnsAsync(order);
            // Act & Assert
            await Assert.ThrowsAsync<OrderOwnershipException>(() => _orderService.ShipOrder(orderId, userId));
        }

        [Fact]
        public async Task ShipOrder_ThrowsOrderStatusModificationException_WhenOrderIsNotShippable()
        {
            // Arrange
            var orderId = 1;
            var userId = 2;
            var user = new User { Id = 2, Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 50, User = user };
            var order = new Order { Quantity = 10, Status = OrderStatus.TO_BE_PAID, Product = product, User = user };
            _orderRepositoryMock.Setup(x => x.GetOrderById(It.IsAny<int>())).ReturnsAsync(order);
            // Act & Assert
            await Assert.ThrowsAsync<OrderStatusModificationException>(() => _orderService.ShipOrder(orderId, userId));
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
        public void IsOrderOwnedByUser_ShouldThrowOrderOwnershipException_WhenThisOrderNotBelongsToThisUser()
        {
            // Arrange
            var userId = 2;
            var user = new User { Id = 1, Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 50, User = user };
            var order = new Order { Id = 1, Quantity = 10, Status = OrderStatus.TO_BE_PAID, Product = product, User = user };
            // Act & Assert
            Assert.Throws<OrderOwnershipException>(() => _orderService.IsOrderOwnedByUser(order, userId));
        }

        [Fact]
        public void IsExpectedOrderStatus_ShouldReturnTrue_WhenThisOrderIsTheStatus()
        {
            // Arrange
            var expectStatus = OrderStatus.TO_BE_PAID;
            var user = new User { Id = 1, Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 50, User = user };
            var order = new Order { Id = 1, Quantity = 10, Status = OrderStatus.TO_BE_PAID, Product = product, User = user };
            _orderRepositoryMock.Setup(repository => repository.GetOrderById(It.IsAny<int>())).ReturnsAsync(order);
            // Act
            var result = _orderService.IsExpectedOrderStatus(order, expectStatus);
            // Assert
            Assert.Equal(true, result);
        }

        [Fact]
        public void IsExpectedOrderStatus_ShouldReturnFalse_WhenThisOrderIsNotTheStatus()
        {
            // Arrange
            var expectStatus = OrderStatus.PAID;
            var user = new User { Id = 1, Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 50, User = user };
            var order = new Order { Id = 1, Quantity = 10, Status = OrderStatus.TO_BE_PAID, Product = product, User = user };
            _orderRepositoryMock.Setup(repository => repository.GetOrderById(It.IsAny<int>())).ReturnsAsync(order);
            // Act
            var result = _orderService.IsExpectedOrderStatus(order, expectStatus);
            // Assert
            Assert.Equal(false, result);
        }

        [Fact]
        public async Task AddOrderFromCartItem_ShouldAddOrderAndReduceProductQuantity()
        {
            // Arrange
            var seller = new User { Id = 1, Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Id = 1, Name = "Apple", Quantity = 100, User = seller };
            var buyer = new User { Id = 2, Name = "Lisa", Password = "Lisa123", Type = UserType.BUYER };
            var cartItem = new CartItem { Id = 1, Product = product, Quantity = 10, User = buyer };
            _cartItemServiceMock.Setup(x => x.GetCartItemById(It.IsAny<int>())).ReturnsAsync(cartItem);
            var addOrderFromCartItemRequestModel = new AddOrderFromCartItemRequestModel { CartItemId = 1, BuyerId = 2 };
            // Act
            var result = await _orderService.AddOrderFromCartItem(addOrderFromCartItemRequestModel);
            // Assert
            Assert.NotNull(result);
            _productServiceMock.Verify(service => service.ReduceProductQuantity(product, result.Quantity), Times.Once);
            _orderRepositoryMock.Verify(repository => repository.AddOrder(result), Times.Once);
            _cartItemServiceMock.Verify(service => service.DeleteCartItemById(cartItem.Id), Times.Once);
        }

        [Fact]
        public async Task AddOrderFromCartItem_ThrowsCartItemNotFoundException_WhenCartItemNotFound()
        {
            // Arrange
            var mockCartItemService = new Mock<ICartItemService>();
            _cartItemServiceMock.Setup(service => service.GetCartItemById(It.IsAny<int>())).ThrowsAsync(new CartItemNotFoundException("The cart item doesn't exist."));
            var addOrderFromCartItemRequestModel = new AddOrderFromCartItemRequestModel { CartItemId = 1, BuyerId = 2 };
            // Act & Assert
            await Assert.ThrowsAsync<CartItemNotFoundException>(async () => await _orderService.AddOrderFromCartItem(addOrderFromCartItemRequestModel));
        }

        [Fact]
        public async Task AddOrderFromCartItem_ThrowsCartItemOwnershipException_WhenCartItemOwnerDoesNotMatchBuyer()
        {
            // Arrange
            _cartItemServiceMock.Setup(service => service.GetCartItemById(It.IsAny<int>())).ThrowsAsync(new CartItemOwnershipException("This cart item is not yours."));
            var addOrderFromCartItemRequestModel = new AddOrderFromCartItemRequestModel { CartItemId = 1, BuyerId = 2 };
            // Act & Assert
            await Assert.ThrowsAsync<CartItemOwnershipException>(async () => await _orderService.AddOrderFromCartItem(addOrderFromCartItemRequestModel));
        }
    }
}