using Service;
using Data.Repository;
using Moq;
using Data.Model;
using Data.Exceptions;
using System.Net;

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
            var user = new User { name = "test", password = "test123123", type = UserType.BUYER };
            _userRepositoryMock
                .Setup(repository => repository.AddUser(user))
                .Returns(Task.CompletedTask);
            // Act
            var result = await _userService.AddUser(user);
            // Assert
            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
        }

        [Fact]
        public async Task CreateUser_ShouldReturnDuplicateUserNameExceptionMessage_WhenUserNameExists()
        {
            // Arrange
            var user = new User { name = "test", password = "test123123", type = UserType.BUYER };
            _userRepositoryMock
                .Setup(repository => repository.AddUser(user))
                .Throws(new DuplicateUserNameException("User name 'test' already exists."));
            // Act
            var result = await _userService.AddUser(user);
            // Assert
            Assert.Equal(HttpStatusCode.Conflict, result.StatusCode);
        }
    }
}