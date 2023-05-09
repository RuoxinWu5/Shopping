using Data.Exceptions;
using Data.Model;
using Data.Repository;
using Data.RequestModel;
using Moq;
using Service;

namespace UnitTest.ServiceTest
{
    public class CartItemServiceTest
    {
        private readonly CartItemService _cartService;
        private readonly Mock<ICartItemRepository> _cartRepositoryMock;
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IProductService> _productServiceMock;
        private readonly Mock<IUserService> _userServiceMock;

        public CartItemServiceTest()
        {
            _cartRepositoryMock = new Mock<ICartItemRepository>();
            _productRepositoryMock = new Mock<IProductRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _productServiceMock = new Mock<IProductService>();
            _userServiceMock = new Mock<IUserService>();
            _cartService = new CartItemService(_cartRepositoryMock.Object, _productRepositoryMock.Object, _userRepositoryMock.Object, _productServiceMock.Object, _userServiceMock.Object);
        }

        private AddProductToCartRequestModel CreateAddProductToCartRequestModelItems()
        {
            var addProductToCartRequestModel = new AddProductToCartRequestModel { ProductId = 1, Quantity = 10, BuyerId = 2 };
            return addProductToCartRequestModel;
        }

        [Fact]
        public async Task AddCartItem_ShouldCallAddCartItemMethodOfRepository_WhenQuantityIsEnoughAndCartItemIsNotExist()
        {
            // Arrange
            CartItem? nullCartItem = null;
            _cartRepositoryMock.Setup(repository => repository.GetCartItemByProductIdAndBuyerId(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(nullCartItem);
            var seller = new User { Id = 1, Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Id = 1, Name = "Apple", Quantity = 100, User = seller };
            _productServiceMock.Setup(service => service.GetProductById(It.IsAny<int>())).ReturnsAsync(product);
            var buyer = new User { Id = 2, Name = "Lisa", Password = "Lisa123", Type = UserType.BUYER };
            _userServiceMock.Setup(service => service.GetBuyerById(It.IsAny<int>())).ReturnsAsync(buyer);
            var addProductToCartRequestModelItem = CreateAddProductToCartRequestModelItems();
            var cartItem = new CartItem { Product = product, Quantity = 10, User = buyer };
            // Act
            var result = await _cartService.AddCartItem(addProductToCartRequestModelItem);
            // Assert
            Assert.Equal(cartItem.ToString(), result.ToString());
            _cartRepositoryMock.Verify(repository => repository.AddCartItem(It.IsAny<CartItem>()), Times.Once);
        }

        [Fact]
        public async Task AddCartItem_ShouldCallUpdateCartItemMethodOfRepository_WhenQuantityIsEnoughAndCartItemIsExist()
        {
            // Arrange
            var seller = new User { Id = 1, Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Id = 1, Name = "Apple", Quantity = 100, User = seller };
            var buyer = new User { Id = 2, Name = "Lisa", Password = "Lisa123", Type = UserType.BUYER };
            var cartItem = new CartItem { Product = product, Quantity = 10, User = buyer };
            _cartRepositoryMock.Setup(repository => repository.GetCartItemByProductIdAndBuyerId(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(cartItem);
            var addProductToCartRequestModelItem = CreateAddProductToCartRequestModelItems();
            // Act
            var result = await _cartService.AddCartItem(addProductToCartRequestModelItem);
            // Assert
            Assert.Equal(cartItem.ToString(), result.ToString());
            _cartRepositoryMock.Verify(repository => repository.UpdateCartItem(It.IsAny<CartItem>()), Times.Once);
        }

        [Fact]
        public async Task AddCartItem_ShouldThrowArgumentException_WhenQuantityIsNotEnough()
        {
            // Arrange
            var addProductToCartRequestModel = new AddProductToCartRequestModel { ProductId = 1, Quantity = 100, BuyerId = 1 };
            CartItem? nullCartItem = null;
            _cartRepositoryMock.Setup(repository => repository.GetCartItemByProductIdAndBuyerId(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(nullCartItem);
            var seller = new User { Id = 1, Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Id = 1, Name = "Apple", Quantity = 10, User = seller };
            _productServiceMock.Setup(service => service.GetProductById(It.IsAny<int>())).ReturnsAsync(product);
            var buyer = new User { Id = 2, Name = "Lisa", Password = "Lisa123", Type = UserType.BUYER };
            _userServiceMock.Setup(service => service.GetBuyerById(It.IsAny<int>())).ReturnsAsync(buyer);
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await _cartService.AddCartItem(addProductToCartRequestModel));
        }

        [Fact]
        public async Task GetCartItemById_ShouldReturnCartItem_WhenCartItemExist()
        {
            // Arrange
            var id = 1;
            var seller = new User { Id = 1, Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Id = 1, Name = "Apple", Quantity = 100, User = seller };
            var buyer = new User { Id = 2, Name = "Lisa", Password = "Lisa123", Type = UserType.BUYER };
            var cartItem = new CartItem { Product = product, Quantity = 10, User = buyer };
            _cartRepositoryMock.Setup(repository => repository.GetCartItemById(It.IsAny<int>())).ReturnsAsync(cartItem);
            // Act
            var result = await _cartService.GetCartItemById(id);
            // Assert
            Assert.Equal(cartItem, result);
        }

        [Fact]
        public async Task GetCartItemById_ShouldThrowCartItemNotFoundException_WhenCartItemNotExist()
        {
            // Arrange
            var id = 1;
            CartItem? nullCartItem = null;
            _cartRepositoryMock.Setup(repository => repository.GetCartItemById(It.IsAny<int>())).ReturnsAsync(nullCartItem);
            // Act & Assert
            await Assert.ThrowsAsync<CartItemNotFoundException>(async () => await _cartService.GetCartItemById(id));
        }
    }
}