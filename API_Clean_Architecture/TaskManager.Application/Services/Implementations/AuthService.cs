using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TaskManager.Application.Dtos.Auth;
using TaskManager.Application.Services.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Domain.IRepositories;

namespace TaskManager.Application.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IConfiguration _configuration;

    public AuthService(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _configuration = configuration;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user is null)
            throw new UnauthorizedAccessException("Invalid email or password.");

        var passwordIsValid = BCrypt.Net.BCrypt.Verify(
            request.Password,
            user.PasswordHash);

        if (!passwordIsValid)
            throw new UnauthorizedAccessException("Invalid email or password.");

        var expiration = DateTime.UtcNow.AddMinutes(
            int.Parse(_configuration["Jwt:ExpirationMinutes"]!));

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        var secretKey = _configuration["Jwt:Secret"]!;
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secretKey));

        var credentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expiration,
            signingCredentials: credentials);

        var accessToken = new JwtSecurityTokenHandler()
            .WriteToken(token);

        var refreshTokenString = Guid.NewGuid().ToString();
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = refreshTokenString,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        };

        await _refreshTokenRepository.AddAsync(refreshToken);

        return new LoginResponseDto
        {
            AccessToken = accessToken,
            Expiration = expiration,
            RefreshToken = refreshTokenString
        };
    }

    public async Task LogoutAsync(Guid userId)
    {
        await _refreshTokenRepository.DeleteByUserIdAsync(userId);
    }

    public async Task<LoginResponseDto> RefreshAsync(string refreshTokenString)
    {
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshTokenString);

        if (storedToken is null || storedToken.IsRevoked || storedToken.ExpiresAt < DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");
        }

        var user = storedToken.User;
        var expiration = DateTime.UtcNow.AddMinutes(
            int.Parse(_configuration["Jwt:ExpirationMinutes"]!));

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        var secretKey = _configuration["Jwt:Secret"]!;
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secretKey));

        var credentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expiration,
            signingCredentials: credentials);

        var accessToken = new JwtSecurityTokenHandler()
            .WriteToken(token);

        // Revoga o token de refresh atual
        storedToken.IsRevoked = true;
        await _refreshTokenRepository.UpdateAsync(storedToken);

        // Gera um novo token de refresh
        var newRefreshTokenString = Guid.NewGuid().ToString();
        var newRefreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = newRefreshTokenString,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        };

        await _refreshTokenRepository.AddAsync(newRefreshToken);

        return new LoginResponseDto
        {
            AccessToken = accessToken,
            Expiration = expiration,
            RefreshToken = newRefreshTokenString
        };
    }
}