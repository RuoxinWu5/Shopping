using Service;
using Controller;
using Moq;
using Data.Model;
using Microsoft.AspNetCore.Mvc;

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
            var user = new User { name = "test", password = "test123123", type = 1 };
            _userServiceMock
                .Setup(service => service.AddUser(user))
                .ReturnsAsync("Registered successfully");
            // Act
            var result = await _userController.AddUser(user);
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Registered successfully", okResult.Value);
        }

        [Fact]
        public async Task CreateUser_ShouldReturnBadRequest_WhenUserNameExists()
        {
            // Arrange
            var user = new User { name = "test", password = "test123123", type = 1 };
            _userServiceMock
                .Setup(service => service.AddUser(user))
                .ReturnsAsync("User name 'test' already exists.");
            // Act
            var result = await _userController.AddUser(user);
            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("User name 'test' already exists.", badRequestObjectResult.Value);
        }

        [Fact]
        public async Task CreateUser_ShouldReturnBadRequest_WhenPasswordIsNull()
        {
            // Arrange
            var user = new User { name = "test", password = "", type = 1 };
            _userServiceMock
                .Setup(service => service.AddUser(user))
                .ReturnsAsync("Password cannot be empty.");
            // Act
            var result = await _userController.AddUser(user);
            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Password cannot be empty.", badRequestObjectResult.Value);
        }

        [Fact]
        public async Task CreateUser_ShouldReturnBadRequest_WhenUserNameIsNull()
        {
            // Arrange
            var user = new User { name = "test", password = "test123123", type = 1 };
            _userServiceMock
                .Setup(service => service.AddUser(user))
                .ReturnsAsync("User name cannot be empty.");
            // Act
            var result = await _userController.AddUser(user);
            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("User name cannot be empty.", badRequestObjectResult.Value);
        }
    }
}