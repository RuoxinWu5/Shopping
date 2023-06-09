using System.Security.Claims;
using Data.Exceptions;
using Data.Model;
using Data.RequestModel;
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

        private void CreateUserIdentity(int userId, string userName, string userRole)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Role, userRole)
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
        }

        private void CreateNullUserIdentity()
        {
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);
            _orderController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = principal
                }
            };
        }

        [Fact]
        public async Task AddOrder_ShouldReturnOk_WhenOrderIsValid()
        {
            // Arrange
            var seller = new User { Id = 1, Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Id = 1, Name = "Apple", Quantity = 100, User = seller };
            _productServiceMock
                .Setup(repository => repository.GetProductById(It.IsAny<int>()))
                .ReturnsAsync(product);

            var buyer = new User { Id = 2, Name = "Lisa", Password = "lisa123", Type = UserType.BUYER };
            _userServiceMock
                .Setup(repository => repository.GetBuyerById(It.IsAny<int>()))
                .ReturnsAsync(buyer);

            var orderRequest = new AddOrderRequestModel { ProductId = 1, Quantity = 10, BuyerId = 1 };

            // Act
            var result = await _orderController.AddOrder(orderRequest);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.NotNull(createdResult.Value);
            var expectedOrder = new Order { Quantity = 10, Status = OrderStatus.TO_BE_PAID, Product = product, User = buyer };
            Assert.Equivalent(expectedOrder, createdResult.Value);
        }

        [Fact]
        public async Task AddOrder_ShouldReturnBadRequest_WhenProductIdNotExists()
        {
            // Arrange
            _productServiceMock
                .Setup(x => x.GetProductById(It.IsAny<int>()))
                .Throws(new ProductNotFoundException("The product doesn't exist."));

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
            var seller = new User { Id = 1, Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Id = 1, Name = "Apple", Quantity = 100, User = seller };
            _productServiceMock
                .Setup(repository => repository.GetProductById(It.IsAny<int>()))
                .ReturnsAsync(product);

            _userServiceMock
                .Setup(x => x.GetBuyerById(It.IsAny<int>()))
                .Throws(new BuyerNotFoundException("The buyer doesn't exist."));

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
            var seller = new User { Id = 1, Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Id = 1, Name = "Apple", Quantity = 100, User = seller };
            _productServiceMock
                .Setup(repository => repository.GetProductById(It.IsAny<int>()))
                .ReturnsAsync(product);

            var buyer = new User { Id = 2, Name = "Lisa", Password = "lisa123", Type = UserType.BUYER };
            _userServiceMock
                .Setup(repository => repository.GetBuyerById(It.IsAny<int>()))
                .ReturnsAsync(buyer);

            _orderServiceMock
                .Setup(x => x.AddOrderAndReduceProductQuantity(It.IsAny<Order>()))
                .ThrowsAsync(new ArgumentException("Quantity not sufficient. Order creation failed."));

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
            var seller = new User { Id = 1, Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Id = 1, Name = "Apple", Quantity = 100, User = seller };
            _productServiceMock
                .Setup(repository => repository.GetProductById(It.IsAny<int>()))
                .ReturnsAsync(product);

            var buyer = new User { Id = 2, Name = "Lisa", Password = "lisa123", Type = UserType.BUYER };
            _userServiceMock
                .Setup(repository => repository.GetBuyerById(It.IsAny<int>()))
                .ReturnsAsync(buyer);

            var order = new Order() { Quantity = 10, Status = OrderStatus.TO_BE_PAID, Product = product, User = buyer };
            _orderServiceMock
                .Setup(x => x.GetOrderById(It.IsAny<int>()))
                .ReturnsAsync(order);

            // Act
            var result = await _orderController.GetOrderById(order.Id);

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okObjectResult.Value);
            var resultItem = new BuyerOrder() { ProductName = "Apple", Quantity = 10, SellerName = "Jack", BuyerName = "Lisa", Status = OrderStatus.TO_BE_PAID };
            Assert.Equivalent(resultItem, okObjectResult.Value);
        }

        [Fact]
        public async Task GetOrderById_ShouldReturnBadRequest_WhenOrderIsNotfound()
        {
            // Arrange
            _orderServiceMock
                .Setup(x => x.GetOrderById(It.IsAny<int>()))
                .ThrowsAsync(new OrderNotFoundException("The order doesn't exist."));

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
            var orderId = 1;
            CreateUserIdentity(1, "Lisa", "Buyer");

            // Act
            var result = await _orderController.PayOrder(orderId);

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Payment successful.", okObjectResult.Value);
        }

        [Fact]
        public async Task PayOrder_ShouldReturnBadRequest_WhenUserIsNotAuthenticated()
        {
            // Arrange
            CreateNullUserIdentity();
            var orderId = 1;

            // Act
            var result = await _orderController.PayOrder(orderId);

            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("You are not authorized to pay for this order.", badRequestObjectResult.Value);
        }

        [Fact]
        public async Task PayOrder_ShouldReturnBadRequest_WhenOrderIsNotFound()
        {
            // Arrange
            _orderServiceMock
                .Setup(x => x.PayOrder(It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new OrderNotFoundException("The order doesn't exist."));
            CreateUserIdentity(1, "Lisa", "Buyer");
            var orderId = 1;

            // Act
            var result = await _orderController.PayOrder(orderId);

            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("The order doesn't exist.", badRequestObjectResult.Value);
        }

        [Fact]
        public async Task PayOrder_ShouldReturnBadRequest_WhenThisOrderIsNotForThisClaim()
        {
            // Arrange
            _orderServiceMock
                .Setup(service => service.PayOrder(It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new OrderOwnershipException("This order is not yours."));
            CreateUserIdentity(1, "Lisa", "Buyer");
            var orderId = 1;

            // Act
            var result = await _orderController.PayOrder(orderId);

            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("This order is not yours.", badRequestObjectResult.Value);
        }

        [Fact]
        public async Task PayOrder_ShouldReturnBadRequest_WhenOrderStateIsNotToBePaid()
        {
            // Arrange
            _orderServiceMock
                .Setup(service => service.PayOrder(It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new OrderStatusModificationException("Current order is not payable."));
            CreateUserIdentity(1, "Lisa", "Buyer");
            var orderId = 1;

            // Act
            var result = await _orderController.PayOrder(orderId);

            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Current order is not payable.", badRequestObjectResult.Value);
        }

        [Fact]
        public async Task ConfirmReceipt_ShouldReturnOk_WhenOrderStateIsShipped()
        {
            // Arrange
            CreateUserIdentity(1, "Lisa", "Buyer");
            var orderId = 1;

            // Act
            var result = await _orderController.ConfirmReceipt(orderId);

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Received the goods successfully.", okObjectResult.Value);
        }

        [Fact]
        public async Task ConfirmReceipt_ShouldReturnBadRequest_WhenUserIsNotAuthenticated()
        {
            // Arrange
            CreateNullUserIdentity();
            var orderId = 1;

            // Act
            var result = await _orderController.ConfirmReceipt(orderId);

            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("You are not authorized to confirm the receipt of this order.", badRequestObjectResult.Value);
        }

        [Fact]
        public async Task ConfirmReceipt_ShouldReturnBadRequest_WhenOrderIsNotfound()
        {
            // Arrange
            _orderServiceMock
                .Setup(x => x.ConfirmReceipt(It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new OrderNotFoundException("The order doesn't exist."));
            CreateUserIdentity(1, "Lisa", "Buyer");
            var orderId = 1;

            // Act
            var result = await _orderController.ConfirmReceipt(orderId);

            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("The order doesn't exist.", badRequestObjectResult.Value);
        }

        [Fact]
        public async Task ConfirmReceipt_ShouldReturnBadRequest_WhenThisOrderIsNotForThisClaim()
        {
            // Arrange
            _orderServiceMock
                .Setup(service => service.ConfirmReceipt(It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new OrderOwnershipException("This order is not yours."));
            CreateUserIdentity(1, "Lisa", "Buyer");
            var orderId = 1;

            // Act
            var result = await _orderController.ConfirmReceipt(orderId);

            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("This order is not yours.", badRequestObjectResult.Value);
        }

        [Fact]
        public async Task ConfirmReceipt_ShouldReturnBadRequest_WhenOrderStateIsNotShipped()
        {
            // Arrange
            _orderServiceMock
                .Setup(service => service.ConfirmReceipt(It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new OrderStatusModificationException("Current order is not receivable."));
            CreateUserIdentity(1, "Lisa", "Buyer");
            var orderId = 1;

            // Act
            var result = await _orderController.ConfirmReceipt(orderId);

            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Current order is not receivable.", badRequestObjectResult.Value);
        }

        [Fact]
        public async Task ShipOrder_ShouldReturnOk_WhenOrderStateIsPaid()
        {
            // Arrange
            CreateUserIdentity(1, "Jack", "Seller");
            var orderId = 1;

            // Act
            var result = await _orderController.ShipOrder(orderId);

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Delivery successful.", okObjectResult.Value);
        }

        [Fact]
        public async Task ShipOrder_ShouldReturnBadRequest_WhenUserIsNotAuthenticated()
        {
            // Arrange
            CreateNullUserIdentity();
            var orderId = 1;

            // Act
            var result = await _orderController.ShipOrder(orderId);

            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("You are not authorized to ship this order.", badRequestObjectResult.Value);
        }

        [Fact]
        public async Task ShipOrder_ShouldReturnBadRequest_WhenOrderIsNotfound()
        {
            // Arrange
            _orderServiceMock
                .Setup(service => service.ShipOrder(It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new OrderNotFoundException("The order doesn't exist."));
            CreateUserIdentity(1, "Jack", "Seller");
            var orderId = 1;

            // Act
            var result = await _orderController.ShipOrder(orderId);

            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("The order doesn't exist.", badRequestObjectResult.Value);
        }

        [Fact]
        public async Task ShipOrder_ShouldReturnBadRequest_WhenThisOrderIsNotForThisClaim()
        {
            // Arrange
            _orderServiceMock
                .Setup(service => service.ShipOrder(It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new OrderOwnershipException("This order is not yours."));
            CreateUserIdentity(1, "Jack", "Seller");
            var orderId = 1;

            // Act
            var result = await _orderController.ShipOrder(orderId);

            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("This order is not yours.", badRequestObjectResult.Value);
        }

        [Fact]
        public async Task ShipOrder_ShouldReturnBadRequest_WhenOrderStateIsNotPaid()
        {
            // Arrange
            _orderServiceMock
                .Setup(service => service.ShipOrder(It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new OrderStatusModificationException("Current order is not shippable."));
            CreateUserIdentity(1, "Jack", "Seller");
            var orderId = 1;

            // Act
            var result = await _orderController.ShipOrder(orderId);

            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Current order is not shippable.", badRequestObjectResult.Value);
        }

        [Fact]
        public async Task GetOrderListBySellerId_ShouldReturnOk_WhenOrdersIsfound()
        {
            // Arrange
            var seller = new User { Id = 1, Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Id = 1, Name = "Apple", Quantity = 100, User = seller };
            _productServiceMock
                .Setup(p => p.GetProductById(It.IsAny<int>()))
                .ReturnsAsync(product);

            var buyer = new User { Id = 2, Name = "Lisa", Password = "lisa123", Type = UserType.BUYER };
            _userServiceMock
                .Setup(u => u.GetBuyerById(It.IsAny<int>()))
                .ReturnsAsync(buyer);

            var serviceResult = new List<Order>{
                new Order{ Quantity = 10, Status = OrderStatus.TO_BE_PAID, User = buyer, Product = product }
            };
            _orderServiceMock
                .Setup(x => x.GetOrderListBySellerId(It.IsAny<int>()))
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _orderController.GetOrderListBySellerId(1);

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okObjectResult.Value);
            var resultItem = new List<SellerOrder>{
                new SellerOrder{ ProductId = 1, ProductName = "Apple", Quantity = 10, BuyerId = 2, BuyerName = "Lisa", Status = OrderStatus.TO_BE_PAID }
            };
            Assert.Equivalent(resultItem, okObjectResult.Value);
        }

        [Fact]
        public async Task GetOrderListBySellerId_ShouldReturnBadRequest_WhenSellerIsNotfound()
        {
            // Arrange
            _orderServiceMock
                .Setup(x => x.GetOrderListBySellerId(It.IsAny<int>()))
                .ThrowsAsync(new SellerNotFoundException("The seller doesn't exist."));

            // Act
            var result = await _orderController.GetOrderListBySellerId(1);

            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("The seller doesn't exist.", badRequestObjectResult.Value);
        }

        [Fact]
        public async Task AddOrderFromCartItem_ShouldReturnOk_WhenCartItemIsValid()
        {
            // Arrange
            var seller = new User { Id = 1, Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var buyer = new User { Id = 2, Name = "Lisa", Password = "lisa123", Type = UserType.BUYER };
            var product = new Product { Id = 1, Name = "Apple", Quantity = 100, User = seller };
            var expectedOrder = new Order { Id = 1, Quantity = 10, Status = OrderStatus.TO_BE_PAID, Product = product, User = buyer };
            _orderServiceMock
                .Setup(service => service.AddOrderFromCartItem(It.IsAny<AddOrderFromCartItemRequestModel>()))
                .ReturnsAsync(expectedOrder);

            var addOrderFromCartItemRequestModel = new AddOrderFromCartItemRequestModel { BuyerId = 2, CartItemId = 1 };

            // Act
            var result = await _orderController.AddOrderFromCartItem(addOrderFromCartItemRequestModel);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.NotNull(createdResult.Value);
            Assert.Equal(expectedOrder, createdResult.Value);
        }

        [Fact]
        public async Task AddOrderFromCartItem_ShouldReturnBadRequest_WhenCartItemNotFound()
        {
            // Arrange
            _orderServiceMock
                .Setup(service => service.AddOrderFromCartItem(It.IsAny<AddOrderFromCartItemRequestModel>()))
                .ThrowsAsync(new CartItemNotFoundException("The cart item doesn't exist."));

            var addOrderFromCartItemRequestModel = new AddOrderFromCartItemRequestModel { BuyerId = 2, CartItemId = 1 };

            // Act
            var result = await _orderController.AddOrderFromCartItem(addOrderFromCartItemRequestModel);

            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("The cart item doesn't exist.", badRequestObjectResult.Value);
        }

        [Fact]
        public async Task AddOrderFromCartItem_ShouldReturnBadRequest_WhenCartItemIsNotForThisUser()
        {
            // Arrange
            _orderServiceMock
                .Setup(service => service.AddOrderFromCartItem(It.IsAny<AddOrderFromCartItemRequestModel>()))
                .ThrowsAsync(new CartItemOwnershipException("This cart item is not yours."));
            var addOrderFromCartItemRequestModel = new AddOrderFromCartItemRequestModel { BuyerId = 2, CartItemId = 1 };

            // Act
            var result = await _orderController.AddOrderFromCartItem(addOrderFromCartItemRequestModel);

            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("This cart item is not yours.", badRequestObjectResult.Value);
        }

        [Fact]
        public async Task AddOrderFromCartItem_ShouldReturnBadRequest_WhenQuantityIsNotEnough()
        {
            // Arrange
            _orderServiceMock
                .Setup(service => service.AddOrderFromCartItem(It.IsAny<AddOrderFromCartItemRequestModel>()))
                .ThrowsAsync(new ArgumentException("Quantity not sufficient. Order creation failed."));

            var addOrderFromCartItemRequestModel = new AddOrderFromCartItemRequestModel { BuyerId = 2, CartItemId = 1 };

            // Act
            var result = await _orderController.AddOrderFromCartItem(addOrderFromCartItemRequestModel);

            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Quantity not sufficient. Order creation failed.", badRequestObjectResult.Value);
        }
    }
}