using Data.Model;
using Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace UnitTest.RepositoryTest
{
    public class ProductRepositoryTest
    {
        private readonly ProductContext _context;
        private readonly ProductRepository _repository;

        public ProductRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<ProductContext>()
                    .UseInMemoryDatabase("ProductList")
                    .Options;
            _context = new ProductContext(options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
            _repository = new ProductRepository(_context);
        }

        private async Task<List<Product>> AddProducts()
        {
            var products = new List<Product>
        {
            new Product { name="Apple",quantity=100,sellerId=1 },
            new Product {name="Banana",quantity=50,sellerId=1 }
        };
            await _context.AddRangeAsync(products);
            await _context.SaveChangesAsync();
            return products;
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
    }
}