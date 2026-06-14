using TaskManager.Domain.Entities;

namespace TaskManager.Domain.IRepositories;

public interface ICommentRepository
{
 Task<IEnumerable<Comment>> GetByTaskIdAsync(Guid taskId);
    Task<Comment?> GetByIdAsync(Guid id);
    Task<Comment> AddAsync(Comment comment);
    Task<bool> DeleteAsync(Guid id);
}