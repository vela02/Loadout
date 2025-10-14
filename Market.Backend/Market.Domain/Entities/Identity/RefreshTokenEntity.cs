// RefreshTokenEntity.cs

// RefreshTokenEntity.cs
using Market.Domain.Entities.Base;

namespace Market.Domain.Entities.Identity;

public sealed class RefreshTokenEntity : BaseEntity
{
    public required string TokenHash { get; set; }       // čuvamo HASH, ne čisti token
    public DateTime ExpiresAtUtc { get; set; }
    public bool IsRevoked { get; set; }
    public int UserId { get; set; }
    public UserEntity User { get; set; } = default!;
    public string? Fingerprint { get; set; }             // (optional) npr. UA/IP hash
}