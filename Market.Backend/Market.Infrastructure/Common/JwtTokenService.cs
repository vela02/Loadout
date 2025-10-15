using Market.Application.Abstractions;
using Market.Shared.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Market.Infrastructure.Common;

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions _jwt;
    private readonly TimeProvider _time;

    public JwtTokenService(IOptions<JwtOptions> options, TimeProvider time)
    {
        _jwt = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _time = time ?? throw new ArgumentNullException(nameof(time));
    }

    public JwtTokenPair IssueTokens(MarketUserEntity user)
    {
        // sada iz TimeProvider-a (konzistentno s ostatkom app-a)
        var nowInstant = _time.GetUtcNow();
        var nowUtc = nowInstant.UtcDateTime;
        var accessExpires = nowInstant.AddMinutes(_jwt.AccessTokenMinutes).UtcDateTime;
        var refreshExpires = nowInstant.AddDays(_jwt.RefreshTokenDays).UtcDateTime;

        // --- claimovi (dodani i jti/aud za standardnost) ---
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier,   user.Id.ToString()),
            new(ClaimTypes.Email,            user.Email),
            new("is_admin",    user.IsAdmin.ToString().ToLowerInvariant()),
            new("is_manager",  user.IsManager.ToString().ToLowerInvariant()),
            new("is_employee", user.IsEmployee.ToString().ToLowerInvariant()),
            new("ver",         user.TokenVersion.ToString()),
            new(JwtRegisteredClaimNames.Iat, ToUnixTimeSeconds(nowInstant).ToString(), ClaimValueTypes.Integer64),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
            new(JwtRegisteredClaimNames.Aud, _jwt.Audience)
        };

        // --- potpis ---
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        // --- access token (JWT) ---
        var jwt = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            notBefore: nowUtc,
            expires: accessExpires,
            signingCredentials: creds
        );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);

        // --- refresh token (raw + hash) ---
        var refreshRaw = GenerateRefreshTokenRaw(64); // base64url
        var refreshHash = HashRefreshToken(refreshRaw); // base64url hash

        return new JwtTokenPair
        {
            AccessToken = accessToken,
            AccessTokenExpiresAtUtc = accessExpires,
            RefreshTokenRaw = refreshRaw,
            RefreshTokenHash = refreshHash,
            RefreshTokenExpiresAtUtc = refreshExpires
        };
    }

    public string HashRefreshToken(string rawToken)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(rawToken));
        // koristimo Base64Url da izbjegnemo problematične znakove
        return Base64UrlEncoder.Encode(bytes);
    }

    private static string GenerateRefreshTokenRaw(int numBytes)
    {
        // Base64UrlEncoder iz Microsoft.IdentityModel.Tokens (bez + / =)
        var bytes = RandomNumberGenerator.GetBytes(numBytes);
        return Base64UrlEncoder.Encode(bytes);
    }

    private static long ToUnixTimeSeconds(DateTimeOffset dto) => dto.ToUnixTimeSeconds();
}
