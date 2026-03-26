using InventoryManager.Application.Abstractions.Integration;
using InventoryManager.Application.DTO.Integration.Odoo;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManager.Controllers;

[ApiController]
[Route("api/odoo")]
public sealed class OdooController(IOdooService service) : ControllerBase
{
    [HttpPost("import")]
    public async Task<ActionResult<ExternalInventoryDto>> Import([FromBody] OdooRequestDto dto, CancellationToken ct)
    {
        try
        {
            var result = await service.GetAggregatedDataAsync(dto.Token, ct);
            return Ok(result);
        }
        catch (InvalidOperationException)
        {
            return Unauthorized();
        }
    }
}