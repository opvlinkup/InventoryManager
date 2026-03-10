using System.ComponentModel.DataAnnotations;

namespace InventoryManager.Domain.Models;

public class Item
{
    public Guid Id { get; set; }

    public Guid InventoryId { get; set; }
    public Inventory Inventory { get; set; } = null!;

    public string CustomId { get; set; } = null!;  // for UI

    public Guid CreatedById { get; set; }
    public User CreatedBy { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public string? Text1 { get; set; }
    public string? Text2 { get; set; }
    public string? Text3 { get; set; }

    public string? LongText1 { get; set; }
    public string? LongText2 { get; set; }
    public string? LongText3 { get; set; }

    public decimal? Number1 { get; set; }
    public decimal? Number2 { get; set; }
    public decimal? Number3 { get; set; }

    public bool? Bool1 { get; set; }
    public bool? Bool2 { get; set; }
    public bool? Bool3 { get; set; }

    public string? Link1 { get; set; }
    public string? Link2 { get; set; }
    public string? Link3 { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public ICollection<Like> Likes { get; set; } = new List<Like>();
}