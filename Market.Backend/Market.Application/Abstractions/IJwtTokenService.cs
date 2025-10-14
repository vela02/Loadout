using Market.Application.Features.Auth.Commands.Login;

namespace Market.Application.Abstractions
{
    public sealed class JwtTokenPair
    {
        public required string AccessToken { get; init; }
        public required DateTime AccessTokenExpiresAtUtc { get; init; }

        public required string RefreshTokenRaw { get; init; }
        public required string RefreshTokenHash { get; init; }
        public required DateTime RefreshTokenExpiresAtUtc { get; init; }
    }

    public interface IJwtTokenService
    {
        /// <summary>Izdaje access token i vraća sve tehničke podatke o njemu.</summary>
        JwtTokenPair IssueTokens(MarketUserEntity user);

        /// <summary>Hashira refresh token za pohranu u bazu.</summary>
        string HashRefreshToken(string rawToken);
    }
}
