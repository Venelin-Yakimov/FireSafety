using FireSafety.Api.Infrastructure.Auth;
using FireSafety.Api.View.DTOs;
using FireSafety.Model.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FireSafety.Api.Controllers;

/// <summary>Регистрация, вход и данни за текущия потребител.</summary>
[ApiController]
[Route("api/v1/auth")]
[Authorize]
public sealed class AuthController(AuthService authService) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType<UserDto>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Register(RegisterRequestDto dto)
    {
        var user = await authService.RegisterAsync(dto);
        return StatusCode(StatusCodes.Status201Created, user);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType<AuthResponseDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Login(LoginRequestDto dto) =>
        Ok(await authService.LoginAsync(dto));

    [HttpGet("me")]
    [ProducesResponseType<UserDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Me() =>
        Ok(await authService.GetCurrentUserAsync(User.GetUserId()));
}
