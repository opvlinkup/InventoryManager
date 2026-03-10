using InventoryManager.Application.Abstractions.Inventory;
using InventoryManager.Application.Abstractions.Persistence.UnitOfWork;
using InventoryManager.Application.DTO.Inventory;

namespace InventoryManager.Application.Services;

public sealed class InventoryQueryService(IUnitOfWork unitOfWork ) : IInventoryQueryService
{
    public async Task<InventoryDetailsDto> GetDetailsAsync(Guid id, CancellationToken ct)
    {
        return await unitOfWork.InventoryRepository.GetDetailsAsync(id, ct)
               ?? throw new InvalidOperationException("Inventory not found.");
    }

    public async  Task<IReadOnlyList<InventoryTableDto>> GetPagedAsync(InventoryFilterDto filter, CancellationToken ct)
    {
        return await unitOfWork.InventoryRepository.GetPagedAsync(filter, ct);
    }

    public async Task<IReadOnlyList<InventoryTableDto>> SearchAsync(InventorySearchDto dto, CancellationToken ct)
    {
        return await unitOfWork.InventoryRepository.SearchAsync(dto, ct);
    }

    public async Task<IReadOnlyList<InventoryTableDto>> GetLatestAsync(int count, CancellationToken ct)
    {
        return await unitOfWork.InventoryRepository.GetLatestAsync(count, ct);
    }

    public async Task<IReadOnlyList<InventoryTableDto>> GetTopAsync(int count, CancellationToken ct)
    {
        return await unitOfWork.InventoryRepository.GetTopAsync(count, ct);
    }

    public async Task<IReadOnlyList<InventoryTableDto>> GetByTagAsync(string tag, CancellationToken ct)
    {
        return await unitOfWork.InventoryRepository.GetByTagAsync(tag, ct);
    }
}