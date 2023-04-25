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

        private async Task<List<CartItem>> AddCarts()
        {
            var users = _context.Users.ToList();
            var products = _context.Products.ToList();
            var cart = new List<CartItem>
            {
                new CartItem { Product = products[0], Quantity = 10, User = users[0] }
            };
            await _context.AddRangeAsync(cart);
            await _context.SaveChangesAsync();
            return cart;
        }

        private async Task<List<Product>> AddProducts()
        {
            var users = _context.Users.ToList();
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Apple", Quantity = 100, User = users[1] },
                new Product { Id = 2, Name = "Banana", Quantity = 50, User = users[1] }
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
        public async Task GetCartItemById_ShouldReturnCart_WhenCartIsfound()
        {
            // Arrange
            var users = await AddUsers();
            var products = await AddProducts();
            var cart = await AddCarts();
            // Act
            var result = await _repository.GetCartItemById(cart[0].Id);
            // Assert
            Assert.NotNull(result);
            Assert.Equal(cart[0].ToString(), result.ToString());
        }

        [Fact]
        public async Task GetCartItemById_ShouldReturnNotFoundException_WhenCartIsNotfound()
        {
            // Arrange
            var users = await AddUsers();
            var products = await AddProducts();
            var cart = await AddCarts();
            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _repository.GetCartItemById(3));
        }
    }
}