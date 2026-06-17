using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManager.Application.Dtos.Auth;
using TaskManager.Application.Services.Interfaces;
using TaskManager.Domain.IServices;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IGoogleCalendar _calendarService;

    public AuthController(
        IAuthService authService,
        IGoogleCalendar calendarService)
    {
        _authService = authService;
        _calendarService = calendarService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto request)
    {
        var response = await _authService.LoginAsync(request);

        return Ok(response);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var userId = Guid.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        await _authService.LogoutAsync(userId);

        return NoContent();
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<ActionResult<LoginResponseDto>> Refresh(RefreshRequestDto request)
    {
        var response = await _authService.RefreshAsync(request.RefreshToken);

        return Ok(response);
    }

    [Authorize]
    [HttpGet("google/login")]
    public async Task<IActionResult> GoogleLogin()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var redirectUri = Url.Action(nameof(GoogleCallback), "Auth", null, Request.Scheme);

        var authUrl = await _calendarService.GetGoogleAuthorizationUrlAsync(userId, redirectUri!);

        return Ok(new { url = authUrl });
    }

    [AllowAnonymous]
    [HttpGet("google/callback")]
    public async Task<IActionResult> GoogleCallback([FromQuery] string code, [FromQuery] string state)
    {
        var userId = Guid.Parse(state);
        var redirectUri = Url.Action(nameof(GoogleCallback), "Auth", null, Request.Scheme);

        await _calendarService.ConnectGoogleAccountAsync(userId, code, redirectUri!);

        return Content("Google Calendar connected successfully! You can close this window now.");
    }
}