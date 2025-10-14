namespace Market.Domain.Security;

public sealed class JwtOptions
{
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required string Key { get; init; }
    public int AccessTokenMinutes { get; init; } = 20;
    public int RefreshTokenDays { get; init; } = 14;
}
