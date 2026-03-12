using System.Text.Json;
using InventoryManager.Application.Abstractions.Email;
using InventoryManager.Application.Events;
using InventoryManager.Infrastructure.Database;
using InventoryManager.Infrastructure.Email;
using Microsoft.EntityFrameworkCore;

namespace InventoryManager.Worker;

public sealed class OutboxProcessor(IServiceProvider serviceProvider, ILogger<OutboxProcessor> logger) : BackgroundService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Outbox processor started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessMessages(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Outbox processing cycle failed");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    private async Task ProcessMessages(CancellationToken ct)
    {
        using var scope = serviceProvider.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<InventoryManagerDbContext>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

        var messages = await db.OutboxMessages
            .Where(x => x.ProcessedOn == null)
            .OrderBy(x => x.OccurredOn)
            .Take(20)
            .ToListAsync(ct);

        foreach (var message in messages)
        {
            try
            {
                await ProcessMessage(message, emailService, ct);

                message.ProcessedOn = DateTime.UtcNow;
                message.Error = null;
            }
            catch (Exception ex)
            {
                message.Error = ex.ToString();

                logger.LogError(ex, "Failed to process outbox message {MessageId}", message.Id);
            }
        }

        await db.SaveChangesAsync(ct);
    }

    private static async Task ProcessMessage(OutboxMessage message, IEmailService emailService, CancellationToken ct)
    {
        switch (message.Type)
        {
            case nameof(UserRegisteredIntegrationEvent):

                var emailEvent =
                    JsonSerializer.Deserialize<UserRegisteredIntegrationEvent>(
                        message.Content,
                        JsonOptions)
                    ?? throw new InvalidOperationException("Invalid outbox payload");

                await emailService.SendConfirmationAsync(
                    emailEvent.UserId,
                    emailEvent.Email,
                    emailEvent.ConfirmationToken,
                    ct);

                break;
        }
    }
}