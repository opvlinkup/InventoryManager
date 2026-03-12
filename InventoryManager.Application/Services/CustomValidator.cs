using System.Text;
using System.Text.RegularExpressions;
using InventoryManager.Application.Abstractions.Persistence.UnitOfWork;
using InventoryManager.Application.Abstractions.Validators;
using InventoryManager.Domain.Models;

namespace InventoryManager.Application.Services;


public sealed class CustomIdValidator(IUnitOfWork unitOfWork) : ICustomIdValidator
{
    public async Task<long?> ValidateAsync(
        Guid inventoryId,
        string customId,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(customId))
            throw new InvalidOperationException("Custom ID cannot be empty.");

        var customIdParts = await unitOfWork.CustomIdPartRepository
            .GetManyByAsync(
                p => p.InventoryId == inventoryId,
                q => q.OrderBy(p => p.Order),
                null,
                true,
                ct);

        if (!customIdParts.Any())
            throw new InvalidOperationException("Custom ID format is not configured.");

        var regex = BuildRegex(customIdParts);

        var match = regex.Match(customId);

        if (!match.Success)
            throw new InvalidOperationException("Custom ID does not match the inventory format.");

        long? sequence = null;

        if (match.Groups["sequence"].Success)
        {
            sequence = long.Parse(match.Groups["sequence"].Value);
        }

        return sequence;
    }

    private static Regex BuildRegex(IEnumerable<CustomIdPart> parts)
    {
        var sb = new StringBuilder();

        sb.Append("^");

        foreach (var part in parts)
        {
            switch (part.Type)
            {
                case CustomIdPartType.FixedText:

                    sb.Append(Regex.Escape(part.FixedValue ?? ""));
                    break;

                case CustomIdPartType.Guid:

                    sb.Append(@"[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}");
                    break;

                case CustomIdPartType.Random6Digit:

                    sb.Append(@"\d{6}");
                    break;

                case CustomIdPartType.Random9Digit:

                    sb.Append(@"\d{9}");
                    break;

                case CustomIdPartType.Random20Bit:
                    
                    sb.Append(@"\d{1,7}");
                    break;

                case CustomIdPartType.Random32Bit:
                    
                    sb.Append(@"\d{1,10}");
                    break;

                case CustomIdPartType.DateTime:

                    sb.Append(BuildDateTimeRegex(part.Format));
                    break;

                case CustomIdPartType.Sequence:

                    var digits = ExtractDigits(part.Format);

                    if (digits.HasValue)
                        sb.Append($@"(?<sequence>\d{{{digits.Value}}})");
                    else
                        sb.Append(@"(?<sequence>\d+)");

                    break;
            }
        }

        sb.Append("$");

        return new Regex(sb.ToString(), RegexOptions.Compiled);
    }

    private static string BuildDateTimeRegex(string? format)
    {
        format ??= "yyyyMMdd";

        var pattern = format;

        pattern = pattern.Replace("yyyy", @"\d{4}");
        pattern = pattern.Replace("MM", @"\d{2}");
        pattern = pattern.Replace("dd", @"\d{2}");
        pattern = pattern.Replace("HH", @"\d{2}");
        pattern = pattern.Replace("mm", @"\d{2}");
        pattern = pattern.Replace("ss", @"\d{2}");

        return pattern;
    }

    private static int? ExtractDigits(string? format)
    {
        if (string.IsNullOrWhiteSpace(format))
            return null;

        if (!format.StartsWith("D"))
            return null;

        if (int.TryParse(format.Substring(1), out var digits))
            return digits;

        return null;
    }
}