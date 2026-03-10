using InventoryManager.Application.Abstractions.Auth;
using InventoryManager.Application.DTO.Auth;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManager.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("google")]
    public async Task<ActionResult<AuthTokensDto>> GoogleLogin(
        [FromBody] GoogleAuthDto dto,
        CancellationToken ct)
    {
        var tokens = await authService.LoginWithGoogleAsync(dto.IdToken, ct);

        return Ok(tokens);
    }
}