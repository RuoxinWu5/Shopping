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
                .Returns(Task.CompletedTask);
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
    }
}