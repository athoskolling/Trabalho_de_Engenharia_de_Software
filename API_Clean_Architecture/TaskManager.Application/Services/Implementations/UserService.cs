using TaskManager.Application.Dtos.User;
using TaskManager.Application.Services.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.IRepositories;

namespace TaskManager.Application.Services.Implementations;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserResponseDto> CreateAsync(CreateUserDto dto)
    {
        var existingUser = await _userRepository.GetByEmailAsync(dto.Email);

        if (existingUser is not null)
            throw new InvalidOperationException("Email already in use.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = dto.Role,
            CreatedAt = DateTime.UtcNow
        };

        var createdUser = await _userRepository.AddAsync(user);

        return MapToResponseDto(createdUser);
    }

    public async Task<UserResponseDto> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user is null)
            throw new KeyNotFoundException("User not found.");

        return MapToResponseDto(user);
    }

    public async Task<IEnumerable<UserResponseDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();

        return users.Select(MapToResponseDto);
    }

    public async Task<UserResponseDto> UpdateAsync(
        Guid id,
        UpdateUserDto dto,
        Guid requestingUserId,
        string requestingUserRole)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user is null)
            throw new KeyNotFoundException("User not found.");

        var isOwner = id == requestingUserId;
        var isAdmin = requestingUserRole == UserRole.Admin.ToString();

        if (!isOwner && !isAdmin)
            throw new UnauthorizedAccessException("You are not allowed to update this user.");

        if (dto.Name is not null)
            user.Name = dto.Name;

        if (dto.Email is not null)
        {
            var existingUser = await _userRepository.GetByEmailAsync(dto.Email);

            if (existingUser is not null && existingUser.Id != user.Id)
                throw new InvalidOperationException("Email already in use.");

            user.Email = dto.Email;
        }
        
        if (dto.Password is not null)
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        }

        var updatedUser = await _userRepository.UpdateAsync(user);

        return MapToResponseDto(updatedUser);
    }

    public async Task DeleteAsync(Guid id)
    {
        var deleted = await _userRepository.DeleteAsync(id);

        if (!deleted)
            throw new KeyNotFoundException("User not found.");
    }

    private static UserResponseDto MapToResponseDto(User user)
    {
        return new UserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };
    }
}