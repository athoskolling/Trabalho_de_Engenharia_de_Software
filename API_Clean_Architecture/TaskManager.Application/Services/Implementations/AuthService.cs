using TaskManager.Application.Dtos.Auth;
using TaskManager.Application.Services.Interfaces;
using TaskManager.Domain.IRepositories;

namespace TaskManager.Application.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        throw new NotImplementedException();
    }

    public async Task LogoutAsync(Guid userId)
    {
        await Task.CompletedTask;
    }
}