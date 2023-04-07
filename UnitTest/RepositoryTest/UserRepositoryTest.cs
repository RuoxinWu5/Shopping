using Data.Model;
using Data.Repository;
using Microsoft.EntityFrameworkCore;
using Data.Exceptions;

namespace UnitTest.RepositoryTest;

public class UserRepositoryTest
{
    private readonly MyDbContext _context;
    private readonly UserRepository _repository;

    public UserRepositoryTest()
    {
        var options = new DbContextOptionsBuilder<MyDbContext>()
            .UseInMemoryDatabase("UserList")
            .Options;
        _context = new MyDbContext(options);
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
        _repository = new UserRepository(_context);
    }

    private async Task<List<User>> AddUsers()
    {
        var users = new List<User>
        {
            new User { Name = "Lisa", Password = "lisa123", Type = UserType.BUYER } ,
            new User { Name = "Jack", Password = "Jack123", Type = UserType.SELLER }
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
        var user = new User { Name = "test", Password = "test123123", Type = UserType.SELLER };
        // Act
        await _repository.AddUser(user);
        // Assert
        var savedUser = await _context.Users.FirstOrDefaultAsync(u => u.Name == user.Name);
        Assert.NotNull(savedUser);
        Assert.Equal(user.Name, savedUser.Name);
        Assert.Equal(user.Password, savedUser.Password);
    }

    [Fact]
    public async Task CreateUser_ShouldCreateNewUserWithTypeBuyer_WhenTypeIsNull()
    {
        // Arrange
        var users = AddUsers();
        var user = new User { Name = "noType", Password = "noType123123" };
        // Act
        await _repository.AddUser(user);
        // Assert
        var savedUser = await _context.Users.FirstOrDefaultAsync(u => u.Name == user.Name);
        Assert.NotNull(savedUser);
        Assert.Equal(user.Password, savedUser.Password);
        Assert.Equal(UserType.BUYER, savedUser.Type);
    }

    [Fact]
    public async Task CreateUser_ShouldThrowDuplicateUserNameException_WhenUserNameExists()
    {
        // Arrange
        var users = AddUsers();
        var user = new User { Name = "Lisa", Password = "test123123", Type = UserType.SELLER };
        // Act & Assert
        await Assert.ThrowsAsync<DuplicateUserNameException>(async () => await _repository.AddUser(user));
    }
}