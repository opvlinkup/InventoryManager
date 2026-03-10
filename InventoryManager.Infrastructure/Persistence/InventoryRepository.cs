using InventoryManager.Application.Abstractions.Persistence.Repository;
using InventoryManager.Application.DTO.Inventory;
using InventoryManager.Domain.Models;
using InventoryManager.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace InventoryManager.Infrastructure.Persistence;

public sealed class InventoryRepository(InventoryManagerDbContext context) : Repository<Inventory>(context), IInventoryRepository
{
    private readonly InventoryManagerDbContext _context = context;

    public async Task<Inventory> GetForUpdateAsync(Guid id, byte[] rowVersion, CancellationToken ct)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Invalid id.", nameof(id));

        ArgumentNullException.ThrowIfNull(rowVersion);

        var entity = await _context.Inventories
                         .FirstOrDefaultAsync(x => x.Id == id, ct)
                     ?? throw new InvalidOperationException("Inventory not found.");

        _context.Entry(entity)
            .Property(x => x.RowVersion)
            .OriginalValue = rowVersion;

        return entity;
    }

    public async Task<InventoryAccessSnapshot?> GetAccessSnapshotAsync(Guid inventoryId, CancellationToken ct)
    {
        return await _context.Inventories
            .Where(i => i.Id == inventoryId)
            .Select(i => new InventoryAccessSnapshot(
                i.OwnerId,
                i.IsPublic,
                i.WriteAccessMode))
            .FirstOrDefaultAsync(ct);
    }

    public async Task<InventoryDetailsDto?> GetDetailsAsync(Guid id, CancellationToken ct)
    {
        return await _context.Inventories
            .AsNoTracking()
            .Where(i => i.Id == id)
            .Select(i => new InventoryDetailsDto
            {
                Id = i.Id,
                Title = i.Title,
                Description = i.Description,
                CategoryId = i.CategoryId,
                ImageUrl = i.ImageUrl,
                IsPublic = i.IsPublic,
                RowVersion = i.RowVersion
            })
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyList<InventoryTableDto>> GetPagedAsync(InventoryFilterDto filter, CancellationToken ct)
    {
        IQueryable<Inventory> query = _context.Inventories.AsNoTracking();

        if (filter.OwnerId.HasValue)
            query = query.Where(i => i.OwnerId == filter.OwnerId.Value);

        if (filter.CategoryId.HasValue)
            query = query.Where(i => i.CategoryId == filter.CategoryId.Value);

        query = filter.SortBy switch
        {
            "title" => filter.Desc
                ? query.OrderByDescending(i => i.Title)
                : query.OrderBy(i => i.Title),

            _ => query.OrderByDescending(i => i.CreatedAt)
        };

        var total = await query.CountAsync(ct);

        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(i => new InventoryTableDto
            {
                Id = i.Id,
                Title = i.Title,
                Description = i.Description,
                ImageUrl = i.ImageUrl,
                OwnerName = $"{i.Owner.Name} {i.Owner.Surname}",
                ItemsCount = i.Items.Count
            })
            .ToListAsync(ct);

        return items;
    }
    public async Task<IReadOnlyList<InventoryTableDto>> SearchAsync(InventorySearchDto dto, CancellationToken ct)
    {
        var query = _context.Inventories
            .AsNoTracking()
            .Where(i =>
                EF.Functions.ILike(i.Title, $"%{dto.Query}%") ||
                EF.Functions.ILike(i.Description!, $"%{dto.Query}%"));

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(i => i.CreatedAt)
            .Skip((dto.Page - 1) * dto.PageSize)
            .Take(dto.PageSize)
            .Select(i => new InventoryTableDto
            {
                Id = i.Id,
                Title = i.Title,
                Description = i.Description,
                ImageUrl = i.ImageUrl,
                OwnerName = $"{i.Owner.Name} {i.Owner.Surname}",
                ItemsCount = i.Items.Count
            })
            .ToListAsync(ct);
        return items;
    }
    public async Task<IReadOnlyList<InventoryTableDto>> GetLatestAsync(int count, CancellationToken ct)
    {
        return await _context.Inventories
            .AsNoTracking()
            .OrderByDescending(i => i.CreatedAt)
            .Take(count)
            .Select(i => new InventoryTableDto
            {
                Id = i.Id,
                Title = i.Title,
                Description = i.Description,
                ImageUrl = i.ImageUrl,
                OwnerName = $"{i.Owner.Name} {i.Owner.Surname}",
                ItemsCount = i.Items.Count
            })
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<InventoryTableDto>> GetTopAsync(int count, CancellationToken ct)
    {
        return await _context.Inventories
            .AsNoTracking()
            .OrderByDescending(i => i.Items.Count)
            .Take(count)
            .Select(i => new InventoryTableDto
            {
                Id = i.Id,
                Title = i.Title,
                Description = i.Description,
                ImageUrl = i.ImageUrl,
                OwnerName = $"{i.Owner.Name} {i.Owner.Surname}",
                ItemsCount = i.Items.Count
            })
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<InventoryTableDto>> GetByTagAsync(string tag, CancellationToken ct)
    {
        return await _context.Inventories
            .AsNoTracking()
            .Where(i => i.Tags.Any(t => t.Tag.Name == tag))
            .Select(i => new InventoryTableDto
            {
                Id = i.Id,
                Title = i.Title,
                Description = i.Description,
                ImageUrl = i.ImageUrl,
                OwnerName = $"{i.Owner.Name} {i.Owner.Surname}",
                ItemsCount = i.Items.Count
            })
            .ToListAsync(ct);
    }
}