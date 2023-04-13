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
        private readonly OrderController _orderController;

        public OrderControllerTest()
        {
            _orderServiceMock = new Mock<IOrderService>();
            _productServiceMock = new Mock<IProductService>();
            _userServiceMock = new Mock<IUserService>();
            _orderController = new OrderController(_orderServiceMock.Object, _productServiceMock.Object, _userServiceMock.Object);
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
            var expectedOrder = new Order { Id = 1, Quantity = 10, Type = OrderType.TO_BE_PAID, Product = product, User = user };
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

            var buyer = new User { Name = "Lisa", Password = "lisa123", Type = UserType.BUYER };
            var seller = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 100, User = seller };
            var order = new Order() { Quantity = 10, Type = OrderType.TO_BE_PAID, Product = product, User = buyer };
            var resultItem = new BuyerOrder() { Id = 1, ProductName = "Apple", Quantity = 10, SellerName = "Jack", BuyerName = "Lisa", Type = OrderType.TO_BE_PAID };
            _orderServiceMock.Setup(x => x.GetOrderById(It.IsAny<int>())).ReturnsAsync(order);
            // Act
            var result = await _orderController.GetOrderById(1);
            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okObjectResult.Value);
            Assert.Equal(resultItem.ToString(), okObjectResult.Value.ToString());
        }

        [Fact]
        public async Task GetOrderById_ShouldReturnNotFoundException_WhenOrderIsNotfound()
        {
            _orderServiceMock.Setup(x => x.GetOrderById(It.IsAny<int>())).ThrowsAsync(new KeyNotFoundException());
            // Act
            var result = await _orderController.GetOrderById(1);
            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}