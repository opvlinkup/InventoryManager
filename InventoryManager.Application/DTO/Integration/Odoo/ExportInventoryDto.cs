namespace InventoryManager.Application.DTO.Integration.Odoo;

public sealed class ExportInventoryDto
{
    public string Title { get; set; } = null!;
    public List<ExportFieldStatisticsDto> FieldStatistics { get; set; } = new();
}