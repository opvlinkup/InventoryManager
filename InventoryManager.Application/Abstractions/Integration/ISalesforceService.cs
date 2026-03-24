using InventoryManager.Application.DTO.Integration.Salesforce;

namespace InventoryManager.Application.Abstractions.Integration;

public interface ISalesforceService
{
    Task<string> CreateCustomerAsync(SalesforceCustomerDto dto, CancellationToken ct);
}