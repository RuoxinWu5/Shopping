using Data.Model;
using Data.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Service;
using Shopping.Controller;

namespace UnitTest.ControllerTest
{
    public class OrderControllerTest
    {
        private readonly Mock<IOrderService> _orderServiceMock = new Mock<IOrderService>();
        private readonly Mock<IProductService> _productServiceMock = new Mock<IProductService>();
        private readonly Mock<IUserService> _userServiceMock = new Mock<IUserService>();
        private readonly OrdersController _orderController;
        public OrderControllerTest()
        {
            _orderServiceMock = new Mock<IOrderService>();
            _productServiceMock = new Mock<IProductService>();
            _userServiceMock = new Mock<IUserService>();
            _orderController = new OrdersController(_orderServiceMock.Object, _productServiceMock.Object, _userServiceMock.Object);
        }
        [Fact]
        public async Task CreateOrder_ShouldReturnOk_WhenOrderIsValid()
        {
            // Arrange
            var user = new User { Name = "Jack", Password = "Jack123", Type = UserType.BUYER };
            var orderRequest = new OrderRequestModel { ProductId = 1, Quantity = 10, BuyerId = 1 };
            var product = new Product { Name = "Apple", Quantity = 100, User = user };
            _productServiceMock.Setup(repository => repository.GetProductById(orderRequest.ProductId)).ReturnsAsync(product);
            _userServiceMock.Setup(repository => repository.GetBuyerById(orderRequest.BuyerId)).ReturnsAsync(user);
            var expectedOrder = new Order { Id = 1, Quantity = 10, Type = OrderState.TO_BE_PAID, Product = product, User = user };
            _orderServiceMock.Setup(x => x.AddOrder(It.IsAny<Order>())).ReturnsAsync(expectedOrder);
            // Act
            var result = await _orderController.AddOrder(orderRequest);
            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(expectedOrder, createdResult.Value);
        }

        [Fact]
        public async Task CreateOrder_ShouldReturnBadRequest_WhenQuantityIsNotEnough()
        {
            // Arrange
            var orderRequest = new OrderRequestModel { ProductId = 1, Quantity = 10, BuyerId = 1 };
            _orderServiceMock.Setup(x => x.AddOrder(It.IsAny<Order>())).Throws(new ArgumentException("Quantity not sufficient. Order creation failed."));
            // Act
            var result = await _orderController.AddOrder(orderRequest);
            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Quantity not sufficient. Order creation failed.", badRequestObjectResult.Value);
        }

        [Fact]
        public async Task CreateOrder_ShouldReturnNotFound_WhenUserIdNotExists()
        {
            // Arrange
            var orderRequest = new OrderRequestModel { ProductId = 1, Quantity = 10, BuyerId = 1 };
            _orderServiceMock.Setup(x => x.AddOrder(It.IsAny<Order>())).Throws(new KeyNotFoundException("The buyer doesn't exist."));
            // Act
            var result = await _orderController.AddOrder(orderRequest);
            // Assert
            var createdResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("The buyer doesn't exist.", createdResult.Value);
        }

        [Fact]
        public async Task CreateOrder_ShouldReturnNotFound_WhenProductIdNotExists()
        {
            // Arrange
            var orderRequest = new OrderRequestModel { ProductId = 1, Quantity = 10, BuyerId = 1 };
            _orderServiceMock.Setup(x => x.AddOrder(It.IsAny<Order>())).Throws(new KeyNotFoundException("The product doesn't exist."));
            // Act
            var result = await _orderController.AddOrder(orderRequest);
            // Assert
            var createdResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("The product doesn't exist.", createdResult.Value);
        }

