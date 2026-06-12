using TaskManager.Application.Dtos.User;

namespace TaskManager.Application.Services.Interfaces;

public interface IUserService
{
    Task<UserResponseDto> CreateAsync(CreateUserDto dto);

    Task<UserResponseDto> GetByIdAsync(Guid id);

    Task<IEnumerable<UserResponseDto>> GetAllAsync();

    Task<UserResponseDto> UpdateAsync(
        Guid id,
        UpdateUserDto dto, //é o que será alterado
        Guid requestingUserId, //tem que saber quem está fazendo a requisição para validar se tem permissão de atualizar
        string requestingUserRole
    );

    Task DeleteAsync(Guid id);
}