// MarketUserEntity.cs
using Market.Domain.Common;

namespace Market.Domain.Entities.Identity;

public sealed class MarketUserEntity : BaseEntity
{
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsManager { get; set; }
    public bool IsEmployee { get; set; }
    public int TokenVersion { get; set; } = 0; // za globalnu revokaciju
    public bool IsEnabled { get; set; }
    public ICollection<RefreshTokenEntity> RefreshTokens { get; private set; } = new List<RefreshTokenEntity>();
}