using InventoryManager.Application.Abstractions.Email;

namespace InventoryManager.Infrastructure.Email;

public class EmailService : IEmailService
{
    public Task SendConfirmationAsync(Guid userId, string email, string token)
    {
        throw new NotImplementedException();
    }
}