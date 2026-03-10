using InventoryManager.Application.Abstractions.Inventory.Fields;
using InventoryManager.Application.Abstractions.Persistence.Repository;
using InventoryManager.Application.DTO.Item;
using InventoryManager.Domain.Models;

namespace InventoryManager.Application.Services;

public class FieldMetadataService : IFieldMetadataService
{
    public bool ItemHasFieldValue(ItemDraftDto item, FieldType type, int slot)
    {
        return type switch
        {
            FieldType.Text => slot switch
            {
                1 => item.Text1 is not null,
                2 => item.Text2 is not null,
                3 => item.Text3 is not null,
                _ => false
            },

            FieldType.LongText => slot switch
            {
                1 => item.LongText1 is not null,
                2 => item.LongText2 is not null,
                3 => item.LongText3 is not null,
                _ => false
            },

            FieldType.Number => slot switch
            {
                1 => item.Number1 is not null,
                2 => item.Number2 is not null,
                3 => item.Number3 is not null,
                _ => false
            },

            FieldType.Bool => slot switch
            {
                1 => item.Bool1 is not null,
                2 => item.Bool2 is not null,
                3 => item.Bool3 is not null,
                _ => false
            },

            FieldType.Link => slot switch
            {
                1 => item.Link1 is not null,
                2 => item.Link2 is not null,
                3 => item.Link3 is not null,
                _ => false
            },

            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };
    }


    public FieldValue GetItemFieldValue(Item item, FieldMetadata field)
    {
        return field.Type switch
        {
            FieldType.Text => new FieldValue.TextValue(
                field.Slot switch { 1 => item.Text1, 2 => item.Text2, 3 => item.Text3, _ => null }),
            FieldType.LongText => new FieldValue.TextValue(
                field.Slot switch { 1 => item.LongText1, 2 => item.LongText2, 3 => item.LongText3, _ => null }),
            FieldType.Number => new FieldValue.NumberValue(
                field.Slot switch { 1 => item.Number1, 2 => item.Number2, 3 => item.Number3, _ => null }),
            FieldType.Bool => new FieldValue.BoolValue(
                field.Slot switch { 1 => item.Bool1, 2 => item.Bool2, 3 => item.Bool3, _ => null }),
            FieldType.Link => new FieldValue.TextValue(
                field.Slot switch { 1 => item.Link1, 2 => item.Link2, 3 => item.Link3, _ => null }),
            _ => throw new ArgumentOutOfRangeException(nameof(field.Type))
        };
    }
}