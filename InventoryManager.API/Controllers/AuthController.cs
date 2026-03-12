using InventoryManager.Application.Abstractions.Auth;
using InventoryManager.Application.Abstractions.Identity;
using InventoryManager.Application.Abstractions.Security;
using InventoryManager.Application.DTO;
using InventoryManager.Application.DTO.Auth;
using InventoryManager.Domain.Exceptions;
using InventoryManager.Infrastructure.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManager.API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(
    IAuthService authService,
    ITokenService tokenService,
    ICurrentUserService currentUserService,
    ICookieService cookieService,
    ILogger<AuthController> logger) : ControllerBase
{

    [HttpPost("register")]
    [AllowAnonymous]
    [NoTransaction]
    public async Task<IActionResult> Register(
        [FromBody] UserRegisterDto dto,
        CancellationToken ct)
    {
        try
        {
            await authService.RegisterAsync(dto, ct);
            return Ok(new { message = "Registration successful" });
        }
        catch (ValidationException ex)
        {
            logger.LogWarning(ex, "Registration validation error");
            return BadRequest(new { error = ex.Message });
        }
        catch (EmailException ex)
        {
            logger.LogWarning(ex, "Email already exists");
            return Conflict(new { error = ex.Message });
        }
        catch (ApplicationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected registration error");
            return StatusCode(500, new { error = "Registration failed" });
        }
    }


    [HttpPost("login")]
    [AllowAnonymous]
    [NoTransaction]
    public async Task<IActionResult> Login(
        [FromBody] UserLoginDto dto,
        CancellationToken ct)
    {
        try
        {
            var tokens = await authService.LoginAsync(dto, ct);

            cookieService.AppendAuthCookies(Response, tokens);

            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { error = "Invalid credentials" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Login failed for {Email}", dto.Email);
            return StatusCode(500, new { error = "Login failed" });
        }
    }


    [HttpPost("refresh")]
    [AllowAnonymous]
    [NoTransaction]
    public async Task<IActionResult> Refresh(CancellationToken ct)
    {
        try
        {
            var refreshToken = cookieService.GetRefreshToken(Request);

            var tokens = await tokenService.RotateRefreshTokenAsync(refreshToken, ct);

            cookieService.AppendAuthCookies(Response, tokens);

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogWarning(ex, "Invalid refresh token");
            return Unauthorized(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Refresh token error");
            return StatusCode(500, new { error = "Token refresh failed" });
        }
    }


    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var refreshToken = cookieService.GetRefreshToken(Request);

        await authService.LogOutAsync(refreshToken, ct);

        cookieService.DeleteAuthCookies(Response);

        return NoContent();
    }


    [HttpGet("confirm-email")]
    [AllowAnonymous]
    [NoTransaction]
    public async Task<IActionResult> ConfirmEmail([FromQuery] Guid userId, [FromQuery] string token, CancellationToken ct)
    {
        var decoded = Uri.UnescapeDataString(token);

        await authService.ConfirmEmailAsync(userId, decoded, ct);

        return Ok("Email confirmed");
    }


    [HttpPost("google")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthTokensDto>> GoogleLogin(
        [FromBody] GoogleAuthDto dto,
        CancellationToken ct)
    {
        var tokens = await authService.LoginWithGoogleAsync(dto.IdToken, ct);

        cookieService.AppendAuthCookies(Response, tokens);

        return Ok(tokens);
    }


    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me(CancellationToken ct)
    {
        var user = await currentUserService.GetCurrentUserAsync(ct);
        return Ok(user);
    }
}