namespace InventoryManager.Application.DTO.Integration.Odoo;

public sealed class ExternalInventoryDto
{
    public string Title { get; set; } = null!;
    public List<ExternalItemDto> Items { get; set; } = null!;  
    public List<ExternalFieldStatisticsDto> Stats { get; set; } = new();
}