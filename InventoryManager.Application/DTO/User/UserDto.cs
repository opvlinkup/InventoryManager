using System.Text.Json.Serialization;
using InventoryManager.Domain.Models;

namespace InventoryManager.Application.DTO;

public class UserDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; } 
    public string? Email { get; set; }
    public DateTime? LastSeenAt { get; set; }
    public Status Status { get; set; } = Status.Unverified;
    public Language Language { get; set; } = Language.En;
    public Theme Theme { get; set; } = Theme.Dark;
}