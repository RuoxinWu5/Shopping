using Data.Exceptions;
using Data.Model;
using Data.Repository;
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

        public CartItemServiceTest()
        {
            _cartRepositoryMock = new Mock<ICartItemRepository>();
            _productRepositoryMock = new Mock<IProductRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _cartService = new CartItemService(_cartRepositoryMock.Object, _productRepositoryMock.Object, _userRepositoryMock.Object);
        }

        [Fact]
        public async Task AddCartItem_ShouldCallAddCartItemMethodOfRepository_WhenQuantityIsEnough()
        {
            // Arrange
            var user = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 100, User = user };
            var cartItem = new CartItem { Product = product, Quantity = 10, User = user };
            // Act
            var result = await _cartService.AddCartItem(cartItem);
            // Assert
            _cartRepositoryMock.Verify(repository => repository.AddCartItem(result), Times.Once);
        }

        [Fact]
        public async Task AddCartItem_ShouldThrowArgumentException_WhenQuantityIsNotEnough()
        {
            // Arrange
            var user = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 10, User = user };
            var cartItem = new CartItem { Product = product, Quantity = 100, User = user };
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await _cartService.AddCartItem(cartItem));
        }

        [Fact]
        public async Task GetCartItemById_ShouldReturnCartItem_WhenCartItemExist()
        {
            // Arrange
            var id = 1;
            var user = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Name = "Apple", Quantity = 10, User = user };
            var cartItem = new CartItem { Product = product, Quantity = 100, User = user };
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