using InventoryManager.Application.Abstractions.Integration;
using InventoryManager.Application.Abstractions.Persistence.UnitOfWork;
using InventoryManager.Application.DTO.Integration.Odoo;
using InventoryManager.Domain.Models;

namespace InventoryManager.Infrastructure.Integration.Odoo;


public sealed class OdooService(IUnitOfWork unitOfWork) : IOdooService
{
    public async Task<ExternalInventoryDto> GetAggregatedDataAsync(string token, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token is required");

        var inventory = await unitOfWork.InventoryRepository.GetByAsync(inventory => inventory.ApiToken == token, cancellationToken);

        if (inventory is null) throw new InvalidOperationException("Invalid token");

        var items = await unitOfWork.ItemRepository.GetManyByAsync(
            item => item.InventoryId == inventory.Id,
            cancellationToken: cancellationToken);

        var fieldDefinitions = await unitOfWork.FieldMetadataRepository.GetManyByAsync(
            field => field.InventoryId == inventory.Id && field.State != FieldState.Disabled,
            orderBy: query => query.OrderBy(f => f.Order),
            cancellationToken: cancellationToken);

        var externalInventory = new ExternalInventoryDto
        {
            //...
        };

        return externalInventory;
    }
    
}