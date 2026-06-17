using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManager.Application.Dtos.Task;
using TaskManager.Application.Services.Interfaces;

namespace TaskManager.API.Controllers;

[ApiController]
[Authorize]
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
        var createdById = GetAuthenticatedUserId();

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
        var requestingUserId = GetAuthenticatedUserId();
        var requestingUserRole = GetAuthenticatedUserRole();

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
        var requestingUserId = GetAuthenticatedUserId();
        var requestingUserRole = GetAuthenticatedUserRole();

        await _taskService.DeleteAsync(
            id,
            requestingUserId,
            requestingUserRole);

        return NoContent();
    }

    private Guid GetAuthenticatedUserId()
    {
        return Guid.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }

    private string GetAuthenticatedUserRole()
    {
        return User.FindFirst(ClaimTypes.Role)!.Value;
    }
}