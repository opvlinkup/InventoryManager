namespace InventoryManager.Application.DTO.Integration.Odoo;

public sealed class ExternalInventoryDto
{
    public string Title { get; set; } = null!;
    List<ExternalItemDto> Items { get; set; } = null!;  
}