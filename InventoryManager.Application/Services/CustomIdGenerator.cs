using System.Text;
using InventoryManager.Application.Abstractions.Inventory.Items;
using InventoryManager.Application.Abstractions.Persistence.UnitOfWork;
using InventoryManager.Domain.Models;

namespace InventoryManager.Application.Services;

public class CustomIdGenerator(IUnitOfWork unitOfWork) : ICustomIdGenerator
{
    public async Task<(string customId, long? sequence)> GenerateAsync(Guid inventoryId, CancellationToken ct)
    {
        var customIdParts = await unitOfWork.CustomIdPartRepository
            .GetManyByAsync(
                p => p.InventoryId == inventoryId,
                q => q.OrderBy(p => p.Order),
                null,
                true,
                ct);

        long? sequence = null;

        var sb = new StringBuilder();

        foreach (var part in customIdParts)
        {
            switch (part.Type)
            {
                case CustomIdPartType.FixedText:
                    sb.Append(part.FixedValue ?? "");
                    break;

                case CustomIdPartType.Guid:
                    sb.Append(Guid.NewGuid().ToString());
                    break;

                case CustomIdPartType.Random6Digit:
                    sb.Append(Random.Shared.Next(0, 1_000_000).ToString("D6"));
                    break;

                case CustomIdPartType.Random9Digit:
                    sb.Append(Random.Shared.Next(0, 1_000_000_000).ToString("D9"));
                    break;

                case CustomIdPartType.Random32Bit:
                    sb.Append(Random.Shared.NextInt64(0, uint.MaxValue));
                    break;

                case CustomIdPartType.Random20Bit:
                    sb.Append(Random.Shared.Next(0, 1 << 20));
                    break;

                case CustomIdPartType.DateTime:
                    sb.Append(DateTime.UtcNow.ToString(part.Format ?? "yyyyMMdd"));
                    break;

                case CustomIdPartType.Sequence:

                    sequence ??= await unitOfWork.ItemRepository.GetNextSequenceAsync(inventoryId, ct);

                    sb.Append(sequence.Value.ToString(part.Format ?? "D"));
                    break;
            }
        }

        return (sb.ToString(), sequence);
    }
}