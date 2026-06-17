using TaskManager.Application.Dtos.Task;
using TaskManager.Application.Dtos.User;
using TaskManager.Application.Services.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.IRepositories;
using TaskManager.Domain.IServices;

namespace TaskManager.Application.Services.Implementations;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IGoogleCalendar _calendarService;

    public TaskService(
        ITaskRepository taskRepository,
        IGoogleCalendar calendarService)
    {
        _taskRepository = taskRepository;
        _calendarService = calendarService;
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

        // Agendar evento no Google Calendar
        var targetUserId = task.AssignedToId ?? createdById;
        await _calendarService.CreateEventAsync(targetUserId, task.Title, task.Description, task.DueDate);

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

        if (filter.State.HasValue)
            tasks = tasks.Where(task => task.State == filter.State.Value);

        if (filter.Priority.HasValue)
            tasks = tasks.Where(task => task.Priority == filter.Priority.Value);

        if (filter.DueBefore.HasValue)
            tasks = tasks.Where(task => task.DueDate <= filter.DueBefore.Value);

        if (filter.AssignedToId.HasValue)
            tasks = tasks.Where(task => task.AssignedToId == filter.AssignedToId.Value);

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

        var isCreator = task.CreatedById == requestingUserId;
        var isAssigned = task.AssignedToId == requestingUserId;
        var isAdmin = requestingUserRole == UserRole.Admin.ToString();

        if (!isCreator && !isAssigned && !isAdmin)
            throw new UnauthorizedAccessException("You are not allowed to update this task.");

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

        // Agendar evento no Google Calendar
        var targetUserId = task.AssignedToId ?? requestingUserId;
        await _calendarService.CreateEventAsync(targetUserId, task.Title, task.Description, task.DueDate);

        return MapToResponseDto(updatedTask);
    }

    public async Task DeleteAsync(
        Guid id,
        Guid requestingUserId,
        string requestingUserRole)
    {
        var task = await _taskRepository.GetByIdAsync(id);

        if (task is null)
            throw new KeyNotFoundException("Task not found.");

        var isCreator = task.CreatedById == requestingUserId;
        var isAdmin = requestingUserRole == UserRole.Admin.ToString();

        if (!isCreator && !isAdmin)
            throw new UnauthorizedAccessException("You are not allowed to delete this task.");

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
            UpdatedAt = task.UpdatedAt,
            AssignedTo = task.AssignedTo != null ? new UserResponseDto
            {
                Id = task.AssignedTo.Id,
                Name = task.AssignedTo.Name,
                Email = task.AssignedTo.Email,
                Role = task.AssignedTo.Role,
                CreatedAt = task.AssignedTo.CreatedAt
            } : null,
            CreatedBy = task.CreatedBy != null ? new UserResponseDto
            {
                Id = task.CreatedBy.Id,
                Name = task.CreatedBy.Name,
                Email = task.CreatedBy.Email,
                Role = task.CreatedBy.Role,
                CreatedAt = task.CreatedBy.CreatedAt
            } : null!
        };
    }
}