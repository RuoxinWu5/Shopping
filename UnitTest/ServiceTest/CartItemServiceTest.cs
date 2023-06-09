using Data.Exceptions;
using Data.Model;
using Data.Repository;
using Data.RequestModel;
using Moq;
using Service;

namespace UnitTest.ServiceTest
{
    public class CartItemServiceTest
    {
        private readonly CartItemService _cartService;
        private readonly Mock<ICartItemRepository> _cartRepositoryMock;
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IProductService> _productServiceMock;
        private readonly Mock<IUserService> _userServiceMock;

        public CartItemServiceTest()
        {
            _cartRepositoryMock = new Mock<ICartItemRepository>();
            _productRepositoryMock = new Mock<IProductRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _productServiceMock = new Mock<IProductService>();
            _userServiceMock = new Mock<IUserService>();
            _cartService = new CartItemService(_cartRepositoryMock.Object, _productRepositoryMock.Object, _userRepositoryMock.Object, _productServiceMock.Object, _userServiceMock.Object);
        }

        private AddProductToCartRequestModel CreateAddProductToCartRequestModelItems()
        {
            var addProductToCartRequestModel = new AddProductToCartRequestModel { ProductId = 1, Quantity = 10, BuyerId = 2 };
            return addProductToCartRequestModel;
        }

