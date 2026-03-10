using InventoryManager.Application.Abstractions.Identity;
using InventoryManager.Application.Abstractions.Session;

namespace InventoryManager.Middleware;

public sealed class LastActivityMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ICurrentUserService currentUser, ISessionService sessionService)
    {
        if (currentUser.IsAuthenticated)
        {
            await sessionService.UpdateLastActivityAsync(currentUser.UserId, context.RequestAborted);
        }

        await next(context);
    }
}