using InventoryManager.Domain.Models;

namespace InventoryManager.Application.DTO.User;

public sealed class UserFilterDto
{
    public string? Search { get; init; }

    public Status? Status { get; init; }

    public bool? IsAdmin { get; init; }

    public int Skip { get; init; }

    public int Take { get; init; } = 50;
}