using System;

namespace InventoryManager.Domain.Models;

public class Session
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    public byte[] RefreshTokenHash { get; set; } = null!;

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public string? DeviceFingerprint { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastUsedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public bool IsRevoked { get; set; }
    public DateTime? RevokedAt { get; set; }

    public string? RevokedByIp { get; set; }

    public byte[]? ReplacedByTokenHash { get; set; }
}
