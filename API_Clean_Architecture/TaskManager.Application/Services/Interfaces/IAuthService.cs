using TaskManager.Application.Dtos.Auth;

namespace TaskManager.Application.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);

    Task LogoutAsync(Guid userId); //Task sem <> significa que não retorna nada
}