using Data.Model;
using Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace UnitTest.RepositoryTest
{
    public class OrderRepositoryTest
    {
        private readonly ShoppingDbContext _context;
        private readonly OrderRepository _repository;
        public OrderRepositoryTest()
        {
            var productOptions = new DbContextOptionsBuilder<ShoppingDbContext>()
                    .UseInMemoryDatabase("ProductList2")
                    .Options;
            _context = new ShoppingDbContext(productOptions);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
            _repository = new OrderRepository(_context);
        }

        private async Task<List<Product>> AddProducts()
        {
            var products = new List<Product>
            {
                new Product { Name = "Apple", Quantity = 100, SellerId = 2 },
                new Product { Name = "Banana", Quantity = 50, SellerId = 2 }
            };
            await _context.AddRangeAsync(products);
            await _context.SaveChangesAsync();
            return products;
        }

        private async Task<List<User>> AddUsers()
        {
            var users = new List<User>
        {
            new User { Name = "Lisa", Password = "lisa123", Type = UserType.BUYER } ,
            new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER }
        };
            await _context.AddRangeAsync(users);
            await _context.SaveChangesAsync();
            return users;
        }

        [Fact]
        public async Task AddOrder_ShouldCreateNewOrder_WhenOrderIsValid()
        {
            // Arrange
            var products = await AddProducts();
            var users = await AddUsers();
            var order = new Order { ProductId = 1, Quantity = 10, BuyerId = 1, Type = OrderType.TO_BE_PAID };
            // Act
            await _repository.AddOrder(order);
            // Assert
            Assert.NotNull(_context.Orders);
            var savedOrder = await _context.Orders.FirstOrDefaultAsync(u => u.ProductId == order.ProductId && u.BuyerId == order.BuyerId);
            Assert.NotNull(savedOrder);
            Assert.Equal(order, savedOrder);
        }

        [Fact]
        public async Task AddOrder_ShouldThrowNotFoundException_WhenProductIdNotExists()
        {
            // Arrange
            var products = await AddProducts();
            var users = await AddUsers();
            var order = new Order { ProductId = 3, Quantity = 10, BuyerId = 1, Type = OrderType.TO_BE_PAID };
            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _repository.AddOrder(order));
        }

        [Fact]
        public async Task AddOrder_ShouldThrowNotFoundException_WhenUserIdNotExists()
        {
            // Arrange
            var products = await AddProducts();
            var users = await AddUsers();
            var order = new Order { ProductId = 1, Quantity = 10, BuyerId = 3, Type = OrderType.TO_BE_PAID };
            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _repository.AddOrder(order));
        }
    }
}