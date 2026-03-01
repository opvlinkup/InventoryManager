namespace InventoryManager.Application.Abstractions.Security;

public interface IHashingService
{
    public byte[] ComputeHash(string token);
}