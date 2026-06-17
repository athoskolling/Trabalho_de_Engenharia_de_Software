using TaskManager.Application.Dtos.User;
using System;

namespace TaskManager.Application.Dtos.Comment;

public class CommentResponseDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public UserResponseDto User { get; set; } = null!;
}
