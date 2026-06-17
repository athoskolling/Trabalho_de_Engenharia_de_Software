using Moq;
using TaskManager.Application.Dtos.Task;
using TaskManager.Application.Services.Implementations;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.IRepositories;
using TaskManager.Domain.IServices;
using Xunit;

namespace TaskManager.Tests;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _taskRepositoryMock;
    private readonly Mock<ICalendarService> _calendarServiceMock;
    private readonly TaskService _taskService;

    public TaskServiceTests()
    {
        _taskRepositoryMock = new Mock<ITaskRepository>();
        _calendarServiceMock = new Mock<ICalendarService>();
        _taskService = new TaskService(_taskRepositoryMock.Object, _calendarServiceMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateTaskAndTriggerCalendarEvent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dto = new CreateTaskDto
        {
            Title = "Test Task",
            Description = "Test Description",
            Priority = TaskPriority.High,
            DueDate = DateTime.UtcNow.AddDays(2),
            AssignedToId = Guid.NewGuid()
        };

        var expectedTask = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            Priority = dto.Priority,
            DueDate = dto.DueDate,
            AssignedToId = dto.AssignedToId,
            CreatedById = userId,
            State = TaskState.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _taskRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<TaskItem>()))
            .ReturnsAsync(expectedTask);

        // Act
        var result = await _taskService.CreateAsync(dto, userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.Title, result.Title);
        _taskRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<TaskItem>()), Times.Once);
        _calendarServiceMock.Verify(cal => cal.CreateEventAsync(
            dto.AssignedToId.Value,
            dto.Title,
            dto.Description,
            dto.DueDate), Times.Once);
    }
}
