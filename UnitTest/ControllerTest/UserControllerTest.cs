using Service;
using Shopping.Controller;
using Moq;
using Data.Model;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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
            var user = new User { name = "test", password = "test123123", type = UserType.BUYER };
            Assert.NotNull(user);
            _userServiceMock
                .Setup(service => service.AddUser(user))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Created));
            // Act
            var result = await _userController.AddUser(user);
            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("Registered successfully.", createdResult.Value);
        }

        [Fact]
        public async Task CreateUser_ShouldReturnBadRequest_WhenUserNameExists()
        {
            // Arrange
            var user = new User { name = "test", password = "test123123", type = UserType.BUYER };
            _userServiceMock
                .Setup(service => service.AddUser(user))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    Content = new StringContent($"User name '{user.name}' already exists.")
                });
            // Act
            var result = await _userController.AddUser(user);
            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal("User name 'test' already exists.", conflictResult.Value);
        }

        [Fact]
        public async Task CreateUser_ShouldReturnBadRequest_WhenPasswordIsNull()
        {
            // Arrange
            var user = new User { name = "test", password = "", type = UserType.BUYER };
            _userServiceMock
                .Setup(service => service.AddUser(user))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Password cannot be empty.")
                });
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
            var user = new User { name = "test", password = "test123123", type = UserType.BUYER };
            _userServiceMock
                .Setup(service => service.AddUser(user))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("User name cannot be empty.")
                });
            // Act
            var result = await _userController.AddUser(user);
            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("User name cannot be empty.", badRequestObjectResult.Value);
        }
    }
}