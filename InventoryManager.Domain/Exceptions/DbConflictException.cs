namespace InventoryManager.Domain.Exceptions;

public class DbConflictException(string? message) : Exception(message);