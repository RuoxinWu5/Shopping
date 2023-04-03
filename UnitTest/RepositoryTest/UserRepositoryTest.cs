using Data.Model;
using Data.Repository;
using Microsoft.EntityFrameworkCore;
using Data.Exceptions;

namespace UnitTest.RepositoryTest;

public class UserRepositoryTest
{
    private readonly UserContext _context;
    private readonly UserRepository _repository;

    public UserRepositoryTest()
    {
        var options = new DbContextOptionsBuilder<UserContext>()
                .UseInMemoryDatabase("UserList")
                .Options;
        _context = new UserContext(options);
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
        _repository = new UserRepository(_context);
    }

    private async Task<List<User>> AddUsers()
    {
        var users = new List<User>
        {
            new User { name = "Lisa", password = "lisa123", type = UserType.BUYER },
            new User { name = "Jack", password = "Jack123", type = UserType.SELLER }
        };
        await _context.AddRangeAsync(users);
        await _context.SaveChangesAsync();
        return users;
    }

    [Fact]
    public async Task CreateUser_ShouldCreateNewUser_WhenInputIsValid()
    {
        // Arrange
        var users = AddUsers();
        var user = new User { name = "test", password = "test123123", type = UserType.SELLER };
        // Act
        await _repository.AddUser(user);
        // Assert
        var savedUser = await _context.Users.FirstOrDefaultAsync(u => u.name == user.name);
        Assert.NotNull(savedUser);
        Assert.Equal(user.name, savedUser.name);
        Assert.Equal(user.password, savedUser.password);
    }

    [Fact]
    public async Task CreateUser_ShouldCreateNewUserWithTypeBuyer_WhenTypeIsNull()
    {
        // Arrange
        var users = AddUsers();
        var user = new User { name = "noType", password = "noType123123" };
        // Act
        await _repository.AddUser(user);
        // Assert
        var savedUser = await _context.Users.FirstOrDefaultAsync(u => u.name == user.name);
        Assert.NotNull(savedUser);
        Assert.Equal(user.password, savedUser.password);
        Assert.Equal(UserType.BUYER, savedUser.type);
    }

    [Fact]
    public async Task CreateUser_ShouldThrowDuplicateUserNameException_WhenUserNameExists()
    {
        // Arrange
        var users = AddUsers();
        var user = new User { name = "Lisa", password = "test123123", type = UserType.BUYER };
        // Act & Assert
        await Assert.ThrowsAsync<DuplicateUserNameException>(async () => await _repository.AddUser(user));
    }
}