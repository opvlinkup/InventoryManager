using InventoryManager.Application.Abstractions.Identity;
using InventoryManager.Application.Abstractions.Inventory;
using InventoryManager.Application.Abstractions.Inventory.Comments;
using InventoryManager.Application.DTO.Comments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManager.Controllers;

[ApiController]
[Route("api/inventories/{inventoryId:guid}/discussion")]
public sealed class DiscussionController(IDiscussionService discussionService, ICurrentUserService currentUser) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CommentDto>>> Get(Guid inventoryId, CancellationToken ct)
    {
        var result = await discussionService.GetAsync(inventoryId, ct);
        
        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Add(Guid inventoryId, CreateCommentDto dto, CancellationToken ct)
    {
        
        await discussionService.AddAsync(inventoryId, dto, currentUser.UserId, ct);

        return NoContent();
    }
}