using System.Security.Claims;
using InventoryManager.Application.Abstractions.Identity;
using InventoryManager.Application.Abstractions.Persistence.UnitOfWork;
using Microsoft.AspNetCore.Http;

namespace InventoryManager.Infrastructure.Identity;

public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork) : ICurrentUserService
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated == true;

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
    
    public string? Email => User?.FindFirstValue(ClaimTypes.Email);
    public string? Name => User?.FindFirstValue(ClaimTypes.Name);
    public string? Surname => User?.FindFirstValue(ClaimTypes.Surname);
    
    public async Task UpdateLastActivityAsync(CancellationToken ct)
    {
       var session =  await unitOfWork.SessionRepository.GetByAsync(s => s.UserId == UserId, ct)?? throw new Exception("Session not found");
        session.LastUsedAt = DateTime.UtcNow;
        unitOfWork.SessionRepository.Update(session);
       await  unitOfWork.SaveChangesAsync(CancellationToken.None);
    }
}