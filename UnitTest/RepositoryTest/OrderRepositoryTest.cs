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
            var options = new DbContextOptionsBuilder<ShoppingDbContext>()
                    .UseInMemoryDatabase("OrderList")
                    .Options;
            _context = new ShoppingDbContext(options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
            _repository = new OrderRepository(_context);
        }

        private async Task<List<Order>> AddOrder()
        {
            var users = _context.Users.ToList();
            var products = _context.Products.ToList();
            var order = new List<Order>
            {
                new Order { Id = 1, Quantity = 10, Type = OrderState.TO_BE_PAID, Product = products[0], User = users[0] }
            };
            await _context.AddRangeAsync(order);
            await _context.SaveChangesAsync();
            return order;
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
        public async Task AddOrder_ShouldCreateNewOrder_WhenOrderIsValid()
        {
            // Arrange
            var users = await AddUsers();
            var products = await AddProducts();
            var order = new Order { Id = 1, Quantity = 10, Type = OrderState.TO_BE_PAID, Product = products[0], User = users[0] };
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
            var users = await AddUsers();
            var products = await AddProducts();
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
            var users = await AddUsers();
            var products = await AddProducts();
            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _repository.GetOrderById(1));
        }

        [Fact]
        public async Task UpdateOrderState_ShouldChangeOrderStateToPaid_WhenOrderIsValid()
        {
            // Arrange
            var users = await AddUsers();
            var products = await AddProducts();
            var orders = await AddOrder();
            var order = orders[0];
            // Act
            await _repository.UpdateOrderState(order.Id, OrderState.PAID);
            // Assert
            var result = await _context.Orders.FirstOrDefaultAsync(o => o.Id == order.Id);
            Assert.NotNull(result);
            Assert.Equal(OrderState.PAID, result.Type);
        }

        [Fact]
        public async Task GetOrderListBySellerId_ShouldReturnOrderList_WhenOrdersIsfound()
        {
            // Arrange
            var users = await AddUsers();
            var products = await AddProducts();
            var orders = await AddOrder();
            var sellerId = 2;
            // Act
            var result = await _repository.GetOrderListBySellerId(sellerId);
            // Assert
            Assert.Equal(orders, result);
        }
    }
}