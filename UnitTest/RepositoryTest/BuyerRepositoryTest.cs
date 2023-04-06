using Data.Model;
using Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace UnitTest.RepositoryTest
{
    public class BuyerRepositoryTest
    {
        private readonly BuyerProductContext _buyerProductContext;
        private readonly MyDbContext _context;
        private readonly BuyerRepository _repository;
        public BuyerRepositoryTest()
        {
            var productOptions = new DbContextOptionsBuilder<MyDbContext>()
                    .UseInMemoryDatabase("ProductList1")
                    .Options;
            var options = new DbContextOptionsBuilder<BuyerProductContext>()
                    .UseInMemoryDatabase("BuyerProductList1")
                    .Options;
            _buyerProductContext = new BuyerProductContext(options);
            _buyerProductContext.Database.EnsureDeleted();
            _buyerProductContext.Database.EnsureCreated();
            _context = new MyDbContext(productOptions);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
            _repository = new BuyerRepository(_context);
        }

        private async Task<List<BuyerProduct>> AddBuyerProducts()
        {
            var buyerProducts = new List<BuyerProduct>
        {
            new BuyerProduct() {name = "Apple",quantity=100, sellerName = "Lisa" }
        };
            await _buyerProductContext.AddRangeAsync(buyerProducts);
            await _buyerProductContext.SaveChangesAsync();
            return buyerProducts;
        }

        private async Task<List<Product>> AddProducts()
        {
            var products = new List<Product>
        {
            new Product ("Apple", 100, 1 ),
            new Product ("Banana", 0, 1 )
        };
            await _context.AddRangeAsync(products);
            await _context.SaveChangesAsync();
            return products;
        }

        private async Task<List<User>> AddUsers()
        {
            var users = new List<User>
        {
            new User ( "Lisa", "lisa123", UserType.BUYER ),
            new User  ( "Jack", "Jack123", UserType.SELLER )
        };
            await _context.AddRangeAsync(users);
            await _context.SaveChangesAsync();
            return users;
        }

        [Fact]
        public async Task GetProductList_ShouldReturnProductList_WhenProductsIsfound()
        {
            // Arrange
            var products = await AddProducts();
            List<string> buyerProducts = new List<string>();
            foreach (Product product in products)
            {
                buyerProducts.Add(product.Name);
            }
            // Act
            var result = await _repository.AllProduct();
            // Assert
            Assert.Equal(buyerProducts.ToString(), result.ToString());
        }

        [Fact]
        public async Task GetProductByProductId_ShouldReturnProduct_WhenProductsIsfound()
        {
            // Arrange
            var buyerProducts = await AddBuyerProducts();
            await AddProducts();
            await AddUsers();
            var detail = buyerProducts.First();
            // Act
            var result = await _repository.GetProductByProductId(1);
            // Assert
            Assert.Equal(detail.ToString(), result.ToString());
        }

        [Fact]
        public async Task GetProductByProductId_ShouldReturnNotFoundException_WhenProductsIsfound()
        {
            // Arrange
            var buyerProducts = await AddBuyerProducts();
            await AddProducts();
            await AddUsers();
            // Act $ Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _repository.GetProductByProductId(3));
        }
    }
}