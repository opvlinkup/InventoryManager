namespace InventoryManager.Application.Abstractions.Email;

public interface IEmailService
{
    Task SendConfirmationAsync(Guid userId, string email, string token);
}