using InventoryManager.Domain.Models;

namespace InventoryManager.Application.DTO.Inventory;

public sealed record InventoryAccessSnapshot(Guid OwnerId, bool IsPublic, WriteAccessMode WriteAccessMode);