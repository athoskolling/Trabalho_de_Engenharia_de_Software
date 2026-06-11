using TaskManager.Application.Dtos.Task;

namespace TaskManager.Application.Services;

public interface ITaskService
{
    Task<TaskResponseDto> CreateAsync(CreateTaskDto dto, Guid createdById);

    Task<TaskResponseDto> GetByIdAsync(Guid id);

    Task<IEnumerable<TaskResponseDto>> GetFilteredAsync(TaskFilterDto filter);

    Task<TaskResponseDto> UpdateAsync(
        Guid id,
        UpdateTaskDto dto,
        Guid requestingUserId,
        string requestingUserRole
    );

    Task DeleteAsync(
        Guid id,
        Guid requestingUserId,
        string requestingUserRole
    );
}