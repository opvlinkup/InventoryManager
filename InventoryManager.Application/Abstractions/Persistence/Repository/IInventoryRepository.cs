using InventoryManager.Application.DTO.Inventory;

namespace InventoryManager.Application.Abstractions.Persistence.Repository;

public interface IInventoryRepository : IRepository<Domain.Models.Inventory>
{
   public Task<InventoryAccessSnapshot?> GetAccessSnapshotAsync(Guid inventoryId, CancellationToken ct);

   public Task<Domain.Models.Inventory> GetForUpdateAsync(Guid id, byte[] rowVersion, CancellationToken ct);
   
   Task<InventoryDetailsDto?> GetDetailsAsync(Guid id, CancellationToken ct);

   Task<IReadOnlyList<InventoryTableDto>> GetPagedAsync(InventoryFilterDto filter, CancellationToken ct);

   Task<IReadOnlyList<InventoryTableDto>> SearchAsync(InventorySearchDto dto, CancellationToken ct);

   Task<IReadOnlyList<InventoryTableDto>> GetLatestAsync(int count, CancellationToken ct);

   Task<IReadOnlyList<InventoryTableDto>> GetTopAsync(int count, CancellationToken ct);

   Task<IReadOnlyList<InventoryTableDto>> GetByTagAsync(string tag, CancellationToken ct);
}