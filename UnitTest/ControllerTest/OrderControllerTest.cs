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
            var orderRequest = new OrderRequestModel { ProductId = 1, Quantity = 10, BuyerId = 1 };
            var product = new Product { Name = "Apple", Quantity = 100, SellerId = 1 };
            _productServiceMock.Setup(repository => repository.GetProductById(orderRequest.ProductId)).Returns(product);
            var user = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            _userServiceMock.Setup(repository => repository.GetUserById(orderRequest.BuyerId)).Returns(user);
            var expectedOrder = new Order { Id = 1, ProductId = 1, Quantity = 10, BuyerId = 1, Type = OrderType.TO_BE_PAID, Product = product, User = user };
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
    }
}