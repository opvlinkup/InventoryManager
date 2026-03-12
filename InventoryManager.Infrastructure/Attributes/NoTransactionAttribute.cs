namespace InventoryManager.Infrastructure.Attributes;


[AttributeUsage(AttributeTargets.Method)]
public sealed class NoTransactionAttribute : Attribute
{
}