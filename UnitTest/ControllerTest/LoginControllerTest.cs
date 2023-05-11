using Data.Exceptions;
using Data.Model;
using Data.RequestModel;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Service;
using Shopping.Controller;

namespace UnitTest.ControllerTest
{
    public class LoginControllerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
        private readonly LoginController _userLoginController;

        public LoginControllerTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _jwtTokenServiceMock = new Mock<IJwtTokenService>();
            _userLoginController = new LoginController(_userServiceMock.Object, _jwtTokenServiceMock.Object);
        }

        [Fact]
        public async Task Login_ReturnsTokenString_WhenUserFound()
        {
            // Arrange
            var user = new User { Id = 1, Name = "testuser", Password = "password", Type = UserType.BUYER };
            _userServiceMock
                .Setup(x => x.GetUserByUserNameAndPassword(user.Name, user.Password))
                .ReturnsAsync(user);

            var request = new LoginRequestModel { Name = user.Name, Password = user.Password };

            // Act
            var result = await _userLoginController.Login(request);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Login_ReturnsBadRequest_WhenUserNotFound()
        {
            // Arrange
            _userServiceMock
                .Setup(x => x.GetUserByUserNameAndPassword("nonexistentuser", "password"))
                .ThrowsAsync(new UserNotFoundException("The user doesn't exist."));

            var request = new LoginRequestModel { Name = "nonexistentuser", Password = "password" };

            // Act
            var result = await _userLoginController.Login(request);

            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("The user doesn't exist.", badRequestObjectResult.Value);
        }
    }
}