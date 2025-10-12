// UserEntity.cs
using Market.Core.Entities.Base;

namespace Market.Core.Entities.Identity;

public sealed class UserEntity : BaseEntity
{
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public string Role { get; set; } = "User";
    public int TokenVersion { get; set; } = 0; // za globalnu revokaciju
    public bool IsEnabled { get; set; }
    public ICollection<RefreshTokenEntity> RefreshTokens { get; private set; } = new List<RefreshTokenEntity>();
}