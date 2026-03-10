using InventoryManager.Domain.Models;

namespace InventoryManager.Application.DTO.Admin;

public sealed class UserAdminDto
{
    public Guid Id { get; init; }

    public string Email { get; init; } = null!;

    public string? Name { get; init; }

    public string? Surname { get; init; }

    public Status Status { get; init; }

    public bool IsAdmin { get; init; }

    public DateTime RegisteredAt { get; init; }
}