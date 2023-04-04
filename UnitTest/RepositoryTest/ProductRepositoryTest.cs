using Data.Exceptions;
using Data.Model;
using Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace UnitTest.RepositoryTest
{
    public class ProductRepositoryTest
    {
        private readonly ProductContext _context;
        private readonly UserContext userContext;
        private readonly ProductRepository _repository;

        public ProductRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<ProductContext>()
                    .UseInMemoryDatabase("ProductList")
                    .Options;
            var userOptions = new DbContextOptionsBuilder<UserContext>()
                    .UseInMemoryDatabase("UserLists")
                    .Options;
            _context = new ProductContext(options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
            userContext = new UserContext(userOptions);
            userContext.Database.EnsureDeleted();
            userContext.Database.EnsureCreated();
            _repository = new ProductRepository(_context, userContext);
        }

        private async Task<List<Product>> AddProducts()
        {
            var products = new List<Product>
        {
            new Product { name = "Apple", quantity = 100, sellerId = 1 },
            new Product { name = "Banana", quantity = 50, sellerId = 1 }
        };
            await _context.AddRangeAsync(products);
            await _context.SaveChangesAsync();
            return products;
        }

        private async Task<List<User>> AddUsers()
        {
            var users = new List<User>
        {
            new User { name = "Lisa", password = "lisa123", type = UserType.BUYER },
            new User { name = "Jack", password = "Jack123", type = UserType.SELLER }
        };
            await userContext.AddRangeAsync(users);
            await userContext.SaveChangesAsync();
            return users;
        }

        [Fact]
        public async Task GetProductListBySellerId_ShouldReturnProductList_WhenTypeIsNull()
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
            var product = new Product { name = "melon", quantity = 90, sellerId = 2 };
            // Act
            await _repository.AddProduct(product);
            // Assert
            Assert.NotNull(_context.Products);
            var savedProduct = await _context.Products.FirstOrDefaultAsync(u => u.name == product.name);
            Assert.NotNull(savedProduct);
            Assert.Equal(product.name, savedProduct.name);
            Assert.Equal(product.quantity, savedProduct.quantity);
            Assert.Equal(product.sellerId, savedProduct.sellerId);
        }

        [Fact]
        public async Task AddProduct_ShouldThrowDuplicateUserNameException_WhenProductNameExists()
        {
            // Arrange
            var products = await AddProducts();
            var product = new Product { name = "Banana", quantity = 50, sellerId = 1 };
            // Act & Assert
            await Assert.ThrowsAsync<DuplicateUserNameException>(async () => await _repository.AddProduct(product));
        }

        [Fact]
        public async Task AddProduct_ShouldThrowNotFoundException_WhenSellerIdNotExists()
        {
            // Arrange
            var products = await AddProducts();
            var product = new Product { name = "Watermelon", quantity = 70, sellerId = 2 };
            // Act & Assert
            await Assert.ThrowsAsync<DllNotFoundException>(async () => await _repository.AddProduct(product));
        }
    }
}