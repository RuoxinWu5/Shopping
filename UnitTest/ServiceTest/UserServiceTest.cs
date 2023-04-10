using Service;
using Data.Repository;
using Moq;
using Data.Model;

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

    }
}