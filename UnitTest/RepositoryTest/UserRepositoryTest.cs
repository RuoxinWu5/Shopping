using Data.Model;
using Data.Repository;
using Microsoft.EntityFrameworkCore;
using Data.Exceptions;

namespace UnitTest.RepositoryTest;

public class UserRepositoryTest
{
    private const string my_connection_string = "server=localhost;port=3306;database=shopping;user=sqluser;password=password";
    private readonly UserContext _context;
    private readonly UserRepository _repository;

    public UserRepositoryTest()
    {
        var options = new DbContextOptionsBuilder<UserContext>()
            .UseMySql(my_connection_string, ServerVersion.AutoDetect(my_connection_string))
            .Options;
        _context = new UserContext(options);
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
        _repository = new UserRepository(_context);
    }

    [Fact]
    public async Task CreateUser_ShouldCreateNewUser_WhenInputIsValid()
    {
        // Arrange
        var user = new User { name = "test", password = "test123123", type = 2 };
        // Act
        await _repository.AddUser(user);
        // Assert
        var savedUser = await _context.Users.FirstOrDefaultAsync(u => u.name == user.name);
        Assert.NotNull(savedUser);
        Assert.Equal(user.password, savedUser.password);
        Assert.Equal(user.type, savedUser.type);
    }

    [Fact]
    public async Task CreateUser_ShouldCreateNewUserWithTypeBuyer_WhenTypeIsNull()
    {
        // Arrange
        var user = new User { name = "noType", password = "noType123123" };
        var buyer_type = 1;
        // Act
        await _repository.AddUser(user);
        // Assert
        var savedUser = await _context.Users.FirstOrDefaultAsync(u => u.name == user.name);
        Assert.NotNull(savedUser);
        Assert.Equal(user.password, savedUser.password);
        Assert.Equal(buyer_type, savedUser.type);
    }

    [Fact]
    public async Task CreateUser_ShouldThrowDuplicateUserNameException_WhenUserNameExists()
    {
        // Arrange
        var user1 = new User { name = "test", password = "test123123", type = 1 };
        var user2 = new User { name = "test", password = "test456456", type = 2 };
        await _repository.AddUser(user1);
        // Act & Assert
        await Assert.ThrowsAsync<DuplicateUserNameException>(async () => await _repository.AddUser(user2));
    }

    [Fact]
    public async Task CreateUser_ShouldThrowArgumentException_WhenPasswordIsEmpty()
    {
        // Arrange
        var user = new User { name = "testuser", password = "", type = 1 };
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await _repository.AddUser(user));
    }

    [Fact]
    public async Task CreateUser_ShouldThrowArgumentException_WhenUserNameIsEmpty()
    {
        // Arrange
        var user = new User { name = "", password = "testpassword", type = 1 };
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await _repository.AddUser(user));
    }

}