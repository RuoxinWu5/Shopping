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
        public async Task CreateUser_ShouldReturnSuccessMessage_WhenUserIsValid()
        {
            // Arrange
            var user = new User { name = "test", password = "test123123", type = 1 };
            _userRepositoryMock
                .Setup(repository => repository.AddUser(user))
                .Returns(Task.CompletedTask);
            // Act
            var result = await _userService.AddUser(user);
            // Assert
            Assert.Equal("Registered successfully", result);
        }

        [Fact]
        public async Task CreateUser_ShouldReturnDuplicateUserNameExceptionMessage_WhenUserNameExists()
        {
            // Arrange
            var user = new User { name = "test", password = "test123123", type = 1 };
            _userRepositoryMock
                .Setup(repository => repository.AddUser(user))
                .Throws(new DuplicateUserNameException("User name 'test' already exists."));
            // Act
            var result = await _userService.AddUser(user);
            // Assert
            Assert.Equal("User name 'test' already exists.", result);
        }

        [Fact]
        public async Task CreateUser_ShouldReturnArgumentExceptionMessage_WhenPasswordIsEmpty()
        {
            // Arrange
            var user = new User { name = "testuser", password = "", type = 1 };
            _userRepositoryMock
                .Setup(repository => repository.AddUser(user))
                .Throws(new ArgumentException("Password cannot be empty."));
            // Act
            var result = await _userService.AddUser(user);
            // Assert
            Assert.Equal("Password cannot be empty.", result);
        }

        [Fact]
        public async Task CreateUser_ShouldReturnArgumentExceptionMessage_WhenUserNameIsEmpty()
        {
            // Arrange
            var user = new User { name = "", password = "testpassword", type = 1 };
            _userRepositoryMock
                .Setup(repository => repository.AddUser(user))
                .Throws(new ArgumentException("User name cannot be empty."));
            // Act
            var result = await _userService.AddUser(user);
            // Assert
            Assert.Equal("User name cannot be empty.", result);
        }
    }
}