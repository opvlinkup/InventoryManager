using System.Security.Claims;
using InventoryManager.Application.Abstractions.Identity;
using InventoryManager.Application.Abstractions.Integration;
using InventoryManager.Application.DTO.Integration.Salesforce;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManager.API;

[ApiController]
[Route("api/users/salesforce-profile")]
[Authorize] 
public class SalesforceController(ISalesforceService salesforceService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateProfile([FromBody] SalesforceCustomerDto request, ICurrentUserService currentUserService, CancellationToken ct)
    {
        
        try
        {
            var contactId = await salesforceService.CreateCustomerAsync(request, ct);
            return Ok(new { Message = "CRM Profile created successfully", ContactId = contactId });
        }
        catch (HttpRequestException ex)
        {

            return BadRequest(new { Error = ex.Message });
        }
    }
}