using TaskManager.Domain.Enums;

namespace TaskManager.Application.Dtos.User;

public class CreateUserDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public UserRole Role { get; set; }
}
