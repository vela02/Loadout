// RefreshTokenEntity.cs

// RefreshTokenEntity.cs
using Market.Domain.Common;

namespace Market.Domain.Entities.Identity;

public sealed class RefreshTokenEntity : BaseEntity
{
    public required string TokenHash { get; set; }       // čuvamo HASH, ne čisti token
    public DateTime ExpiresAtUtc { get; set; }
    public bool IsRevoked { get; set; }
    public int UserId { get; set; }
    public MarketUserEntity User { get; set; } = default!;
    public string? Fingerprint { get; set; }             // (optional) npr. UA/IP hash
    public DateTime? RevokedAtUtc { get; set; }
}