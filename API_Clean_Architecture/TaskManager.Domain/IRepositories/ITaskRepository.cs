using TaskManager.Domain.Entities;

namespace TaskManager.Domain.IRepositories;

public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(Guid id);
    Task<IEnumerable<TaskItem>> GetAllAsync();
    Task<IEnumerable<TaskItem>> GetByAssignedUserAsync(Guid userId);
    Task<TaskItem> AddAsync(TaskItem task);
    Task<TaskItem> UpdateAsync(TaskItem task);
    Task<bool> DeleteAsync(Guid id);
}