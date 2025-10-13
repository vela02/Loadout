using Market.Core.Entities.Identity;

namespace Market.Core.Security
{
    public interface IJwtTokenService
    {
        TokenPairDto Issue(UserEntity user);
        string HashRefreshToken(string rawToken);
        (string Raw, string Hash, DateTime ExpiresUtc) IssueRefreshToken();
    }
}
