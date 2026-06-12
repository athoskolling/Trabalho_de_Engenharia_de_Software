using TaskManager.Domain.Enums;

namespace TaskManager.Application.Dtos.Task;

public class UpdateTaskDto
{
    public string? Title { get; set; } //todos os campos são opcionais pois o usuário pode querer atualizar apenas um dos campos

    public string? Description { get; set; }

    public TaskState? Status { get; set; }

    public TaskPriority? Priority { get; set; }

    public DateTime? DueDate { get; set; }

    public Guid? AssignedToId { get; set; }
}