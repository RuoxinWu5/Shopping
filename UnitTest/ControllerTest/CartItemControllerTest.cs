using Data.Exceptions;
using Data.Model;
using Data.RequestModel;
using Data.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Service;
using Shopping.Controller;

namespace UnitTest.ControllerTest
{
    public class CartItemControllerTest
    {
        private readonly Mock<ICartItemService> _cartServiceMock = new Mock<ICartItemService>();
        private readonly Mock<IProductService> _productServiceMock = new Mock<IProductService>();
        private readonly Mock<IUserService> _userServiceMock = new Mock<IUserService>();
        private readonly CartItemsController _cartController;
        public CartItemControllerTest()
        {
            _cartServiceMock = new Mock<ICartItemService>();
            _productServiceMock = new Mock<IProductService>();
            _userServiceMock = new Mock<IUserService>();
            _cartController = new CartItemsController(_cartServiceMock.Object, _productServiceMock.Object, _userServiceMock.Object);
        }

        [Fact]
        public async Task AddCartItem_ShouldReturnOk_WhenCartItemIsValid()
        {
            // Arrange
            var seller = new User { Id = 1, Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Id = 1, Name = "Apple", Quantity = 100, User = seller };
            var buyer = new User { Id = 2, Name = "Lisa", Password = "Lisa123", Type = UserType.BUYER };
            var addProductToCartRequestModel = new AddProductToCartRequestModel { ProductId = 1, Quantity = 10, BuyerId = 2 };
            var expectedCartItem = new CartItem { Product = product, Quantity = 10, User = buyer };
            _cartServiceMock.Setup(service => service.AddCartItem(It.IsAny<AddProductToCartRequestModel>())).ReturnsAsync(expectedCartItem);
            // Act
            var result = await _cartController.AddCartItem(addProductToCartRequestModel);
            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.NotNull(createdResult.Value);
            Assert.Equal(expectedCartItem, createdResult.Value);
        }

        [Fact]
        public async Task AddCartItem_ShouldReturnBadRequest_WhenQuantityIsNotEnough()
        {
            // Arrange
            var addProductToCartRequestModel = new AddProductToCartRequestModel { ProductId = 1, Quantity = 10, BuyerId = 2 };
            _cartServiceMock.Setup(repository => repository.AddCartItem(It.IsAny<AddProductToCartRequestModel>())).Throws(new ArgumentException("Quantity not sufficient. CartItem creation failed."));
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
            var addProductToCartRequestModel = new AddProductToCartRequestModel { ProductId = 1, Quantity = 10, BuyerId = 2 };
            _cartServiceMock.Setup(service => service.AddCartItem(It.IsAny<AddProductToCartRequestModel>())).Throws(new BuyerNotFoundException("The buyer doesn't exist."));
            // Act
            var result = await _cartController.AddCartItem(addProductToCartRequestModel);
            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("The buyer doesn't exist.", badRequestObjectResult.Value);
        }

        [Fact]
        public async Task AddCartItem_ShouldReturnBadRequest_WhenProductIdNotExists()
        {
            // Arrange
            var addProductToCartRequestModel = new AddProductToCartRequestModel { ProductId = 1, Quantity = 10, BuyerId = 1 };
            _cartServiceMock.Setup(service => service.AddCartItem(It.IsAny<AddProductToCartRequestModel>())).Throws(new ProductNotFoundException("The product doesn't exist."));
            // Act
            var result = await _cartController.AddCartItem(addProductToCartRequestModel);
            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("The product doesn't exist.", badRequestObjectResult.Value);
        }

        [Fact]
        public async Task GetCartItemById_ShouldReturnOk_WhenCartItemIsfound()
        {
            // Arrange
            var seller = new User { Id = 1, Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Id = 1, Name = "Apple", Quantity = 100, User = seller };
            var buyer = new User { Id = 2, Name = "Lisa", Password = "Lisa123", Type = UserType.BUYER };
            var cartItem = new CartItem { Product = product, Quantity = 10, User = buyer };
            _cartServiceMock.Setup(x => x.GetCartItemById(It.IsAny<int>())).ReturnsAsync(cartItem);
            // Act
            var result = await _cartController.GetCartItemById(cartItem.Id);
            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okObjectResult.Value);
            Assert.Equal(cartItem, okObjectResult.Value);
        }

        [Fact]
        public async Task GetCartItemById_ShouldReturnBadRequest_WhenCartItemIsNotfound()
        {
            // Arrange
            _cartServiceMock.Setup(x => x.GetCartItemById(It.IsAny<int>())).ThrowsAsync(new CartItemNotFoundException("The cart item doesn't exist."));
            // Act
            var result = await _cartController.GetCartItemById(1);
            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("The cart item doesn't exist.", badRequestObjectResult.Value);
        }

        [Fact]
        public async Task GetCartItemListByBuyerId_ShouldReturnCartItemList_WhenCartItemListIsFound()
        {
            // Arrange
            int id = 1;
            var seller = new User { Id = 1, Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Id = 1, Name = "Apple", Quantity = 100, User = seller };
            var buyer = new User { Id = 2, Name = "Lisa", Password = "Lisa123", Type = UserType.BUYER };
            var cartItemList = new List<CartItem>
            {
                new CartItem { Product = product, Quantity = 10, User = buyer }
            };
            _cartServiceMock.Setup(service => service.GetCartItemListByBuyerId(It.IsAny<int>())).ReturnsAsync(cartItemList);
            var cartItemListResult = new List<BuyerCartItem>
            {
                new BuyerCartItem { ProductName = "Apple", Quantity = 10 }
            };
            // Act
            var result = await _cartController.GetCartItemListByBuyerId(id);
            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okObjectResult.Value);
            Assert.Equal(cartItemListResult.ToString(), okObjectResult.Value.ToString());
        }

        [Fact]
        public async Task GetCartItemListByBuyerId_ShouldReturnBadRequest_WhenBuyerIsNotFound()
        {
            // Arrange
            int id = 1;
            _cartServiceMock.Setup(service => service.GetCartItemListByBuyerId(It.IsAny<int>())).ThrowsAsync(new BuyerNotFoundException("The buyer doesn't exist."));
            // Act
            var result = await _cartController.GetCartItemListByBuyerId(id);
            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("The buyer doesn't exist.", badRequestObjectResult.Value);
        }
    }
}