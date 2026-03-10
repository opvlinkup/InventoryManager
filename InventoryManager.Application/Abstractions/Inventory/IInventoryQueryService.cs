using InventoryManager.Application.DTO.Inventory;

namespace InventoryManager.Application.Abstractions.Inventory;

public interface IInventoryQueryService
{
    Task<InventoryDetailsDto> GetDetailsAsync(Guid id, CancellationToken ct);

    Task<IReadOnlyList<InventoryTableDto>> GetPagedAsync(InventoryFilterDto filter, CancellationToken ct);

    Task<IReadOnlyList<InventoryTableDto>> SearchAsync(InventorySearchDto dto, CancellationToken ct);

    Task<IReadOnlyList<InventoryTableDto>> GetLatestAsync(int count, CancellationToken ct);

    Task<IReadOnlyList<InventoryTableDto>> GetTopAsync(int count, CancellationToken ct);

    Task<IReadOnlyList<InventoryTableDto>> GetByTagAsync(string tag, CancellationToken ct);
}