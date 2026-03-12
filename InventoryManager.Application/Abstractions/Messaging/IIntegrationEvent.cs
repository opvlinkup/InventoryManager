namespace InventoryManager.Application.Abstractions.Messaging;

public interface IIntegrationEvent
{
    Guid Id { get; }
    DateTime OccurredOn { get; }
    public string ConfirmationToken { get;  }
    public string Email { get; } 
    public Guid UserId { get; }


}