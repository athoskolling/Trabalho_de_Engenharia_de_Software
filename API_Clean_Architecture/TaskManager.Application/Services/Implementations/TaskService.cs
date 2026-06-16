using TaskManager.Application.Dtos.Task;
using TaskManager.Application.Dtos.User;
using TaskManager.Application.Services.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.IRepositories;

namespace TaskManager.Application.Services.Implementations;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;

    public TaskService(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<TaskResponseDto> CreateAsync(
        CreateTaskDto dto,
        Guid createdById)
    {
        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            Priority = dto.Priority,
            DueDate = dto.DueDate,
            AssignedToId = dto.AssignedToId,
            CreatedById = createdById,
            State = TaskState.Pending,
            CreatedAt = DateTime.UtcNow
        };

        var createdTask = await _taskRepository.AddAsync(task);

        return MapToResponseDto(createdTask);
    }

    public async Task<TaskResponseDto> GetByIdAsync(Guid id)
    {
        var task = await _taskRepository.GetByIdAsync(id);

        if (task is null)
            throw new KeyNotFoundException("Task not found.");

        return MapToResponseDto(task);
    }

    public async Task<IEnumerable<TaskResponseDto>> GetFilteredAsync(TaskFilterDto filter)
    {
        var tasks = await _taskRepository.GetAllAsync();

        return tasks.Select(MapToResponseDto);
    }

    public async Task<TaskResponseDto> UpdateAsync(
        Guid id,
        UpdateTaskDto dto,
        Guid requestingUserId,
        string requestingUserRole)
    {
        var task = await _taskRepository.GetByIdAsync(id);

        if (task is null)
            throw new KeyNotFoundException("Task not found.");

        if (dto.Title is not null)
            task.Title = dto.Title;

        if (dto.Description is not null)
            task.Description = dto.Description;

        if (dto.State.HasValue)
            task.State = dto.State.Value;

        if (dto.Priority.HasValue)
            task.Priority = dto.Priority.Value;

        if (dto.DueDate.HasValue)
            task.DueDate = dto.DueDate;

        task.AssignedToId = dto.AssignedToId;

        task.UpdatedAt = DateTime.UtcNow;

        var updatedTask = await _taskRepository.UpdateAsync(task);

        return MapToResponseDto(updatedTask);
    }

    public async Task DeleteAsync(
        Guid id,
        Guid requestingUserId,
        string requestingUserRole)
    {
        var deleted = await _taskRepository.DeleteAsync(id);

        if (!deleted)
            throw new KeyNotFoundException("Task not found.");
    }

    private static TaskResponseDto MapToResponseDto(TaskItem task)
    {
        return new TaskResponseDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            State = task.State,
            Priority = task.Priority,
            DueDate = task.DueDate,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };
    }
}