namespace InventoryManager.Infrastructure.Email;

public class OutboxMessage
{
    public Guid Id { get; set; }

    public string Type { get; set; } = null!;

    public string Content { get; set; } = null!;

    public DateTime OccurredOn { get; set; }

    public DateTime? ProcessedOn { get; set; }

    public string? Error { get; set; }
}