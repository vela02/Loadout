using Market.Application.Features.Auth.Commands.Login;

namespace Market.Application.Abstractions
{
    public interface IJwtTokenService
    {
        LoginCommandDto Issue(UserEntity user);
        string HashRefreshToken(string rawToken);
        (string Raw, string Hash, DateTime ExpiresUtc) IssueRefreshToken();
    }
}
