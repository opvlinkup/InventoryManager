using System.ComponentModel.DataAnnotations;

namespace InventoryManager.Domain.Models;

public class Inventory
{
    public Guid Id { get; set; }

    public Guid OwnerId { get; set; }
    public User Owner { get; set; } = null!;
    
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = null!;

    [MaxLength(4000)]
    public string? Description { get; set; } = null!;

    [MaxLength(2048)]
    public string? ImageUrl { get; set; }

    public bool IsPublic { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    
    [Required]
    public WriteAccessMode WriteAccessMode { get; set; } = WriteAccessMode.Restricted;
    
    [Timestamp]
    public byte[] RowVersion { get; set; } = null!;

    public ICollection<Item> Items { get; set; } = new List<Item>();
    public ICollection<InventoryWriteAccess> InventoryWriteAccesses { get; set; } = new List<InventoryWriteAccess>();
    public ICollection<FieldMetadata> FieldDefinitions { get; set; } = new List<FieldMetadata>();
    public ICollection<CustomIdPart> CustomIdParts { get; set; } = new List<CustomIdPart>();
    public ICollection<InventoryTag> Tags { get; set; } = new List<InventoryTag>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
