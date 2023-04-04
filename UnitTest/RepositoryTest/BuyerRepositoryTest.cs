using Data.Model;
using Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace UnitTest.RepositoryTest
{
    public class BuyerRepositoryTest
    {
        private readonly BuyerProductContext _context;
        private readonly BuyerRepository _repository;

        public BuyerRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<BuyerProductContext>()
                    .UseInMemoryDatabase("BuyerProductList")
                    .Options;
            _context = new BuyerProductContext(options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
            _repository = new BuyerRepository(_context);
        }
        private async Task<List<BuyerProduct>> AddBuyerProducts()
        {
            var buyerProducts = new List<BuyerProduct>
        {
            new BuyerProduct() {name = "Apple",quantity=100, sellerName = "Lisa" },
            new BuyerProduct() {name = "Banana",quantity=50, sellerName = "Lisa" }
        };
            await _context.AddRangeAsync(buyerProducts);
            await _context.SaveChangesAsync();
            return buyerProducts;
        }

        [Fact]
        public async Task GetProductList_ShouldReturnProductList_WhenProductsIsfound()
        {
            // Arrange
            var buyerProducts = await AddBuyerProducts();
            // Act
            var result = await _repository.AllProduct();
            // Assert
            Assert.Equal(buyerProducts, result);
        }
    }
}