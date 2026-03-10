using InventoryManager.Application.DTO.Item;
using InventoryManager.Domain.Models;

namespace InventoryManager.Application.Abstractions.Inventory.Fields;

public interface IFieldMetadataService
{
    public FieldValue GetItemFieldValue(Item item, FieldMetadata field);
    bool ItemHasFieldValue(ItemDraftDto item, FieldType type, int slot);
}