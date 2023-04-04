using Data.Model;
using Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace UnitTest.RepositoryTest
{
    public class BuyerRepositoryTest
    {
        private readonly BuyerProductContext _context;
        private readonly ProductContext productContext;
        private readonly UserContext userContext;
        private readonly BuyerRepository _repository;
        public BuyerRepositoryTest()
        {
            var productOptions = new DbContextOptionsBuilder<ProductContext>()
                    .UseInMemoryDatabase("ProductList1")
                    .Options;
            var userOptions = new DbContextOptionsBuilder<UserContext>()
                    .UseInMemoryDatabase("UserList1")
                    .Options;
            var options = new DbContextOptionsBuilder<BuyerProductContext>()
                    .UseInMemoryDatabase("BuyerProductList1")
                    .Options;
            _context = new BuyerProductContext(options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
            productContext = new ProductContext(productOptions);
            productContext.Database.EnsureDeleted();
            productContext.Database.EnsureCreated();
            userContext = new UserContext(userOptions);
            userContext.Database.EnsureDeleted();
            userContext.Database.EnsureCreated();
            _repository = new BuyerRepository(_context, productContext, userContext);
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

        private async Task<List<Product>> AddProducts()
        {
            var products = new List<Product>
        {
            new Product { name = "Apple", quantity = 100, sellerId = 1 },
            new Product { name = "Banana", quantity = 50, sellerId = 1 }
        };
            await productContext.AddRangeAsync(products);
            await productContext.SaveChangesAsync();
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
        public async Task GetProductList_ShouldReturnProductList_WhenProductsIsfound()
        {
            // Arrange
            var products = await AddProducts();
            List<string> buyerProducts = new List<string>();
            foreach (Product product in products)
            {
                buyerProducts.Add(product.name);
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