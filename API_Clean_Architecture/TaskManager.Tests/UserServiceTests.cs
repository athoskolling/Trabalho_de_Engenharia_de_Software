using Moq;
using TaskManager.Application.Dtos.User;
using TaskManager.Application.Services.Implementations;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.IRepositories;
using Xunit;

namespace TaskManager.Tests;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userService = new UserService(_userRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WithUniqueEmail_ShouldCreateUserAndHashPassword()
    {
        // Arrange
        var dto = new CreateUserDto
        {
            Name = "Alice",
            Email = "alice@example.com",
            Password = "mypassword123",
            Role = UserRole.User
        };

        _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(dto.Email))
            .ReturnsAsync((User?)null);

        User? capturedUser = null;
        _userRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<User>()))
            .Callback<User>(u => capturedUser = u)
            .ReturnsAsync((User u) => u);

        // Act
        var result = await _userService.CreateAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.Name, result.Name);
        Assert.Equal(dto.Email, result.Email);

        Assert.NotNull(capturedUser);
        Assert.Equal(dto.Name, capturedUser.Name);
        Assert.Equal(dto.Email, capturedUser.Email);
        Assert.NotEqual(dto.Password, capturedUser.PasswordHash);
        Assert.True(BCrypt.Net.BCrypt.Verify(dto.Password, capturedUser.PasswordHash));

        _userRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateEmail_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var dto = new CreateUserDto
        {
            Name = "Alice",
            Email = "alice@example.com",
            Password = "mypassword123",
            Role = UserRole.User
        };

        var existingUser = new User
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            Name = "Old Alice"
        };

        _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(dto.Email))
            .ReturnsAsync(existingUser);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.CreateAsync(dto));
        _userRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Never);
    }
}
