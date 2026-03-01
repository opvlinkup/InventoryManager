namespace InventoryManager.Domain.Exceptions;

public class EmailException(string? email) : Exception($"{email} is already taken");