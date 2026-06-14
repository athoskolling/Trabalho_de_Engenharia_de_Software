using TaskManager.Domain.Enums;

namespace TaskManager.Application.Dtos.Task;

public class TaskFilterDto
{
    public TaskState? Status { get; set; }

    public TaskPriority? Priority { get; set; }

    public DateTime? DueBefore { get; set; }

    public Guid? AssignedToId { get; set; }
}
namespace TaskManager.Application.Dtos.Task;
