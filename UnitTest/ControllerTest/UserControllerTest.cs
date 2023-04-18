using Service;
using Shopping.Controller;
using Moq;
using Data.Model;
using Microsoft.AspNetCore.Mvc;
using Data.Exceptions;

namespace UnitTest.ControllerTest
{
    public class UserControllerTest
    {
        private readonly UserController _userController;
        private readonly Mock<IUserService> _userServiceMock;

        public UserControllerTest()
        {
            _userServiceMock = new Mock<IUserService>();
            _userController = new UserController(_userServiceMock.Object);
        }

        [Fact]
        public async Task CreateUser_ShouldReturnOk_WhenUserIsValid()
        {
            // Arrange
            var user = new User { Name = "test", Password = "test123123", Type = UserType.BUYER };
            Assert.NotNull(user);
            _userServiceMock
                .Setup(service => service.AddUser(user))
                .ReturnsAsync(user);
            // Act
            var result = await _userController.AddUser(user);
            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(user, createdResult.Value);
        }

        [Fact]
        public async Task CreateUser_ShouldReturnBadRequest_WhenUserNameExists()
        {
            // Arrange
            var user = new User { Name = "test", Password = "test123123", Type = UserType.BUYER };
            _userServiceMock
                .Setup(service => service.AddUser(user))
                .Throws(new DuplicateUserNameException("User name 'test' already exists."));
            // Act
            var result = await _userController.AddUser(user);
            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal("User name 'test' already exists.", conflictResult.Value);
        }

        [Fact]
        public async Task GetUserById_ShouldReturnOk_WhenUserIsExist()
        {
            // Arrange
            var user = new User { Name = "test", Password = "test123123", Type = UserType.BUYER };
            _userServiceMock
                .Setup(service => service.GetUserById(It.IsAny<int>()))
                .ReturnsAsync(user);
            // Act
            var result = await _userController.GetUserById(1);
            // Assert
            var createdResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(user, createdResult.Value);
        }

        [Fact]
        public async Task GetUserById_ShouldReturnBadRequest_WhenUserIsNotExists()
        {
            // Arrange
            _userServiceMock
                .Setup(service => service.GetUserById(It.IsAny<int>()))
                .Throws(new KeyNotFoundException("The user doesn't exist."));
            // Act
            var result = await _userController.GetUserById(1);
            // Assert
            var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("The user doesn't exist.", notFoundObjectResult.Value);
        }
    }
}