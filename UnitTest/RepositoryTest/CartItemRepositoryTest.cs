using Data.Model;
using Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace UnitTest.RepositoryTest
{
    public class CartItemRepositoryTest
    {
        private readonly ShoppingDbContext _context;
        private readonly CartItemRepository _repository;
        public CartItemRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<ShoppingDbContext>()
                    .UseInMemoryDatabase("CartList")
                    .Options;
            _context = new ShoppingDbContext(options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
            _repository = new CartItemRepository(_context);
        }

        private async Task<List<CartItem>> AddCartItems()
        {
            var user = new User { Id = 1, Name = "Lisa", Password = "lisa123", Type = UserType.BUYER };
            var product = new Product { Id = 1, Name = "Apple", Quantity = 100, User = user };
            var carts = new List<CartItem>
            {
                new CartItem { Id = 1, Product = product, Quantity = 10, User = user }
            };
            await _context.AddRangeAsync(carts);
            await _context.SaveChangesAsync();
            return carts;
        }

        [Fact]
        public async Task AddCartItem_ShouldCreateNewCartItem_WhenCartItemIsValid()
        {
            // Arrange
            var user = new User { Id = 1, Name = "Lisa", Password = "lisa123", Type = UserType.BUYER };
            var product = new Product { Id = 1, Name = "Apple", Quantity = 100, User = user };
            var cart = new CartItem { Product = product, Quantity = 10, User = user };
            // Act
            await _repository.AddCartItem(cart);
            // Assert
            Assert.NotNull(_context.CartItems);
            var savedCartItem = await _context.CartItems.FirstOrDefaultAsync(u => u.Product.Id == cart.Product.Id && u.User.Id == cart.User.Id);
            Assert.NotNull(savedCartItem);
            Assert.Equal(cart, savedCartItem);
        }

        [Fact]
        public async Task GetCartItemById_ShouldReturnCartItem()
        {
            // Arrange
            var cart = await AddCartItems();
            // Act
            var result = await _repository.GetCartItemById(1);
            // Assert
            Assert.NotNull(result);
            Assert.Equal(cart[0], result);
        }

        [Fact]
        public async Task GetCartItemByProductIdAndBuyerId_ShouldReturnCartItem()
        {
            // Arrange
            var carts = await AddCartItems();
            // Act
            var result = await _repository.GetCartItemByProductIdAndBuyerId(1, 1);
            // Assert
            Assert.NotNull(result);
            Assert.Equal(carts[0], result);
        }
    }
}