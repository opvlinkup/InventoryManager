using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManager.Domain.Models;

[Table("Sessions")]
public class Session
{
    [Key] public Guid Id { get; set; }

    [Required] public Guid UserId { get; set; }

    [ForeignKey(nameof(UserId))] public User User { get; set; } = null!;

    [Required] public byte[] RefreshTokenHash { get; set; } = null!;

    [MaxLength(100)] public string? IpAddress { get; set; }

    [MaxLength(250)] public string? UserAgent { get; set; }

    public string? DeviceFingerprint { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastUsedAt { get; set; }

    [Required] public DateTime ExpiresAt { get; set; }

    public bool IsRevoked { get; set; }
    public DateTime? RevokedAt { get; set; }

    [MaxLength(100)] public string? RevokedByIp { get; set; }

    public byte[]? ReplacedByTokenHash { get; set; }
}