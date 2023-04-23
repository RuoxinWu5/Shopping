using Data.Model;
using Data.Repository;
using Microsoft.EntityFrameworkCore;
using Data.Exceptions;

namespace UnitTest.RepositoryTest;

public class UserRepositoryTest
{
    private readonly ShoppingDbContext _context;
    private readonly UserRepository _repository;

    public UserRepositoryTest()
    {
        var options = new DbContextOptionsBuilder<ShoppingDbContext>()
            .UseInMemoryDatabase("UserList")
            .Options;
        _context = new ShoppingDbContext(options);
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
    public async Task GetUserById_ShouldReturnUser_WhenUserIsfound()
    {
        // Arrange
        var users = await AddUsers();
        // Act
        var result = await _repository.GetUserById(users[0].Id);
        // Assert
        Assert.Equal(users[0], result);
    }

    [Fact]
    public async Task GetUserById_ShouldReturnNotFoundException_WhenUserIsNotfound()
    {
        // Arrange
        var users = await AddUsers();
        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async ()=>await _repository.GetUserById(3));
    }

    [Fact]
    public async Task GetUserByName_ShouldReturnUser_WhenUserIsfound()
    {
        // Arrange
        var users = await AddUsers();
        // Act
        var result = await _repository.GetUserByName(users[0].Name);
        // Assert
        Assert.Equal(users[0], result);
    }

    [Fact]
    public async Task GetUserByName_ShouldReturnNotFoundException_WhenUserIsNotfound()
    {
        // Arrange
        var users = await AddUsers();
        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async ()=>await _repository.GetUserByName("Tizzy"));
    }
}