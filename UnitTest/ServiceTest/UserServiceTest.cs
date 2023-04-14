using Service;
using Data.Repository;
using Moq;
using Data.Model;
using Microsoft.AspNetCore.Mvc;

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
            var user = new User { Id = 1, Name = "testuser", Password = "testpassword" };
            // Act
            await _userService.AddUser(user);
            // Assert
            _userRepositoryMock.Verify(repository => repository.AddUser(user), Times.Once);
        }

        [Fact]
        public async Task GetUserById_ShouldCallGetUserByIdMethodOfRepository()
        {
            // Arrange
            var id = 1;
            // Act
            await _userService.GetUserById(id);
            // Assert
            _userRepositoryMock.Verify(repository => repository.GetUserById(id), Times.Once);
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
            Assert.Equal(UserType.SELLER, result.Type);
            _userRepositoryMock.Verify(repository => repository.GetUserById(id), Times.Once);
        }

        [Fact]
        public async Task GetSellerById_ShouldThrowNotFoundException_WhenSellerIsNotfound()
        {
            // Arrange
            var id = 1;
            var seller = new User { Name = "Jack", Password = "Jack123", Type = UserType.BUYER };
            _userRepositoryMock.Setup(repository => repository.GetUserById(It.IsAny<int>())).ReturnsAsync(seller);
            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _userService.GetSellerById(id));
        }

        [Fact]
        public async Task GetBuyerById_ShouldReturnBuyerInfo_WhenBuyerIsfound()
        {
            // Arrange
            var id = 1;
            var seller = new User { Name = "Jack", Password = "Jack123", Type = UserType.BUYER };
            _userRepositoryMock.Setup(repository => repository.GetUserById(It.IsAny<int>())).ReturnsAsync(seller);
            // Act
            var result = await _userService.GetBuyerById(id);
            // Assert
            Assert.Equal(UserType.BUYER, result.Type);
            _userRepositoryMock.Verify(repository => repository.GetUserById(id), Times.Once);
        }

        [Fact]
        public async Task GetBuyerById_ShouldThrowNotFoundException_WhenBuyerIsNotfound()
        {
            // Arrange
            var id = 1;
            var seller = new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER };
            _userRepositoryMock.Setup(repository => repository.GetUserById(It.IsAny<int>())).ReturnsAsync(seller);
            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _userService.GetBuyerById(id));
        }
    }
}