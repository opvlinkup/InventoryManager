using InventoryManager.Application.Abstractions.Identity;
using InventoryManager.Application.Abstractions.Security;
using InventoryManager.Application.DTO.Admin;
using InventoryManager.Application.DTO.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManager.API.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public sealed class AdminController(IAdminService adminService, ICurrentUserService currentUser
) : ControllerBase
{
    [HttpGet("users")]
    public async Task<ActionResult<IReadOnlyList<UserAdminDto>>> GetUsers([FromQuery] UserFilterDto filter, CancellationToken ct)
    {
        var result = await adminService.GetUsersAsync(filter, ct);
        return Ok(result);
    }

    [HttpPut("users/{userId:guid}/block")]
    public async Task<IActionResult> Block(Guid userId, CancellationToken ct)
    {
        await adminService.BlockAsync(userId, currentUser.UserId, ct);
        return NoContent();
    }

    [HttpPut("users/{userId:guid}/unblock")]
    public async Task<IActionResult> Unblock(Guid userId, CancellationToken ct)
    {
        await adminService.UnblockAsync(userId, ct);
        return NoContent();
    }

    [HttpPut("users/{userId:guid}/grant-admin")]
    public async Task<IActionResult> GrantAdmin(Guid userId, CancellationToken ct)
    {
        await adminService.GrantAdminAsync(userId, ct);
        return NoContent();
    }

    [HttpPut("users/{userId:guid}/revoke-admin")]
    public async Task<IActionResult> RevokeAdmin(Guid userId, CancellationToken ct)
    {
        await adminService.RevokeAdminAsync(userId, currentUser.UserId, ct);
        return NoContent();
    }

    [HttpDelete("users/{userId:guid}")]
    public async Task<IActionResult> DeleteUser(Guid userId, CancellationToken ct)
    {
        await adminService.DeleteAsync(userId, currentUser.UserId, ct);
        return NoContent();
    }
}