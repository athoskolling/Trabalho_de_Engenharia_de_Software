using TaskManager.Application.Dtos.Comment;
using TaskManager.Application.Dtos.User;
using TaskManager.Application.Services.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.IRepositories;

namespace TaskManager.Application.Services.Implementations;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;

    public CommentService(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public async Task<CommentResponseDto> AddAsync(
    Guid taskId,
    CreateCommentDto dto,
    Guid userId)
{
    var comment = new Comment
    {
        Id = Guid.NewGuid(),
        Content = dto.Content,
        TaskItemId = taskId,
        UserId = userId,
        CreatedAt = DateTime.UtcNow
    };

    var createdComment = await _commentRepository.AddAsync(comment);

    var commentWithUser = await _commentRepository.GetByIdAsync(createdComment.Id);

    if (commentWithUser is null)
        throw new KeyNotFoundException("Comment not found.");

    return new CommentResponseDto
    {
        Id = commentWithUser.Id,
        Content = commentWithUser.Content,
        CreatedAt = commentWithUser.CreatedAt,
        User = new UserResponseDto
        {
            Id = commentWithUser.User.Id,
            Name = commentWithUser.User.Name,
            Email = commentWithUser.User.Email,
            Role = commentWithUser.User.Role,
            CreatedAt = commentWithUser.User.CreatedAt
        }
    };
}

    public async Task<IEnumerable<CommentResponseDto>> GetByTaskIdAsync(Guid taskId)
    {
        var comments = await _commentRepository.GetByTaskIdAsync(taskId);

        return comments.Select(comment => new CommentResponseDto
        {
            Id = comment.Id,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt
        });
    }

    public async Task DeleteAsync(
        Guid commentId,
        Guid requestingUserId,
        string requestingUserRole)
    {
        var comment = await _commentRepository.GetByIdAsync(commentId);

        if (comment is null)
            throw new KeyNotFoundException("Comment not found.");

        var isOwner = comment.UserId == requestingUserId;
        var isAdmin = requestingUserRole == UserRole.Admin.ToString();

        if (!isOwner && !isAdmin)
            throw new UnauthorizedAccessException("You are not allowed to delete this comment.");

        var deleted = await _commentRepository.DeleteAsync(commentId);

        if (!deleted)
            throw new KeyNotFoundException("Comment not found.");
    }
}