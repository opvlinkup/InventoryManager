using InventoryManager.Application.Abstractions.Persistence.UnitOfWork;
using InventoryManager.Infrastructure.Attributes;
using Microsoft.EntityFrameworkCore;

namespace InventoryManager.Middleware;

public sealed class TransactionMiddleware(
    RequestDelegate next,
    ILogger<TransactionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context, IUnitOfWork unitOfWork)
    {
        var endpoint = context.GetEndpoint();
        var noTransaction = endpoint?.Metadata.GetMetadata<NoTransactionAttribute>() != null;
        
        if (noTransaction ||
            HttpMethods.IsGet(context.Request.Method) ||
            HttpMethods.IsHead(context.Request.Method) ||
            HttpMethods.IsOptions(context.Request.Method))
        {
            await next(context);
            return;
        }

        await unitOfWork.BeginTransactionAsync(context.RequestAborted);

        try
        {
            await next(context);

            if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
            {
                logger.LogInformation("Transaction commited for {Method} {Path}", context.Request.Method,
                    context.Request.Path);
                await unitOfWork.CommitTransactionAsync(context.RequestAborted);
            }
            else
            {
                logger.LogWarning("Transaction rollback due to status code: {StatusCode}", context.Response.StatusCode);
                await unitOfWork.RollbackTransactionAsync(context.RequestAborted);
            }
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Transaction rollback due to a database error ");
            await unitOfWork.RollbackTransactionAsync(CancellationToken.None);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Transaction rollback due to an unhandled exception");
            await unitOfWork.RollbackTransactionAsync(CancellationToken.None);
            throw;
        }
    }
}