using Data.Model;
using Data.RequestModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Service;
using Shopping.Controller;

namespace UnitTest.ControllerTest
{
    public class LoginControllerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly LoginController _userLoginController;

        public LoginControllerTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _configurationMock = new Mock<IConfiguration>();
            _userLoginController = new LoginController(_userServiceMock.Object, _configurationMock.Object);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsTokenString()
        {
            // Arrange
            var user = new User { Id = 1, Name = "testuser", Password = "password", Type = UserType.BUYER };
            _userServiceMock.Setup(x => x.GetUserByUserNameAndPassword(user.Name, user.Password)).ReturnsAsync(user);
            _configurationMock.Setup(x => x["Jwt:Issuer"]).Returns("testissuer");
            _configurationMock.Setup(x => x["Jwt:Audience"]).Returns("testaudience");
            _configurationMock.Setup(x => x["Jwt:SecretKey"]).Returns("this-is-my-test-secret-key");
            var request = new LoginRequestModel { Name = user.Name, Password = user.Password };
            // Act
            var result = await _userLoginController.Login(request);
            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Login_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            _userServiceMock.Setup(x => x.GetUserByUserNameAndPassword("nonexistentuser", "password")).ThrowsAsync(new KeyNotFoundException("User not found"));
            var request = new LoginRequestModel { Name = "nonexistentuser", Password = "password" };

            // Act
            var result = await _userLoginController.Login(request) as NotFoundObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
            Assert.Equal("User not found", result.Value);
        }
    }
}