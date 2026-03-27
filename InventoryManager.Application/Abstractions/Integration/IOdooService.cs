using InventoryManager.Application.DTO.Integration.Odoo;

namespace InventoryManager.Application.Abstractions.Integration;

public interface IOdooService
{
    Task<ExportInventoryDto> GetAggregatedDataAsync(string token, CancellationToken ct);
}