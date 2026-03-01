using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace InventoryManager.Domain.Models;

public class User : IdentityUser<Guid>
{
    [Required]
    [MaxLength(120)]
    public string? Name { get; set; }
    [MaxLength(120)]
    public string? Surname { get; set; }
    
    public Status Status { get; set; } = Status.Unverified;

    public Language Language { get; set; } = Language.En;

    public Theme Theme { get; set; } = Theme.Dark;

    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

    public ICollection<Inventory> OwnedInventories { get; set; } = new List<Inventory>();
    public ICollection<InventoryWriteAccess> InventoryWriteAccesses { get; set; } = new List<InventoryWriteAccess>();
    public ICollection<Item> CreatedItems { get; set; } = new List<Item>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Like> Likes { get; set; } = new List<Like>();
    public ICollection<Session> Sessions { get; set; } = new List<Session>();
}