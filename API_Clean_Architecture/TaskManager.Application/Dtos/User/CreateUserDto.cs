using TaskManager.Domain.Enums;

namespace TaskManager.Application.Dtos.User;

public class CreateUserDto
{
    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public UserRole Role { get; set; } = UserRole.User;
}
namespace TaskManager.Application.Dtos.User;
