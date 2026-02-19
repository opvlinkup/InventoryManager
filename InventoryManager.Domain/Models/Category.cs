using System.ComponentModel.DataAnnotations;

namespace InventoryManager.Domain.Models;

public class Category
{
    public Guid Id { get; set; }
    [Required]
    [MaxLength(120)]
    public string Name { get; set; } = null!;
}
