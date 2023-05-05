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
            var user = _context.Users.Single(u => u.Id == 1);
            var product = _context.Products.Single(p => p.Id == 1);
            var carts = new List<CartItem>
            {
                new CartItem { Product = product, Quantity = 10, User = user }
            };
            await _context.AddRangeAsync(carts);
            await _context.SaveChangesAsync();
            return carts;
        }

        private async Task<List<Product>> AddProducts()
        {
            var user = _context.Users.Single(u => u.Id == 1);
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Apple", Quantity = 100, User = user },
                new Product { Id = 2, Name = "Banana", Quantity = 50, User = user }
            };
            await _context.AddRangeAsync(products);
            await _context.SaveChangesAsync();
            return products;
        }

        private async Task<List<User>> AddUsers()
        {
            var users = new List<User>
        {
            new User { Id = 1, Name = "Lisa", Password = "lisa123", Type = UserType.BUYER } ,
            new User { Id = 2, Name = "Jack", Password = "Jack123", Type = UserType.SELLER }
        };
            await _context.AddRangeAsync(users);
            await _context.SaveChangesAsync();
            return users;
        }

        [Fact]
        public async Task AddCartItem_ShouldCreateNewCartItem_WhenCartItemIsValid()
        {
            // Arrange
            var users = await AddUsers();
            var products = await AddProducts();
            var cart = new CartItem { Product = products[0], Quantity = 10, User = users[0] };
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
            var users = await AddUsers();
            var products = await AddProducts();
            var cart = await AddCartItems();
            // Act
            var result = await _repository.GetCartItemById(cart[0].Id);
            // Assert
            Assert.NotNull(result);
            Assert.Equal(cart[0], result);
        }

        [Fact]
        public async Task GetCartItemByProductIdAndBuyerId_ShouldReturnCartItem()
        {
            // Arrange
            var users = await AddUsers();
            var products = await AddProducts();
            var carts = await AddCartItems();
            // Act
            var result = await _repository.GetCartItemByProductIdAndBuyerId(1, 1);
            // Assert
            Assert.NotNull(result);
            Assert.Equal(carts[0], result);
        }
    }
}