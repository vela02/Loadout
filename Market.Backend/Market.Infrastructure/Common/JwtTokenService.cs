using Market.Application.Abstractions;
using Market.Shared;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Market.Infrastructure.Identity;

public sealed class JwtTokenService(IOptions<JwtOptions> options) : IJwtTokenService
{
    private readonly JwtOptions _o = options.Value;

    public JwtTokenPair IssueTokens(MarketUserEntity user)
    {
        var now = DateTimeOffset.UtcNow;
        var accessExpires = now.AddMinutes(_o.AccessTokenMinutes);
        var refreshExpires = now.AddDays(_o.RefreshTokenDays);

        // --- claimovi ---
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new("is_admin", user.IsAdmin.ToString().ToLowerInvariant()),
            new("is_manager", user.IsManager.ToString().ToLowerInvariant()),
            new("is_employee", user.IsEmployee.ToString().ToLowerInvariant()),
            new("ver", user.TokenVersion.ToString()),
            new(JwtRegisteredClaimNames.Iat, now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_o.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(
            issuer: _o.Issuer,
            audience: _o.Audience,
            claims: claims,
            notBefore: now.UtcDateTime,
            expires: accessExpires.UtcDateTime,
            signingCredentials: creds);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);

        // --- refresh token ---
        var refreshRaw = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var refreshHash = HashRefreshToken(refreshRaw);

        return new JwtTokenPair
        {
            AccessToken = accessToken,
            AccessTokenExpiresAtUtc = accessExpires.UtcDateTime,
            RefreshTokenRaw = refreshRaw,
            RefreshTokenHash = refreshHash,
            RefreshTokenExpiresAtUtc = refreshExpires.UtcDateTime 
        };
    }

    public string HashRefreshToken(string rawToken)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(rawToken));
        return Convert.ToBase64String(bytes);
    }
}
