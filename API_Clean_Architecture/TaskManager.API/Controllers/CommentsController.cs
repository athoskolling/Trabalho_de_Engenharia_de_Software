using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Dtos.Comment;
using TaskManager.Application.Services.Interfaces;

namespace TaskManager.API.Controllers;

[ApiController]
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
        // Temporário até JWT
        var userId = Guid.NewGuid();

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
        // Temporário até JWT
        var requestingUserId = Guid.NewGuid();
        var requestingUserRole = "Admin";

        await _commentService.DeleteAsync(
            id,
            requestingUserId,
            requestingUserRole);

        return NoContent();
    }
}