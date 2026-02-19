using System.ComponentModel.DataAnnotations;

namespace InventoryManager.Domain.Models;

public class Item
{
    public Guid Id { get; set; }

    public Guid InventoryId { get; set; }
    public Inventory Inventory { get; set; } = null!;

    [Required]
    [MaxLength(200)]
    public string CustomId { get; set; } = null!;

    public Guid CreatedById { get; set; }
    public User CreatedBy { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(300)]
    public string? Text1 { get; set; }
    [MaxLength(300)]
    public string? Text2 { get; set; }
    [MaxLength(300)]
    public string? Text3 { get; set; }
    
    [MaxLength(3000)]
    public string? LongText1 { get; set; }
    [MaxLength(3000)]
    public string? LongText2 { get; set; }
    [MaxLength(3000)]
    public string? LongText3 { get; set; }
    
    
    public decimal? Number1 { get; set; }
    public decimal? Number2 { get; set; }
    public decimal? Number3 { get; set; }
    
    public bool? Bool1 { get; set; }
    public bool? Bool2 { get; set; }
    public bool? Bool3 { get; set; }
    
    [MaxLength(2048)]
    public string? Link1 { get; set; }
    [MaxLength(2048)]
    public string? Link2 { get; set; }
    [MaxLength(2048)]
    public string? Link3 { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; } = null!;

    public ICollection<Like> Likes { get; set; } = new List<Like>();
}
