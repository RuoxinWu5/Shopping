using System.Security.Claims;
using Data.Exceptions;
using Data.Model;
using Data.ViewModel;
using Microsoft.AspNetCore.Http;
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
        public async Task AddOrder_ShouldReturnOk_WhenOrderIsValid()
        {
            // Arrange
            var user = new User { Name = "Jack", Password = "Jack123", Type = UserType.BUYER };
            var product = new Product { Name = "Apple", Quantity = 100, User = user };
            _productServiceMock.Setup(repository => repository.GetProductById(It.IsAny<int>())).ReturnsAsync(product);
            _userServiceMock.Setup(repository => repository.GetBuyerById(It.IsAny<int>())).ReturnsAsync(user);
            var orderRequest = new AddOrderRequestModel { ProductId = 1, Quantity = 10, BuyerId = 1 };
            var expectedOrder = new Order { Id = 1, Quantity = 10, Status = OrderState.TO_BE_PAID, Product = product, User = user };
            // Act
            var result = await _orderController.AddOrder(orderRequest);
            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.NotNull(createdResult.Value);
            Assert.Equal(expectedOrder.ToString(), createdResult.Value.ToString());
        }

        [Fact]
        public async Task AddOrder_ShouldReturnBadRequest_WhenProductIdNotExists()
        {
            // Arrange
            _productServiceMock.Setup(x => x.GetProductById(It.IsAny<int>())).Throws(new ProductNotFoundException("The product doesn't exist."));
            var orderRequest = new AddOrderRequestModel { ProductId = 1, Quantity = 10, BuyerId = 1 };
            // Act
            var result = await _orderController.AddOrder(orderRequest);
            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("The product doesn't exist.", badRequestObjectResult.Value);
        }

        [Fact]
        public async Task AddOrder_ShouldReturnNotFound_WhenUserIdNotExists()
        {
            // Arrange
            var user = new User { Name = "Jack", Password = "Jack123", Type = UserType.BUYER };
            var product = new Product { Name = "Apple", Quantity = 100, User = user };
            _productServiceMock.Setup(repository => repository.GetProductById(It.IsAny<int>())).ReturnsAsync(product);
            _userServiceMock.Setup(x => x.GetBuyerById(It.IsAny<int>())).Throws(new BuyerNotFoundException("The buyer doesn't exist."));
            var orderRequest = new AddOrderRequestModel { ProductId = 1, Quantity = 10, BuyerId = 1 };
            // Act
            var result = await _orderController.AddOrder(orderRequest);
            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("The buyer doesn't exist.", badRequestObjectResult.Value);
        }

        [Fact]
        public async Task AddOrder_ShouldReturnBadRequest_WhenQuantityIsNotEnough()
        {
            // Arrange
            var user = new User { Name = "Jack", Password = "Jack123", Type = UserType.BUYER };
            var product = new Product { Name = "Apple", Quantity = 100, User = user };
            _productServiceMock.Setup(repository => repository.GetProductById(It.IsAny<int>())).ReturnsAsync(product);
            _userServiceMock.Setup(repository => repository.GetBuyerById(It.IsAny<int>())).ReturnsAsync(user);
            _orderServiceMock.Setup(x => x.AddOrderAndReduceProductQuantity(It.IsAny<Order>())).Throws(new ArgumentException("Quantity not sufficient. Order creation failed."));
            var orderRequest = new AddOrderRequestModel { ProductId = 1, Quantity = 10, BuyerId = 1 };
            // Act
            var result = await _orderController.AddOrder(orderRequest);
            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Quantity not sufficient. Order creation failed.", badRequestObjectResult.Value);
        }

        [Fact]
        public async Task GetOrderById_ShouldReturnOk_WhenOrderIsfound()
        {
            // Arrange
            var buyer = new User { Name = "Lisa", Password = "lisa123", Type = UserType.BUYER };
            var seller = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 100, User = seller };
            var order = new Order() { Quantity = 10, Status = OrderState.TO_BE_PAID, Product = product, User = buyer };
            var resultItem = new BuyerOrder() { ProductName = "Apple", Quantity = 10, SellerName = "Jack", BuyerName = "Lisa", Status = OrderState.TO_BE_PAID };
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
        public async Task GetOrderById_ShouldReturnBadRequest_WhenOrderIsNotfound()
        {
            // Arrange
            _orderServiceMock.Setup(x => x.GetOrderById(It.IsAny<int>())).ThrowsAsync(new OrderNotFoundException("The order doesn't exist."));
            // Act
            var result = await _orderController.GetOrderById(1);
            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("The order doesn't exist.", badRequestObjectResult.Value);
        }

        [Fact]
        public async Task PayOrder_ShouldReturnOk_WhenOrderStateIsToBePaid()
        {
            // Arrange
            var buyer = new User { Id = 1, Name = "Lisa", Password = "lisa123", Type = UserType.BUYER };
            var seller = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 100, User = seller };
            var order = new Order() { Quantity = 10, Status = OrderState.TO_BE_PAID, Product = product, User = buyer };
            _orderServiceMock.Setup(x => x.GetOrderById(It.IsAny<int>())).ReturnsAsync(order);
            _orderServiceMock
                .Setup(service => service.UpdateOrderState(It.IsAny<int>(), It.IsAny<OrderState>()))
                .Returns(Task.CompletedTask);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "Lisa"),
                new Claim(ClaimTypes.Role, "Buyer")
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            _orderController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = principal
                }
            };
            // Act
            var result = await _orderController.PayOrder(order.Id);
            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Payment successful.", okObjectResult.Value);
        }

        [Fact]
        public async Task PayOrder_ShouldReturnBadRequest_WhenOrderIsNotfound()
        {
            // Arrange
            _orderServiceMock.Setup(x => x.GetOrderById(It.IsAny<int>())).ThrowsAsync(new OrderNotFoundException("The order doesn't exist."));
            // Act
            var result = await _orderController.PayOrder(1);
            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("The order doesn't exist.", badRequestObjectResult.Value);
        }

        [Fact]
        public async Task PayOrder_ShouldReturnBadRequest_WhenThisOrderIsNotForThisClaim()
        {
            // Arrange
            var buyer = new User { Id = 1, Name = "Lisa", Password = "lisa123", Type = UserType.BUYER };
            var seller = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 100, User = seller };
            var order = new Order() { Quantity = 10, Status = OrderState.TO_BE_PAID, Product = product, User = buyer };
            _orderServiceMock.Setup(x => x.GetOrderById(It.IsAny<int>())).ReturnsAsync(order);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "2"),
                new Claim(ClaimTypes.Name, "Rose"),
                new Claim(ClaimTypes.Role, "Buyer")
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            _orderController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = principal
                }
            };
            // Act
            var result = await _orderController.PayOrder(order.Id);
            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("This order is not yours.", badRequestObjectResult.Value);
        }

        [Fact]
        public async Task PayOrder_ShouldReturnBadRequest_WhenOrderStateIsNotToBePaid()
        {
            // Arrange
            var buyer = new User { Id = 1, Name = "Lisa", Password = "lisa123", Type = UserType.BUYER };
            var seller = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 100, User = seller };
            var order = new Order() { Quantity = 10, Status = OrderState.PAID, Product = product, User = buyer };
            _orderServiceMock.Setup(x => x.GetOrderById(It.IsAny<int>())).ReturnsAsync(order);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "Lisa"),
                new Claim(ClaimTypes.Role, "Buyer")
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            _orderController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = principal
                }
            };
            // Act
            var result = await _orderController.PayOrder(order.Id);
            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Current order is not payable.", badRequestObjectResult.Value);
        }

        [Fact]
        public async Task ConfirmReceipt_ShouldReturnOk_WhenOrderStateIsShipped()
        {
            // Arrange
            var buyer = new User { Id = 1, Name = "Lisa", Password = "lisa123", Type = UserType.BUYER };
            var seller = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 100, User = seller };
            var order = new Order() { Quantity = 10, Status = OrderState.SHIPPED, Product = product, User = buyer };
            _orderServiceMock.Setup(x => x.GetOrderById(It.IsAny<int>())).ReturnsAsync(order);
            _orderServiceMock
                .Setup(service => service.UpdateOrderState(It.IsAny<int>(), It.IsAny<OrderState>()))
                .Returns(Task.CompletedTask);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "Lisa"),
                new Claim(ClaimTypes.Role, "Buyer")
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            _orderController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = principal
                }
            };
            // Act
            var result = await _orderController.ConfirmReceipt(order.Id);
            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Received the goods successfully.", okObjectResult.Value);
        }

        [Fact]
        public async Task ConfirmReceipt_ShouldReturnBadRequest_WhenOrderIsNotfound()
        {
            // Arrange
            _orderServiceMock.Setup(x => x.GetOrderById(It.IsAny<int>())).ThrowsAsync(new OrderNotFoundException("The order doesn't exist."));
            // Act
            var result = await _orderController.ConfirmReceipt(1);
            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("The order doesn't exist.", badRequestObjectResult.Value);
        }

        [Fact]
        public async Task ConfirmReceipt_ShouldReturnBadRequest_WhenThisOrderIsNotForThisClaim()
        {
            // Arrange
            var buyer = new User { Id = 1, Name = "Lisa", Password = "lisa123", Type = UserType.BUYER };
            var seller = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 100, User = seller };
            var order = new Order() { Quantity = 10, Status = OrderState.SHIPPED, Product = product, User = buyer };
            _orderServiceMock.Setup(x => x.GetOrderById(It.IsAny<int>())).ReturnsAsync(order);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "2"),
                new Claim(ClaimTypes.Name, "Rose"),
                new Claim(ClaimTypes.Role, "Buyer")
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            _orderController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = principal
                }
            };
            // Act
            var result = await _orderController.ConfirmReceipt(order.Id);
            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("This order is not yours.", badRequestObjectResult.Value);
        }

        [Fact]
        public async Task ConfirmReceipt_ShouldReturnBadRequest_WhenOrderStateIsNotShipped()
        {
            // Arrange
            var buyer = new User { Id = 1, Name = "Lisa", Password = "lisa123", Type = UserType.BUYER };
            var seller = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 100, User = seller };
            var order = new Order() { Quantity = 10, Status = OrderState.PAID, Product = product, User = buyer };
            _orderServiceMock.Setup(x => x.GetOrderById(It.IsAny<int>())).ReturnsAsync(order);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "Lisa"),
                new Claim(ClaimTypes.Role, "Buyer")
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            _orderController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = principal
                }
            };
            // Act
            var result = await _orderController.ConfirmReceipt(order.Id);
            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Current order is not receivable.", badRequestObjectResult.Value);
        }

        [Fact]
        public async Task ShipOrder_ShouldReturnOk_WhenOrderStateIsPaid()
        {
            // Arrange
            var buyer = new User { Name = "Lisa", Password = "lisa123", Type = UserType.BUYER };
            var seller = new User { Id = 2, Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 100, User = seller };
            var order = new Order() { Quantity = 10, Status = OrderState.PAID, Product = product, User = buyer };
            _orderServiceMock.Setup(x => x.GetOrderById(It.IsAny<int>())).ReturnsAsync(order);
            _orderServiceMock
                .Setup(service => service.UpdateOrderState(It.IsAny<int>(), It.IsAny<OrderState>()))
                .Returns(Task.CompletedTask);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "2"),
                new Claim(ClaimTypes.Name, "Jack"),
                new Claim(ClaimTypes.Role, "Seller")
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            _orderController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = principal
                }
            };
            // Act
            var result = await _orderController.ShipOrder(order.Id);
            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Delivery successful.", okObjectResult.Value);
        }

        [Fact]
        public async Task ShipOrder_ShouldReturnBadRequest_WhenOrderIsNotfound()
        {
            // Arrange
            _orderServiceMock.Setup(x => x.GetOrderById(It.IsAny<int>())).ThrowsAsync(new OrderNotFoundException("The order doesn't exist."));
            // Act
            var result = await _orderController.ShipOrder(1);
            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("The order doesn't exist.", badRequestObjectResult.Value);
        }

        [Fact]
        public async Task ShipOrder_ShouldReturnBadRequest_WhenThisOrderIsNotForThisClaim()
        {
            // Arrange
            var buyer = new User { Name = "Lisa", Password = "lisa123", Type = UserType.BUYER };
            var seller = new User { Id = 2, Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 100, User = seller };
            var order = new Order() { Quantity = 10, Status = OrderState.PAID, Product = product, User = buyer };
            _orderServiceMock.Setup(x => x.GetOrderById(It.IsAny<int>())).ReturnsAsync(order);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "Lisa"),
                new Claim(ClaimTypes.Role, "Buyer")
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            _orderController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = principal
                }
            };
            // Act
            var result = await _orderController.ShipOrder(order.Id);
            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("This order is not yours.", badRequestObjectResult.Value);
        }

        [Fact]
        public async Task ShipOrder_ShouldReturnBadRequest_WhenOrderStateIsNotPaid()
        {
            // Arrange
            var buyer = new User { Name = "Lisa", Password = "lisa123", Type = UserType.BUYER };
            var seller = new User { Id = 2, Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 100, User = seller };
            var order = new Order() { Quantity = 10, Status = OrderState.TO_BE_PAID, Product = product, User = buyer };
            _orderServiceMock.Setup(x => x.GetOrderById(It.IsAny<int>())).ReturnsAsync(order);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "2"),
                new Claim(ClaimTypes.Name, "Jack"),
                new Claim(ClaimTypes.Role, "Seller")
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            _orderController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = principal
                }
            };
            // Act
            var result = await _orderController.ShipOrder(order.Id);
            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Current order is not shippable.", badRequestObjectResult.Value);
        }

        [Fact]
        public async Task GetOrderListBySellerId_ShouldReturnOk_WhenOrdersIsfound()
        {
            // Arrange
            var seller = new User { Id = 1, Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var buyer = new User { Id = 2, Name = "Lisa", Password = "lisa123", Type = UserType.BUYER };
            var product = new Product { Id = 1, Name = "Apple", Quantity = 100, User = seller };
            var serviceResult = new List<Order>{
                new Order{ Quantity = 10, Status = OrderState.TO_BE_PAID, User = buyer, Product = product }
            };
            _orderServiceMock.Setup(x => x.GetOrderListBySellerId(It.IsAny<int>())).ReturnsAsync(serviceResult);
            _productServiceMock.Setup(p => p.GetProductById(It.IsAny<int>())).ReturnsAsync(product);
            _userServiceMock.Setup(u => u.GetBuyerById(It.IsAny<int>())).ReturnsAsync(buyer);
            var resultItem = new List<SellerOrder>{
                new SellerOrder{ ProductId = 1, ProductName = "Apple", Quantity = 10, BuyerId = 2, BuyerName = "Lisa", Status = OrderState.TO_BE_PAID }
            };
            // Act
            var result = await _orderController.GetOrderListBySellerId(1);
            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okObjectResult.Value);
            Assert.Equal(resultItem.ToString(), okObjectResult.Value.ToString());
        }

        [Fact]
        public async Task GetOrderListBySellerId_ShouldReturnOk_WhenSellerIsNotfound()
        {
            // Arrange
            _orderServiceMock.Setup(x => x.GetOrderListBySellerId(It.IsAny<int>())).ThrowsAsync(new SellerNotFoundException("The seller doesn't exist."));
            // Act
            var result = await _orderController.GetOrderListBySellerId(1);
            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("The seller doesn't exist.", badRequestObjectResult.Value);
        }
    }
}