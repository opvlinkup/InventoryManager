namespace InventoryManager.Domain.Models;

public enum CustomIdPartType
{
    FixedText,
    Random20Bit,
    Random32Bit,
    Random6Digit,
    Random9Digit,
    Guid,
    DateTime,
    Sequence
}