using TaskManager.Domain.Enums;

namespace TaskManager.Application.Dtos.Task;

public class CreateTaskDto
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    public DateTime? DueDate { get; set; }

    public Guid? AssignedToId { get; set; }
}
namespace TaskManager.Application.Dtos.Task;
