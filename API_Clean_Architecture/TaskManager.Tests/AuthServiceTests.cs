using Microsoft.Extensions.Configuration;
using Moq;
using TaskManager.Application.Dtos.Auth;
using TaskManager.Application.Services.Implementations;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.IRepositories;
using Xunit;

namespace TaskManager.Tests;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
        _configurationMock = new Mock<IConfiguration>();
        _authService = new AuthService(
            _userRepositoryMock.Object,
            _refreshTokenRepositoryMock.Object,
            _configurationMock.Object);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnTokensAndPersistRefreshToken()
    {
        // Arrange
        var password = "securepassword";
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "John Doe",
            Email = "john@example.com",
            PasswordHash = passwordHash,
            Role = UserRole.User,
            CreatedAt = DateTime.UtcNow
        };

        var request = new LoginRequestDto
        {
            Email = user.Email,
            Password = password
        };

        _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(user.Email))
            .ReturnsAsync(user);

        _configurationMock.Setup(cfg => cfg["Jwt:ExpirationMinutes"]).Returns("60");
        _configurationMock.Setup(cfg => cfg["Jwt:Secret"]).Returns("minha-chave-super-secreta-com-mais-de-32-caracteres");
        _configurationMock.Setup(cfg => cfg["Jwt:Issuer"]).Returns("TaskManager.API");
        _configurationMock.Setup(cfg => cfg["Jwt:Audience"]).Returns("TaskManager.Client");

        _refreshTokenRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<RefreshToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.AccessToken);
        Assert.NotEmpty(result.RefreshToken);
        _refreshTokenRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<RefreshToken>()), Times.Once);
    }
}
