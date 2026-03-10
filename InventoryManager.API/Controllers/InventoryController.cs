using InventoryManager.Application.Abstractions.Identity;
using InventoryManager.Application.Abstractions.Inventory;
using InventoryManager.Application.Abstractions.Inventory.Items;
using InventoryManager.Application.DTO.Inventory;
using InventoryManager.Application.DTO.Item;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManager.API.Controllers;

[ApiController]
[Route("api/inventories")]
public sealed class InventoryController(
    IInventoryManagementService managementService,
    IInventoryQueryService queryService,
    ICurrentUserService currentUser,
    IItemService itemService
) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<InventoryDetailsDto>> Get(Guid id, CancellationToken ct)
    {
        try
        {
            var result = await queryService.GetDetailsAsync(id, ct);
            return Ok(result);
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }


    [HttpGet]
    public async Task<ActionResult<PagedResult<InventoryTableDto>>> GetPaged([FromQuery] InventoryFilterDto filter,
        CancellationToken ct)
    {
        var result = await queryService.GetPagedAsync(filter, ct);
        return Ok(result);
    }


    [HttpGet("search")]
    public async Task<ActionResult<PagedResult<InventoryTableDto>>> Search([FromQuery] InventorySearchDto dto,
        CancellationToken ct)
    {
        var result = await queryService.SearchAsync(dto, ct);
        return Ok(result);
    }


    [HttpGet("latest")]
    public async Task<ActionResult<IReadOnlyList<InventoryTableDto>>> GetLatest(
        [FromQuery] int count = 10,
        CancellationToken ct = default)
    {
        var result = await queryService.GetLatestAsync(count, ct);
        return Ok(result);
    }


    [HttpGet("top")]
    public async Task<ActionResult<IReadOnlyList<InventoryTableDto>>> GetTop(
        [FromQuery] int count = 10,
        CancellationToken ct = default)
    {
        var result = await queryService.GetTopAsync(count, ct);
        return Ok(result);
    }


    [HttpGet("tag/{tag}")]
    public async Task<ActionResult<IReadOnlyList<InventoryTableDto>>> GetByTag(
        string tag,
        CancellationToken ct)
    {
        var result = await queryService.GetByTagAsync(tag, ct);
        return Ok(result);
    }


    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Guid>> Create(
        [FromBody] CreateInventoryDto dto,
        CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated)
            return Unauthorized();

        var id = await managementService.CreateInventoryAsync(
            dto,
            currentUser.UserId,
            ct);

        return CreatedAtAction(nameof(Get), new { id }, id);
    }


    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateInventoryDto dto, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated)
            return Unauthorized();

        try
        {
            await managementService.UpdateInventoryAsync(id, dto, currentUser.UserId, ct);

            return NoContent();
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict(new
            {
                message = "Inventory was modified by another user."
            });
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }


    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        [FromBody] byte[] rowVersion,
        CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated)
            return Unauthorized();

        try
        {
            await managementService.DeleteInventoryAsync(
                id,
                currentUser.UserId,
                rowVersion,
                ct);

            return NoContent();
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict(new
            {
                message = "Inventory was modified by another user."
            });
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }

    [Authorize]
    [HttpPost("{inventoryId:guid}/items")]
    public async Task<ActionResult<Guid>> CreateItem(
        Guid inventoryId,
        [FromBody] ItemDraftDto dto,
        CancellationToken ct)
    {
        var id = await itemService.CreateItemAsync(inventoryId, dto, currentUser.UserId, ct);

        return Created($"/api/items/{id}", id);
    }

    [HttpGet("{inventoryId:guid}/items")]
    public async Task<ActionResult<List<ItemTableDto>>> GetItems(Guid inventoryId, [FromQuery] ItemFilterDto filter, CancellationToken ct)
    {
        var result = await itemService.GetByInventoryAsync(inventoryId, filter, ct);

        return Ok(result);
    }
}