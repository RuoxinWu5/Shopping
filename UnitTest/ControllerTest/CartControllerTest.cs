using Data.Model;
using Data.RequestModel;
using Data.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Service;
using Shopping.Controller;

namespace UnitTest.ControllerTest
{
    public class CartControllerTest
    {
        private readonly Mock<ICartService> _cartServiceMock = new Mock<ICartService>();
        private readonly Mock<IProductService> _productServiceMock = new Mock<IProductService>();
        private readonly Mock<IUserService> _userServiceMock = new Mock<IUserService>();
        private readonly CartsController _cartController;
        public CartControllerTest()
        {
            _cartServiceMock = new Mock<ICartService>();
            _productServiceMock = new Mock<IProductService>();
            _userServiceMock = new Mock<IUserService>();
            _cartController = new CartsController(_cartServiceMock.Object, _productServiceMock.Object, _userServiceMock.Object);
        }

        [Fact]
        public async Task AddCartItem_ShouldReturnOk_WhenCartItemIsValid()
        {
            // Arrange
            var seller = new User { Id = 1, Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var buyer = new User { Id = 2, Name = "Lisa", Password = "Lisa123", Type = UserType.BUYER };
            var product = new Product { Id = 1, Name = "Apple", Quantity = 100, User = seller };
            var addProductToCartRequestModel = new AddProductToCartRequestModel { ProductId = 1, Quantity = 10, BuyerId = 2 };
            _productServiceMock.Setup(repository => repository.GetProductById(addProductToCartRequestModel.ProductId)).ReturnsAsync(product);
            _userServiceMock.Setup(repository => repository.GetBuyerById(addProductToCartRequestModel.BuyerId)).ReturnsAsync(buyer);
            var expectedCartItem = new CartItem { Id = 1, Product = product, Quantity = 10, User = buyer };
            _cartServiceMock.Setup(x => x.AddCartItem(It.IsAny<CartItem>())).ReturnsAsync(expectedCartItem);
            // Act
            var result = await _cartController.AddCartItem(addProductToCartRequestModel);
            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(expectedCartItem, createdResult.Value);
        }

        [Fact]
        public async Task AddCartItem_ShouldReturnBadRequest_WhenQuantityIsNotEnough()
        {
            // Arrange
            var addProductToCartRequestModel = new AddProductToCartRequestModel { ProductId = 1, Quantity = 10, BuyerId = 1 };
            _cartServiceMock.Setup(x => x.AddCartItem(It.IsAny<CartItem>())).Throws(new ArgumentException("Quantity not sufficient. CartItem creation failed."));
            // Act
            var result = await _cartController.AddCartItem(addProductToCartRequestModel);
            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Quantity not sufficient. CartItem creation failed.", badRequestObjectResult.Value);
        }

        [Fact]
        public async Task AddCartItem_ShouldReturnNotFound_WhenUserIdNotExists()
        {
            // Arrange
            var addProductToCartRequestModel = new AddProductToCartRequestModel { ProductId = 1, Quantity = 10, BuyerId = 1 };
            _cartServiceMock.Setup(x => x.AddCartItem(It.IsAny<CartItem>())).Throws(new KeyNotFoundException("The buyer doesn't exist."));
            // Act
            var result = await _cartController.AddCartItem(addProductToCartRequestModel);
            // Assert
            var createdResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("The buyer doesn't exist.", createdResult.Value);
        }

        [Fact]
        public async Task AddCartItem_ShouldReturnNotFound_WhenProductIdNotExists()
        {
            // Arrange
            var addProductToCartRequestModel = new AddProductToCartRequestModel { ProductId = 1, Quantity = 10, BuyerId = 1 };
            _cartServiceMock.Setup(x => x.AddCartItem(It.IsAny<CartItem>())).Throws(new KeyNotFoundException("The product doesn't exist."));
            // Act
            var result = await _cartController.AddCartItem(addProductToCartRequestModel);
            // Assert
            var createdResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("The product doesn't exist.", createdResult.Value);
        }

        [Fact]
        public async Task GetCartItemById_ShouldReturnOk_WhenCartItemIsfound()
        {
            // Arrange
            var seller = new User { Id = 1, Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var buyer = new User { Id = 2, Name = "Lisa", Password = "Lisa123", Type = UserType.BUYER };
            var product = new Product { Id = 1, Name = "Apple", Quantity = 100, User = seller };
            var cartItem = new CartItem { Product = product, Quantity = 10, User = buyer };
            var resultItem = new BuyerCartItem() { ProductName = "Apple", Quantity = 10, BuyerName = "Lisa" };
            _productServiceMock.Setup(p => p.GetProductById(It.IsAny<int>())).ReturnsAsync(product);
            _userServiceMock.Setup(u => u.GetBuyerById(It.IsAny<int>())).ReturnsAsync(buyer);
            _cartServiceMock.Setup(x => x.GetCartItemById(It.IsAny<int>())).ReturnsAsync(cartItem);
            // Act
            var result = await _cartController.GetCartItemById(cartItem.Id);
            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okObjectResult.Value);
            Assert.Equal(resultItem.ToString(), okObjectResult.Value.ToString());
        }

        [Fact]
        public async Task GetCartItemById_ShouldReturnNotFoundException_WhenCartItemIsNotfound()
        {
            // Arrange
            _cartServiceMock.Setup(x => x.GetCartItemById(It.IsAny<int>())).ThrowsAsync(new KeyNotFoundException());
            // Act
            var result = await _cartController.GetCartItemById(1);
            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}