        [Fact]
        public async Task GetOrderById_ShouldReturnOk_WhenOrderIsfound()
        {
            // Arrange
            var buyer = new User { Name = "Lisa", Password = "lisa123", Type = UserType.BUYER };
            var seller = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 100, User = seller };
            var order = new Order() { Quantity = 10, Type = OrderState.TO_BE_PAID, Product = product, User = buyer };
            var resultItem = new BuyerOrder() { ProductName = "Apple", Quantity = 10, SellerName = "Jack", BuyerName = "Lisa", Type = OrderState.TO_BE_PAID };
            _productServiceMock.Setup(p => p.GetProductById(It.IsAny<int>())).ReturnsAsync(product);
            _userServiceMock.Setup(u => u.GetBuyerById(It.IsAny<int>())).ReturnsAsync(seller);
            _orderServiceMock.Setup(x => x.GetOrderById(It.IsAny<int>())).ReturnsAsync(order);
            // Act
            var result = await _orderController.GetOrderById(order.Id);
            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okObjectResult.Value);
            Assert.Equal(resultItem.ToString(), okObjectResult.Value.ToString());
        }

        [Fact]
        public async Task GetOrderById_ShouldReturnNotFoundException_WhenOrderIsNotfound()
        {
            // Arrange
            _orderServiceMock.Setup(x => x.GetOrderById(It.IsAny<int>())).ThrowsAsync(new KeyNotFoundException());
            // Act
            var result = await _orderController.GetOrderById(1);
            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task PayOrder_ShouldReturnOk_WhenOrderStateIsToBePaid()
        {
            // Arrange
            var buyer = new User { Id = 1, Name = "Lisa", Password = "lisa123", Type = UserType.BUYER };
            var seller = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 100, User = seller };
            var order = new Order() { Quantity = 10, Type = OrderState.TO_BE_PAID, Product = product, User = buyer };
            _orderServiceMock.Setup(x => x.GetOrderById(It.IsAny<int>())).ReturnsAsync(order);
            _orderServiceMock
                .Setup(service => service.UpdateOrderState(It.IsAny<int>(), It.IsAny<OrderState>()))
                .Returns(Task.CompletedTask);
            // Act
            var result = await _orderController.PayOrder(order.Id);
            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Payment successful.", okObjectResult.Value);
        }

        [Fact]
        public async Task PayOrder_ShouldReturnBadRequest_WhenOrderStateIsNotToBePaid()
        {
            // Arrange
            var buyer = new User { Name = "Lisa", Password = "lisa123", Type = UserType.BUYER };
            var seller = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 100, User = seller };
            var order = new Order() { Quantity = 10, Type = OrderState.PAID, Product = product, User = buyer };
            _orderServiceMock.Setup(x => x.GetOrderById(It.IsAny<int>())).ReturnsAsync(order);
            // Act
            var result = await _orderController.PayOrder(order.Id);
            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Current order is not payable.", badRequestObjectResult.Value);
        }

        [Fact]
        public async Task PayOrder_ShouldReturnNotFoundException_WhenOrderIsNotfound()
        {
            // Arrange
            _orderServiceMock.Setup(x => x.GetOrderById(It.IsAny<int>())).ThrowsAsync(new KeyNotFoundException());
            // Act
            var result = await _orderController.PayOrder(1);
            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task ConfirmReceipt_ShouldReturnOk_WhenOrderStateIsShipped()
        {
            // Arrange
            var buyer = new User { Name = "Lisa", Password = "lisa123", Type = UserType.BUYER };
            var seller = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 100, User = seller };
            var order = new Order() { Quantity = 10, Type = OrderState.SHIPPED, Product = product, User = buyer };
            _orderServiceMock.Setup(x => x.GetOrderById(It.IsAny<int>())).ReturnsAsync(order);
            _orderServiceMock
                .Setup(service => service.UpdateOrderState(It.IsAny<int>(), It.IsAny<OrderState>()))
                .Returns(Task.CompletedTask);
            // Act
            var result = await _orderController.ConfirmReceipt(order.Id);
            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Received the goods successfully.", okObjectResult.Value);
        }

        [Fact]
        public async Task ConfirmReceipt_ShouldReturnBadRequest_WhenOrderStateIsNotShipped()
        {
            // Arrange
            var buyer = new User { Name = "Lisa", Password = "lisa123", Type = UserType.BUYER };
            var seller = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 100, User = seller };
            var order = new Order() { Quantity = 10, Type = OrderState.PAID, Product = product, User = buyer };
            _orderServiceMock.Setup(x => x.GetOrderById(It.IsAny<int>())).ReturnsAsync(order);
            // Act
            var result = await _orderController.ConfirmReceipt(order.Id);
            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Current order is not receivable.", badRequestObjectResult.Value);
        }

        [Fact]
        public async Task ConfirmReceipt_ShouldReturnNotFoundException_WhenOrderIsNotfound()
        {
            // Arrange
            _orderServiceMock.Setup(x => x.GetOrderById(It.IsAny<int>())).ThrowsAsync(new KeyNotFoundException());
            // Act
            var result = await _orderController.ConfirmReceipt(1);
            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task ShipOrder_ShouldReturnOk_WhenOrderStateIsPaid()
        {
            // Arrange
            var buyer = new User { Name = "Lisa", Password = "lisa123", Type = UserType.BUYER };
            var seller = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 100, User = seller };
            var order = new Order() { Quantity = 10, Type = OrderState.PAID, Product = product, User = buyer };
            _orderServiceMock.Setup(x => x.GetOrderById(It.IsAny<int>())).ReturnsAsync(order);
            _orderServiceMock
                .Setup(service => service.UpdateOrderState(It.IsAny<int>(), It.IsAny<OrderState>()))
                .Returns(Task.CompletedTask);
            // Act
            var result = await _orderController.ShipOrder(order.Id);
            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Delivery successful.", okObjectResult.Value);
        }

        [Fact]
        public async Task ShipOrder_ShouldReturnBadRequest_WhenOrderStateIsNotPaid()
        {
            // Arrange
            var buyer = new User { Name = "Lisa", Password = "lisa123", Type = UserType.BUYER };
            var seller = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 100, User = seller };
            var order = new Order() { Quantity = 10, Type = OrderState.TO_BE_PAID, Product = product, User = buyer };
            _orderServiceMock.Setup(x => x.GetOrderById(It.IsAny<int>())).ReturnsAsync(order);
            // Act
            var result = await _orderController.ShipOrder(order.Id);
            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Current order is not shippable.", badRequestObjectResult.Value);
        }

        [Fact]
        public async Task ShipOrder_ShouldReturnNotFoundException_WhenOrderIsNotfound()
        {
            // Arrange
            _orderServiceMock.Setup(x => x.GetOrderById(It.IsAny<int>())).ThrowsAsync(new KeyNotFoundException());
            // Act
            var result = await _orderController.ShipOrder(1);
            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetOrderListBySellerId_ShouldReturnOk_WhenOrdersIsfound()
        {
            // Arrange
            var seller = new User { Id = 1, Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var buyer = new User { Id = 2, Name = "Lisa", Password = "lisa123", Type = UserType.BUYER };
            var product = new Product { Id = 1, Name = "Apple", Quantity = 100, User = seller };
            var serviceResult = new List<Order>{
                new Order{ Quantity = 10, Type = OrderState.TO_BE_PAID, User = buyer, Product = product }
            };
            var resultItem = new List<SellerOrder>{
                new SellerOrder{ ProductId = 1, ProductName = "Apple", Quantity = 10, BuyerId = 2, BuyerName = "Lisa", Type = OrderState.TO_BE_PAID }
            };
            _productServiceMock.Setup(p => p.GetProductById(It.IsAny<int>())).ReturnsAsync(product);
            _userServiceMock.Setup(u => u.GetBuyerById(It.IsAny<int>())).ReturnsAsync(buyer);
            _orderServiceMock.Setup(x => x.GetOrderListBySellerId(It.IsAny<int>())).ReturnsAsync(serviceResult);
            // Act
            var result = await _orderController.GetOrderListBySellerId(1);
            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okObjectResult.Value);
            Assert.Equal(resultItem.ToString(), okObjectResult.Value.ToString());
        }

        [Fact]
        public async Task GetOrderListBySellerId_ShouldReturnNotFound_WhenSellerNotFound()
        {
            // Arrange
            int sellerId = 1;
            _userServiceMock.Setup(x => x.GetSellerById(sellerId)).Throws(new KeyNotFoundException("Seller not found"));
            // Act
            var result = await _orderController.GetOrderListBySellerId(sellerId);
            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Seller not found", notFoundResult.Value);
        }
    }
}