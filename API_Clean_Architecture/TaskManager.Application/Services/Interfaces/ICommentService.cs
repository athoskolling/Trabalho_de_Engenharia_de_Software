using TaskManager.Application.Dtos.Comment;

namespace TaskManager.Application.Services;

public interface ICommentService
{
    Task<CommentResponseDto> AddAsync(
        Guid taskId,
        CreateCommentDto dto,
        Guid userId
    );

    Task<IEnumerable<CommentResponseDto>> GetByTaskIdAsync(Guid taskId);

    Task DeleteAsync(
        Guid commentId,
        Guid requestingUserId,
        string requestingUserRole
    );
}