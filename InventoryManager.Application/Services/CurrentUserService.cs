using System.Security.Claims;
using InventoryManager.Application.Abstractions.Identity;
using InventoryManager.Application.Abstractions.Persistence.UnitOfWork;
using InventoryManager.Application.DTO;
using Microsoft.AspNetCore.Http;

namespace InventoryManager.Infrastructure.Identity;

public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork) : ICurrentUserService
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated == true;
    public string? Email => User?.FindFirst(ClaimTypes.Email)?.Value;
    public string? IpAddress => httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
    public string? UserAgent => httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString();
    public string? DeviceFingerprint => httpContextAccessor.HttpContext?.Request.Headers["X-Device-Fingerprint"].ToString();


    public Guid UserId
    {
        get
        {
            if (!IsAuthenticated)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var value = User!.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(value))
                throw new UnauthorizedAccessException("User identifier claim missing.");

            return Guid.Parse(value);
        }
    }

    public bool IsInRole(string role)
    {
        if (!IsAuthenticated)
            return false;

        return User!.IsInRole(role);
    }
    
    
    public async Task UpdateLastActivityAsync(CancellationToken ct)
    {
       var session =  await unitOfWork.SessionRepository.GetByAsync(s => s.UserId == UserId, ct)?? throw new Exception("Session not found");
        session.LastUsedAt = DateTime.UtcNow;
        unitOfWork.SessionRepository.Update(session);
       await  unitOfWork.SaveChangesAsync(CancellationToken.None);
    }
    
    public async Task<UserDto> GetCurrentUserAsync(CancellationToken ct)
    {
        if (!IsAuthenticated)
            throw new UnauthorizedAccessException("User not authenticated.");

        var userId = UserId;

        var user = await unitOfWork.UserRepository
                       .GetByAsync(u => u.Id == userId, ct)
                   ?? throw new UnauthorizedAccessException("User not found.");

        var session = await unitOfWork.SessionRepository
            .GetByAsync(s => s.UserId == userId, ct);

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email!,
            Name = user.Name!,
            Language = user.Language,
            Status = user.Status,
            Theme = user.Theme,
            LastSeenAt = session?.LastUsedAt
        };
    }
}