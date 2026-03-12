using System.Security.Claims;
using InventoryManager.Application.DTO;

namespace InventoryManager.Application.Abstractions.Identity;

public interface ICurrentUserService
{
    Guid UserId { get; }
    bool IsAuthenticated { get; }
    string? Email { get; }
    string? IpAddress { get; }
    string? UserAgent { get; }
    public string? DeviceFingerprint { get; }
    bool IsInRole(string role);
    public Task UpdateLastActivityAsync(CancellationToken ct);
    Task<UserDto> GetCurrentUserAsync(CancellationToken ct);
}