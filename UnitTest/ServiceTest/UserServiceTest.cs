using Service;
using Data.Repository;
using Moq;
using Data.Model;
using Data.Exceptions;

namespace UnitTest.ServiceTest
{
    public class UserServiceTest
    {
        private readonly UserService _userService;
        private readonly Mock<IUserRepository> _userRepositoryMock;

        public UserServiceTest()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _userService = new UserService(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task AddUser_ShouldCallAddUserMethodOfRepository()
        {
            // Arrange
            var user = new User { Id = 1, Name = "testuser", Password = "testpassword", Type = UserType.BUYER };
            User? nullUser = null;
            _userRepositoryMock.Setup(x => x.GetUserByName(user.Name)).ReturnsAsync(nullUser);
            // Act
            await _userService.AddUser(user);
            // Assert
            _userRepositoryMock.Verify(repository => repository.AddUser(user), Times.Once);
        }

        [Fact]
        public async Task AddUser_ShouldCreateNewUserWithTypeBuyer_WhenTypeIsNull()
        {
            // Arrange
            var user = new User { Id = 1, Name = "testuser", Password = "testpassword" };
            User? nullUser = null;
            _userRepositoryMock.Setup(x => x.GetUserByName(user.Name)).ReturnsAsync(nullUser);
            // Act
            await _userService.AddUser(user);
            // Assert
            _userRepositoryMock.Verify(repository => repository.AddUser(user), Times.Once);
        }

        [Fact]
        public async Task AddUser_ShouldThrowDuplicateUserNameException_WhenUserNameExists()
        {
            // Arrange
            var user = new User { Name = "Lisa", Password = "test123123", Type = UserType.SELLER };
            _userRepositoryMock.Setup(x => x.GetUserByName(user.Name)).ReturnsAsync(user);
            // Act & Assert
            await Assert.ThrowsAsync<DuplicateUserNameException>(async () => await _userService.AddUser(user));
        }

        [Fact]
        public async Task GetUserById_ShouldReturnUserInfo_WhenUserIsfound()
        {
            // Arrange
            var id = 1;
            var user = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            _userRepositoryMock.Setup(repository => repository.GetUserById(It.IsAny<int>())).ReturnsAsync(user);
            // Act
            var result = await _userService.GetUserById(id);
            // Assert
            Assert.Equal(user, result);
            _userRepositoryMock.Verify(repository => repository.GetUserById(id), Times.Once);
        }

        [Fact]
        public async Task GetUserById_ShouldReturnUserNotFoundException_WhenUserIsNotfound()
        {
            // Arrange
            var id = 1;
            User? user = null;
            _userRepositoryMock.Setup(repository => repository.GetUserById(It.IsAny<int>())).ReturnsAsync(user);
            // Act & Assert
            await Assert.ThrowsAsync<UserNotFoundException>(async () => await _userService.GetUserById(id));
        }

        [Fact]
        public async Task GetSellerById_ShouldReturnSellerInfo_WhenSellerIsfound()
        {
            // Arrange
            var id = 1;
            var seller = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            _userRepositoryMock.Setup(repository => repository.GetUserById(It.IsAny<int>())).ReturnsAsync(seller);
            // Act
            var result = await _userService.GetSellerById(id);
            // Assert
            Assert.Equal(seller, result);
            _userRepositoryMock.Verify(repository => repository.GetUserById(id), Times.Once);
        }

        [Fact]
        public async Task GetSellerById_ShouldThrowSellerNotFoundException_WhenSellerIsNotfound()
        {
            // Arrange
            var id = 1;
            var buyer = new User { Name = "Jack", Password = "Jack123", Type = UserType.BUYER };
            _userRepositoryMock.Setup(repository => repository.GetUserById(It.IsAny<int>())).ReturnsAsync(buyer);
            // Act & Assert
            await Assert.ThrowsAsync<SellerNotFoundException>(async () => await _userService.GetSellerById(id));
        }

        [Fact]
        public async Task GetBuyerById_ShouldReturnBuyerInfo_WhenBuyerIsfound()
        {
            // Arrange
            var id = 1;
            var buyer = new User { Name = "Jack", Password = "Jack123", Type = UserType.BUYER };
            _userRepositoryMock.Setup(repository => repository.GetUserById(It.IsAny<int>())).ReturnsAsync(buyer);
            // Act
            var result = await _userService.GetBuyerById(id);
            // Assert
            Assert.Equal(buyer, result);
            _userRepositoryMock.Verify(repository => repository.GetUserById(id), Times.Once);
        }

        [Fact]
        public async Task GetBuyerById_ShouldThrowBuyerNotFoundException_WhenBuyerIsNotfound()
        {
            // Arrange
            var id = 1;
            var seller = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            _userRepositoryMock.Setup(repository => repository.GetUserById(It.IsAny<int>())).ReturnsAsync(seller);
            // Act & Assert
            await Assert.ThrowsAsync<BuyerNotFoundException>(async () => await _userService.GetBuyerById(id));
        }

        [Fact]
        public async Task ValidateIfSellerExit_ShouldCallGetUserByIdMethodOfRepository_WhenSellerIsfound()
        {
            // Arrange
            var id = 1;
            var user = new User { Id = 1, Name = "testuser", Password = "testpassword", Type = UserType.SELLER };
            _userRepositoryMock.Setup(repository => repository.GetUserById(It.IsAny<int>())).ReturnsAsync(user);
            // Act
            await _userService.ValidateIfSellerExist(id);
            // Assert
            _userRepositoryMock.Verify(repository => repository.GetUserById(id), Times.Once);
        }

        [Fact]
        public async Task ValidateIfSellerExit_ShouldThrowSellerNotFoundException_WhenSellerIsNotfound()
        {
            // Arrange
            var id = 1;
            User? user = null;
            _userRepositoryMock.Setup(repository => repository.GetUserById(It.IsAny<int>())).ReturnsAsync(user);
            // Act & Assert
            await Assert.ThrowsAsync<SellerNotFoundException>(async () => await _userService.ValidateIfSellerExist(id));
        }

        [Fact]
        public async Task ValidateIfBuyererExit_ShouldCallGetUserByIdMethodOfRepository_WhenBuyerIsfound()
        {
            // Arrange
            var id = 1;
            var user = new User { Id = 1, Name = "testuser", Password = "testpassword", Type = UserType.BUYER };
            _userRepositoryMock.Setup(repository => repository.GetUserById(It.IsAny<int>())).ReturnsAsync(user);
            // Act
            await _userService.ValidateIfBuyerExist(id);
            // Assert
            _userRepositoryMock.Verify(repository => repository.GetUserById(id), Times.Once);
        }

        [Fact]
        public async Task ValidateIfBuyererExit_ShouldThrowBuyerNotFoundException_WhenBuyerIsNotfound()
        {
            // Arrange
            var id = 1;
            User? user = null;
            _userRepositoryMock.Setup(repository => repository.GetUserById(It.IsAny<int>())).ReturnsAsync(user);
            // Act & Assert
            await Assert.ThrowsAsync<BuyerNotFoundException>(async () => await _userService.ValidateIfBuyerExist(id));
        }

        [Fact]
        public async Task GetUserByUserNameAndPassword_ShouldReturnUserInfo_WhenUserIsfound()
        {
            // Arrange
            var user = new User { Id = 1, Name = "testuser", Password = "testpassword" };
            _userRepositoryMock.Setup(repository => repository.GetUserByName(It.IsAny<string>())).ReturnsAsync(user);
            // Act
            var result = await _userService.GetUserByUserNameAndPassword("testuser", "testpassword");
            // Assert
            _userRepositoryMock.Verify(repository => repository.GetUserByName(result.Name), Times.Once);
            Assert.Equal(user, result);
        }

        [Fact]
        public async Task GetUserByUserNameAndPassword_ShouldThrowNotFoundException_WhenUserIsNotfound()
        {
            // Arrange
            var user = new User { Id = 1, Name = "testuser", Password = "testpassword" };
            User? nullUser = null;
            _userRepositoryMock.Setup(repository => repository.GetUserByName(It.IsAny<string>())).ReturnsAsync(nullUser);
            // Act & Assert
            await Assert.ThrowsAsync<UserNotFoundException>(async () => await _userService.GetUserByUserNameAndPassword("testuser", "testpassword"));
        }
        
        [Fact]
        public async Task GetUserByUserNameAndPassword_ShouldThrowNotFoundException_WhenPasswordIsNotEqual()
        {
            // Arrange
            var user = new User { Id = 1, Name = "testuser", Password = "testpassword" };
            _userRepositoryMock.Setup(repository => repository.GetUserByName(It.IsAny<string>())).ReturnsAsync(user);
            // Act & Assert
            await Assert.ThrowsAsync<UserNotFoundException>(async () => await _userService.GetUserByUserNameAndPassword("testuser", "password"));
        }
    }
}