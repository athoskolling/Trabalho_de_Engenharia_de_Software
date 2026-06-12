using TaskManager.Application.Dtos.User;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Dtos.Task;

public class TaskResponseDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public TaskState Status { get; set; }

    public TaskPriority Priority { get; set; }

    public DateTime? DueDate { get; set; }

    public UserResponseDto? AssignedTo { get; set; } //aqui usa useresponsedto pro front n precisar saber detalhes do usuario, so o nome e id por exemplo

    public UserResponseDto CreatedBy { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}