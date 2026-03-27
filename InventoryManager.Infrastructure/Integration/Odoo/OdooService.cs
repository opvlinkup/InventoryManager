using InventoryManager.Application.Abstractions.Integration;
using InventoryManager.Application.Abstractions.Inventory.Fields;
using InventoryManager.Application.Abstractions.Persistence.UnitOfWork;
using InventoryManager.Application.DTO.Integration.Odoo;
using InventoryManager.Domain.Models;

namespace InventoryManager.Infrastructure.Integration.Odoo;


public sealed class OdooService(IUnitOfWork unitOfWork, IFieldMetadataService fieldMetadataService) : IOdooService
{
    public async Task<ExportInventoryDto> GetAggregatedDataAsync(string token, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token is required");

        var inventory = await unitOfWork.InventoryRepository
            .GetByAsync(i => i.ApiToken == token, ct);

        if (inventory is null)
            throw new InvalidOperationException("Invalid token");

        var items = await unitOfWork.ItemRepository
            .GetManyByAsync(i => i.InventoryId == inventory.Id, cancellationToken: ct);

        var fieldMetadatas = await unitOfWork.FieldMetadataRepository
            .GetManyByAsync(
                fieldMetadata => fieldMetadata.InventoryId == inventory.Id && fieldMetadata.State != FieldState.Disabled,
                orderBy: q => q.OrderBy(f => f.Order), cancellationToken: ct);

        var statistics = new List<ExportFieldStatisticsDto>();

        foreach (var field in fieldMetadatas)
        {
            var fieldStats = new ExportFieldStatisticsDto
            {
                FieldName = field.DisplayName,
                FieldType = field.Type.ToString()
            };

            switch (field.Type)
            {
                case FieldType.Number:
                {
                    var numbers = items
                        .Select(item => fieldMetadataService.GetItemFieldValue(item, field))
                        .OfType<FieldValue.NumberValue>()
                        .Where(v => v.Value.HasValue)
                        .Select(v => v.Value!.Value)
                        .ToList();

                    if (numbers.Count > 0)
                    {
                        fieldStats.Min = numbers.Min();
                        fieldStats.Max = numbers.Max();
                        fieldStats.Average = numbers.Average();
                    }

                    break;
                }

                case FieldType.Bool:
                {
                    var values = items
                        .Select(i => fieldMetadataService.GetItemFieldValue(i, field))
                        .OfType<FieldValue.BoolValue>()
                        .Where(v => v.Value.HasValue)
                        .Select(v => v.Value!.Value.ToString())
                        .ToList();

                    fieldStats.TopValues = GetTopValues(values);
                    break;
                }

                case FieldType.Text:
                case FieldType.LongText:
                case FieldType.Link:
                default:
                {
                    var values = items
                        .Select(i => fieldMetadataService.GetItemFieldValue(i, field))
                        .OfType<FieldValue.TextValue>()
                        .Where(v => !string.IsNullOrWhiteSpace(v.Value))
                        .Select(v => v.Value!.Trim())
                        .ToList();

                    fieldStats.TopValues = GetTopValues(values);
                    break;
                }
            }

            statistics.Add(fieldStats);
        }

        return new ExportInventoryDto
        {
            Title = inventory.Title,
            FieldStatistics = statistics
        };
    }

    private static List<ValueFrequencyDto> GetTopValues(List<string> values)
    {
        return values
            .GroupBy(v => v)
            .Select(g => new ValueFrequencyDto
            {
                Value = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .Take(5)
            .ToList();
    }
}