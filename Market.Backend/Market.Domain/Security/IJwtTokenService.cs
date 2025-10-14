using Market.Domain.Entities.Identity;

namespace Market.Domain.Security
{
    public interface IJwtTokenService
    {
        TokenPairDto Issue(UserEntity user);
        string HashRefreshToken(string rawToken);
        (string Raw, string Hash, DateTime ExpiresUtc) IssueRefreshToken();
    }
}
