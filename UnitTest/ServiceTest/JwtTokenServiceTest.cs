using System.IdentityModel.Tokens.Jwt;
using Data.Model;
using Microsoft.Extensions.Configuration;
using Moq;
using Service;

namespace UnitTest.ServiceTest
{
    public class JwtTokenServiceTest
    {
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly JwtTokenService _jwtTokenService;

        public JwtTokenServiceTest()
        {
            _configurationMock = new Mock<IConfiguration>();
            _jwtTokenService = new JwtTokenService(_configurationMock.Object);
        }

        [Fact]
        public void GenerateJwtToken_ShouldReturnToken()
        {
            // Arrange
            _configurationMock.Setup(x => x["Jwt:Issuer"]).Returns("testIssuer");
            _configurationMock.Setup(x => x["Jwt:Audience"]).Returns("testAudience");
            _configurationMock.Setup(x => x["Jwt:SecretKey"]).Returns("this-is-my-test-secret-key");
            var user = new User { Id = 1, Name = "Test User", Type = UserType.BUYER };
            // Act
            var token = _jwtTokenService.GenerateJwtToken(user);
            // Assert
            Assert.NotNull(token);
            var tokenHandler = new JwtSecurityTokenHandler();
            var decodedToken = tokenHandler.ReadJwtToken(token);
            Assert.Equal("testIssuer", decodedToken.Issuer);
            var claims = decodedToken.Claims.ToList();
            Assert.Equal("1", claims[0].Value);
            Assert.Equal("Test User", claims[1].Value);
            Assert.Equal("Buyer", claims[2].Value);
        }
    }
}