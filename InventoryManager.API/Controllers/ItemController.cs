using InventoryManager.Application.Abstractions.Identity;
using InventoryManager.Application.Abstractions.Inventory.Items;
using InventoryManager.Application.Abstractions.Inventory.Likes;
using InventoryManager.Application.DTO.Item;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManager.API.Controllers;

[ApiController]
[Route("api/items")]
public sealed class ItemController(IItemService itemService, ILikeService likeService, ICurrentUserService currentUser
) : ControllerBase
{
    [Authorize]
    [HttpPut("{itemId:guid}")]
    public async Task<IActionResult> Update(Guid itemId, [FromBody] UpdateItemDto dto, CancellationToken ct)
    {
        try
        {
            await itemService.UpdateItemAsync(
                itemId,
                dto,
                currentUser.UserId,
                ct);

            return NoContent();
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict(new
            {
                message = "Item was modified by another user."
            });
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }

    [Authorize]
    [HttpDelete("{itemId:guid}")]
    public async Task<IActionResult> Delete(Guid itemId, [FromBody] long rowVersion, CancellationToken ct)
    {
        if (rowVersion is < 0 or > uint.MaxValue)
            return BadRequest("Invalid rowVersion value.");
        
        try
        {
            await itemService.DeleteItemAsync(
                itemId,
                (uint)rowVersion,
                currentUser.UserId,
                ct);

            return NoContent();
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict(new
            {
                message = "Item was modified by another user."
            });
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }

    [Authorize]
    [HttpPost("{itemId:guid}/like")]
    public async Task<IActionResult> Like(
        Guid itemId,
        CancellationToken ct)
    {
        await likeService.LikeAsync(itemId, currentUser.UserId, ct);

        return NoContent();
    }

    [Authorize]
    [HttpDelete("{itemId:guid}/like")]
    public async Task<IActionResult> Unlike(
        Guid itemId,
        CancellationToken ct)
    {
        await likeService.UnlikeAsync(itemId, currentUser.UserId, ct);

        return NoContent();
    }
}