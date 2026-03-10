using InventoryManager.Application.Abstractions.Comments;
using Microsoft.AspNetCore.SignalR;

namespace InventoryManager.Infrastructure.Hubs;

public sealed class DiscussionHub : Hub<IDiscussionClient>
{
    public async Task JoinInventoryGroup(Guid inventoryId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, inventoryId.ToString());
    }
    
    public async Task LeaveInventoryGroup(Guid inventoryId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, inventoryId.ToString());
    }
}