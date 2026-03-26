namespace InventoryManager.Application.DTO.Integration.Odoo;

public sealed class ExternalFieldStatisticsDto
{
    public string FieldName { get; set; } = null!;
    public string FieldType { get; set; } = null!;

    public decimal? Min { get; set; }
    public decimal? Max { get; set; }
    public decimal? Average { get; set; }

    public List<ValueFrequencyDto>? TopValues { get; set; }
}