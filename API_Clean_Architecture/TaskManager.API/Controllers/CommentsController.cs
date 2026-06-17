using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManager.Application.Dtos.Comment;
using TaskManager.Application.Services.Interfaces;

namespace TaskManager.API.Controllers;

[ApiController]
[Authorize]
[Route("api/comments")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentsController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpPost("task/{taskId:guid}")]
    public async Task<ActionResult<CommentResponseDto>> Create(
        Guid taskId,
        CreateCommentDto dto)
    {
        var userId = GetAuthenticatedUserId();

        var comment = await _commentService.AddAsync(
            taskId,
            dto,
            userId);

        return Ok(comment);
    }

    [HttpGet("task/{taskId:guid}")]
    public async Task<ActionResult<IEnumerable<CommentResponseDto>>> GetByTaskId(
        Guid taskId)
    {
        var comments = await _commentService.GetByTaskIdAsync(taskId);

        return Ok(comments);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var requestingUserId = GetAuthenticatedUserId();
        var requestingUserRole = GetAuthenticatedUserRole();

        await _commentService.DeleteAsync(
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