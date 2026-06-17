using TaskManager.Domain.Enums;

namespace TaskManager.Application.Dtos.Task;

public class UpdateTaskDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }

    public TaskState? State { get; set; }

    public TaskPriority? Priority { get; set; }

    public DateTime? DueDate { get; set; }

    public Guid? AssignedToId { get; set; }
}
