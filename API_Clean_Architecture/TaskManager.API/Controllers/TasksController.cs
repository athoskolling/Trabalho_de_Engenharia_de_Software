using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Dtos.Task;
using TaskManager.Application.Services.Interfaces;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpPost]
    public async Task<ActionResult<TaskResponseDto>> Create(CreateTaskDto dto)
    {
        // Temporário até JWT estar pronto
        var createdById = Guid.NewGuid();

        var task = await _taskService.CreateAsync(dto, createdById);

        return CreatedAtAction(
            nameof(GetById),
            new { id = task.Id },
            task);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TaskResponseDto>> GetById(Guid id)
    {
        var task = await _taskService.GetByIdAsync(id);

        return Ok(task);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskResponseDto>>> GetFiltered(
        [FromQuery] TaskFilterDto filter)
    {
        var tasks = await _taskService.GetFilteredAsync(filter);

        return Ok(tasks);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TaskResponseDto>> Update(
        Guid id,
        UpdateTaskDto dto)
    {
        // Temporário até JWT estar pronto
        var requestingUserId = Guid.NewGuid();
        var requestingUserRole = "Admin";

        var task = await _taskService.UpdateAsync(
            id,
            dto,
            requestingUserId,
            requestingUserRole);

        return Ok(task);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        // Temporário até JWT estar pronto
        var requestingUserId = Guid.NewGuid();
        var requestingUserRole = "Admin";

        await _taskService.DeleteAsync(
            id,
            requestingUserId,
            requestingUserRole);

        return NoContent();
    }
}