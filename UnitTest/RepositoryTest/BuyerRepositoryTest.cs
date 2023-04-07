using Data.Model;
using Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace UnitTest.RepositoryTest
{
    public class BuyerRepositoryTest
    {
        private readonly MyDbContext _context;
        private readonly BuyerRepository _repository;
        public BuyerRepositoryTest()
        {
            var productOptions = new DbContextOptionsBuilder<MyDbContext>()
                    .UseInMemoryDatabase("ProductList1")
                    .Options;
            _context = new MyDbContext(productOptions);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
            _repository = new BuyerRepository(_context);
        }

        private async Task<List<Product>> AddProducts()
        {
            var products = new List<Product>
            {
                new Product { Name = "Apple", Quantity = 100, SellerId = 1 },
                new Product { Name = "Apple", Quantity = 0, SellerId = 1 }
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

        private async Task<List<BuyerProduct>> AddBuyerProducts()
        {
            var products = await AddProducts();
            var users = await AddUsers();
            var buyerProducts = new List<BuyerProduct>
            {
                new BuyerProduct() {Name = "Apple",Quantity=100, SellerName = users[0].Name, Id = products[0].Id }
            };
            await _context.AddRangeAsync(buyerProducts);
            await _context.SaveChangesAsync();
            return buyerProducts;
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
            var products = await AddProducts();
            var users = await AddUsers();
            var expectedProduct = new BuyerProduct
            {
                Id = products[0].Id,
                Name = products[0].Name,
                Quantity = products[0].Quantity,
                SellerName = users[1].Name
            };
            // Act
            var result = await _repository.GetProductByProductId(products[0].Id);
            // Assert
            Assert.Equal(expectedProduct.ToString(), result.ToString());
        }

        [Fact]
        public async Task GetProductByProductId_ShouldReturnNotFoundException_WhenProductsIsNotfound()
        {
            // Arrange
            var products = await AddProducts();
            var users = await AddUsers();
            var expectedProduct = new BuyerProduct
            {
                Id = products[0].Id,
                Name = products[0].Name,
                Quantity = products[0].Quantity,
                SellerName = users[1].Name
            };
            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _repository.GetProductByProductId(3));
        }
    }
}