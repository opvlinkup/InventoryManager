namespace InventoryManager.Domain.Models;

public enum WriteAccessMode
{ 
    Restricted = 0,
    OwnerOnly = 1,
    AllAuthenticatedUsers = 2
}