        [Fact]
        public async Task AddCartItem_ShouldCallAddCartItemMethodOfRepository_WhenQuantityIsEnoughAndCartItemIsNotExist()
        {
            // Arrange
            CartItem? nullCartItem = null;
            _cartRepositoryMock
                .Setup(repository => repository.GetCartItemByProductIdAndBuyerId(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(nullCartItem);

            var seller = new User { Id = 1, Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Id = 1, Name = "Apple", Quantity = 100, User = seller };
            _productServiceMock
                .Setup(service => service.GetProductById(It.IsAny<int>()))
                .ReturnsAsync(product);

            var buyer = new User { Id = 2, Name = "Lisa", Password = "Lisa123", Type = UserType.BUYER };
            _userServiceMock
                .Setup(service => service.GetBuyerById(It.IsAny<int>()))
                .ReturnsAsync(buyer);

            var addProductToCartRequestModelItem = CreateAddProductToCartRequestModelItems();

            // Act
            var result = await _cartService.AddCartItem(addProductToCartRequestModelItem);

            // Assert
            var cartItem = new CartItem { Product = product, Quantity = 10, User = buyer };
            Assert.Equivalent(cartItem, result);
            _cartRepositoryMock.Verify(repository => repository.AddCartItem(It.IsAny<CartItem>()), Times.Once);
        }

        [Fact]
        public async Task AddCartItem_ShouldCallUpdateCartItemMethodOfRepository_WhenQuantityIsEnoughAndCartItemIsExist()
        {
            // Arrange
            var seller = new User { Id = 1, Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Id = 1, Name = "Apple", Quantity = 100, User = seller };
            var buyer = new User { Id = 2, Name = "Lisa", Password = "Lisa123", Type = UserType.BUYER };
            var cartItem = new CartItem { Product = product, Quantity = 10, User = buyer };
            _cartRepositoryMock
                .Setup(repository => repository.GetCartItemByProductIdAndBuyerId(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(cartItem);

            var addProductToCartRequestModelItem = CreateAddProductToCartRequestModelItems();

            // Act
            var result = await _cartService.AddCartItem(addProductToCartRequestModelItem);

            // Assert
            var cartItemResult = new CartItem { Product = product, Quantity = 20, User = buyer };
            Assert.Equivalent(cartItemResult, result);
            _cartRepositoryMock.Verify(repository => repository.UpdateCartItem(It.IsAny<CartItem>()), Times.Once);
        }

        [Fact]
        public async Task AddCartItem_ShouldThrowArgumentException_WhenQuantityIsNotEnough()
        {
            // Arrange
            CartItem? nullCartItem = null;
            _cartRepositoryMock
                .Setup(repository => repository.GetCartItemByProductIdAndBuyerId(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(nullCartItem);

            var seller = new User { Id = 1, Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Id = 1, Name = "Apple", Quantity = 10, User = seller };
            _productServiceMock.Setup(service => service.GetProductById(It.IsAny<int>())).ReturnsAsync(product);

            var buyer = new User { Id = 2, Name = "Lisa", Password = "Lisa123", Type = UserType.BUYER };
            _userServiceMock.Setup(service => service.GetBuyerById(It.IsAny<int>())).ReturnsAsync(buyer);

            var addProductToCartRequestModel = new AddProductToCartRequestModel { ProductId = 1, Quantity = 100, BuyerId = 1 };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _cartService.AddCartItem(addProductToCartRequestModel));
        }

        [Fact]
        public async Task GetCartItemById_ShouldReturnCartItem_WhenCartItemExist()
        {
            // Arrange
            var seller = new User { Id = 1, Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Id = 1, Name = "Apple", Quantity = 100, User = seller };
            var buyer = new User { Id = 2, Name = "Lisa", Password = "Lisa123", Type = UserType.BUYER };
            var cartItem = new CartItem { Product = product, Quantity = 10, User = buyer };
            _cartRepositoryMock.Setup(repository => repository.GetCartItemById(It.IsAny<int>())).ReturnsAsync(cartItem);

            var id = 1;

            // Act
            var result = await _cartService.GetCartItemById(id);

            // Assert
            Assert.Equal(cartItem, result);
        }

        [Fact]
        public async Task GetCartItemById_ShouldThrowCartItemNotFoundException_WhenCartItemNotExist()
        {
            // Arrange
            CartItem? nullCartItem = null;
            _cartRepositoryMock.Setup(repository => repository.GetCartItemById(It.IsAny<int>())).ReturnsAsync(nullCartItem);

            var id = 1;

            // Act & Assert
            await Assert.ThrowsAsync<CartItemNotFoundException>(() => _cartService.GetCartItemById(id));
        }

        [Fact]
        public async Task GetCartItemListByBuyerId_ShouldReturnCartItemList_WhenCartItemListIsFound()
        {
            // Arrange
            var seller = new User { Id = 1, Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Id = 1, Name = "Apple", Quantity = 100, User = seller };
            var buyer = new User { Id = 2, Name = "Lisa", Password = "Lisa123", Type = UserType.BUYER };
            var cartItemList = new List<CartItem>
            {
                new CartItem { Product = product, Quantity = 10, User = buyer }
            };
            _cartRepositoryMock
                .Setup(repository => repository.GetCartItemListByBuyerId(It.IsAny<int>()))
                .ReturnsAsync(cartItemList);

            int id = 1;

            // Act
            var result = await _cartService.GetCartItemListByBuyerId(id);

            // Assert
            Assert.Equal(cartItemList, result);
        }

        [Fact]
        public async Task GetCartItemListByBuyerId_ShouldThrowBuyerNotFoundException_WhenBuyerNotExist()
        {
            // Arrange
            _userServiceMock
                .Setup(service => service.ValidateIfBuyerExist(It.IsAny<int>()))
                .ThrowsAsync(new BuyerNotFoundException("The buyer doesn't exist."));

            int id = 1;

            // Act & Assert
            await Assert.ThrowsAsync<BuyerNotFoundException>(() => _cartService.GetCartItemListByBuyerId(id));
        }

        [Fact]
        public void IsCartItemOwnedByUser_ShouldThrowCartItemOwnershipException_WhenThisCartItemNotBelongsToThisUser()
        {
            // Arrange
            var seller = new User { Id = 1, Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Id = 1, Name = "Apple", Quantity = 100, User = seller };
            var buyer = new User { Id = 2, Name = "Lisa", Password = "Lisa123", Type = UserType.BUYER };
            var cartItem = new CartItem { Product = product, Quantity = 10, User = buyer };
            var userId = 1;

            // Act & Assert
            Assert.Throws<CartItemOwnershipException>(() => _cartService.IsCartItemOwnedByUser(cartItem, userId));
        }

        [Fact]
        public void IsCartItemOwnedByUser_ShouldNotThrowException_WhenThisCartItemBelongsToThisUser()
        {
            // Arrange
            var seller = new User { Id = 1, Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Id = 1, Name = "Apple", Quantity = 100, User = seller };
            var buyer = new User { Id = 2, Name = "Lisa", Password = "Lisa123", Type = UserType.BUYER };
            var cartItem = new CartItem { Product = product, Quantity = 10, User = buyer };
            var userId = 2;

            // Act
            var exception = Record.Exception(() => _cartService.IsCartItemOwnedByUser(cartItem, userId));

            //Assert
            Assert.Null(exception);
        }

        [Fact]
        public async Task DeleteCartItemById_ShouldCallDeleteCartItemMethodOfRepository_WhenCartItemIsFound()
        {
            // Arrange
            var seller = new User { Id = 1, Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            var product = new Product { Id = 1, Name = "Apple", Quantity = 100, User = seller };
            var buyer = new User { Id = 2, Name = "Lisa", Password = "Lisa123", Type = UserType.BUYER };
            var cartItem = new CartItem { Product = product, Quantity = 10, User = buyer };
            _cartRepositoryMock.Setup(repository => repository.GetCartItemById(It.IsAny<int>())).ReturnsAsync(cartItem);

            var id = 1;

            // Act
            await _cartService.DeleteCartItemById(id);

            // Assert
            _cartRepositoryMock.Verify(repository => repository.DeleteCartItem(cartItem), Times.Once);
        }
    }
}