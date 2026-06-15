namespace TaskManager.Application.Dtos.Task;
using TaskManager.Application.Dtos.User;
using TaskManager.Domain.Enums;

public class CommentResponseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public TaskState Status { get; set; }
    public TaskPriority Priority { get; set; }  
    public DateTime DueDate { get; set; }
    public UserResponseDto AssignedTo { get; set; }
    public UserResponseDto CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
