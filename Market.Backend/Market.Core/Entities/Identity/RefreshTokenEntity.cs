// RefreshTokenEntity.cs
using Market.Core.Entities.Base;

namespace Market.Core.Entities.Identity;

public sealed class RefreshTokenEntity : BaseEntity
{
    public required string TokenHash { get; set; }       // čuvamo HASH, ne čisti token
    public DateTime ExpiresAtUtc { get; set; }
    public bool IsRevoked { get; set; }
    public int UserId { get; set; }
    public UserEntity User { get; set; } = default!;
    public string? Fingerprint { get; set; }             // (optional) npr. UA/IP hash
}