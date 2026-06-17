using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManager.Application.Dtos.User;
using TaskManager.Application.Services.Interfaces;

namespace TaskManager.API.Controllers;

[ApiController]
[Authorize]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<UserResponseDto>> Create(CreateUserDto dto)
    {
        var user = await _userService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = user.Id },
            user);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAll()
    {
        var users = await _userService.GetAllAsync();

        return Ok(users);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserResponseDto>> GetById(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);

        return Ok(user);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UserResponseDto>> Update(Guid id, UpdateUserDto dto)
    {
        var requestingUserId = GetAuthenticatedUserId();
        var requestingUserRole = GetAuthenticatedUserRole();

        var user = await _userService.UpdateAsync(
            id,
            dto,
            requestingUserId,
            requestingUserRole);

        return Ok(user);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _userService.DeleteAsync(id);

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