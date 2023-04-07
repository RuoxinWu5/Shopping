using Data.Exceptions;
using Data.Model;
using Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace UnitTest.RepositoryTest
{
    public class ProductRepositoryTest
    {
        private readonly MyDbContext _context;
        private readonly ProductRepository _repository;

        public ProductRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<MyDbContext>()
                .UseInMemoryDatabase("ProductList")
                .Options;
            _context = new MyDbContext(options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
            _repository = new ProductRepository(_context);
        }

        private async Task<List<Product>> AddProducts()
        {
            var products = new List<Product>
        {
            new Product { Name = "Apple", Quantity = 100, SellerId = 1 },
            new Product { Name = "Banana", Quantity = 10, SellerId = 1 }
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
        public async Task GetProductListBySellerId_ShouldReturnProductList_WhenProductsIsfound()
        {
            // Arrange
            var products = await AddProducts();
            var sellerId = 1;
            // Act
            var result = await _repository.GetProductListBySellerId(sellerId);
            // Assert
            Assert.Equal(products, result);
        }

        [Fact]
        public async Task AddProduct_ShouldCreateNewProduct_WhenProductIsValid()
        {
            // Arrange
            var products = await AddProducts();
            var users = await AddUsers();
            var product = new Product { Name = "melon", Quantity = 90, SellerId = 2 };
            // Act
            await _repository.AddProduct(product);
            // Assert
            Assert.NotNull(_context.Products);
            var savedProduct = await _context.Products.FirstOrDefaultAsync(u => u.Name == product.Name);
            Assert.NotNull(savedProduct);
            Assert.Equal(product.Name, savedProduct.Name);
            Assert.Equal(product.Quantity, savedProduct.Quantity);
            Assert.Equal(product.SellerId, savedProduct.SellerId);
        }

        [Fact]
        public async Task AddProduct_ShouldThrowDuplicateUserNameException_WhenProductNameExists()
        {
            // Arrange
            var products = await AddProducts();
            var product = new Product { Name = "Banana", Quantity = 90, SellerId = 2 };
            // Act & Assert
            await Assert.ThrowsAsync<DuplicateUserNameException>(async () => await _repository.AddProduct(product));
        }

        [Fact]
        public async Task AddProduct_ShouldThrowNotFoundException_WhenSellerIdNotExists()
        {
            // Arrange
            var products = await AddProducts();
            var product = new Product { Name = "melon", Quantity = 90, SellerId = 1 };
            // Act & Assert
            await Assert.ThrowsAsync<DllNotFoundException>(async () => await _repository.AddProduct(product));
        }
    }
}