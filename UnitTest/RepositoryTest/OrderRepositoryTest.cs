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

        private async Task<List<Order>> AddOrder()
        {
            var users = await AddUsers();
            var products = await AddProducts();
            var order = new List<Order>
            {
                new Order { Quantity = 10, Type = OrderType.TO_BE_PAID, Product = products[0], User = users[0] }
            };
            await _context.AddRangeAsync(order);
            await _context.SaveChangesAsync();
            return order;
        }

        private async Task<List<Product>> AddProducts()
        {
            var users = await AddUsers();
            var products = new List<Product>
            {
                new Product { Name = "Apple", Quantity = 100, User = users[1] },
                new Product { Name = "Banana", Quantity = 50, User = users[1] }
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
            var order = new Order { Quantity = 10, Type = OrderType.TO_BE_PAID, Product = products[0], User = users[0] };
            // Act
            await _repository.AddOrder(order);
            // Assert
            Assert.NotNull(_context.Orders);
            var savedOrder = await _context.Orders.FirstOrDefaultAsync(u => u.Product.Id == order.Product.Id && u.User.Id == order.User.Id);
            Assert.NotNull(savedOrder);
            Assert.Equal(order, savedOrder);
        }

        [Fact]
        public async Task GetOrderById_ShouldReturnOrder_WhenOrderIsfound()
        {
            // Arrange
            var order = await AddOrder();
            // Act
            var result = await _repository.GetOrderById(order[0].Id);
            // Assert
            Assert.Equal(order[0].ToString(), result.ToString());
        }

        [Fact]
        public async Task GetOrderById_ShouldReturnNotFoundException_WhenOrderIsNotfound()
        {
            // Arrange
            var products = await AddProducts();
            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _repository.GetOrderById(1));
        }
    }
}