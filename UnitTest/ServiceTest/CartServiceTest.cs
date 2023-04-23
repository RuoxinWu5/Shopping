using Data.Model;
using Data.Repository;
using Moq;
using Service;

namespace UnitTest.ServiceTest
{
    public class CartServiceTest
    {
        private readonly CartService _cartService;
        private readonly Mock<ICartRepository> _cartRepositoryMock;
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;

        public CartServiceTest()
        {
            _cartRepositoryMock = new Mock<ICartRepository>();
            _productRepositoryMock = new Mock<IProductRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _cartService = new CartService(_cartRepositoryMock.Object, _productRepositoryMock.Object, _userRepositoryMock.Object);
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
        public async Task GetCartItemById_ShouldCallGetOrderByIdMethodOfRepository()
        {
            // Arrange
            var id = 1;
            // Act
            await _cartService.GetCartItemById(id);
            // Assert
            _cartRepositoryMock.Verify(repository => repository.GetCartItemById(id), Times.Once);
        }
    }
}