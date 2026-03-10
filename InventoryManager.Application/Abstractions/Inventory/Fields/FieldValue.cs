namespace InventoryManager.Application.Abstractions.Inventory.Fields;

public abstract record FieldValue
{
    public sealed record TextValue(string? Value) : FieldValue;
    public sealed record NumberValue(decimal? Value) : FieldValue;
    public sealed record BoolValue(bool? Value) : FieldValue;
}
