using Market.Core.Entities.Identity;
using Market.Core.Security;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

public sealed class JwtTokenService(IOptions<JwtOptions> options) : IJwtTokenService
{
    private readonly JwtOptions _o = options.Value;

    public TokenPairDto Issue(UserEntity user)
    {
        var now = DateTime.UtcNow;
        var claims = new List<Claim>
        {
            new(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.Name, user.Email),
            new(ClaimTypes.Role, user.Role),
            new("ver", user.TokenVersion.ToString()) // za global revoke
        };

        var creds = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_o.Key)),
            SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(
            issuer: _o.Issuer,
            audience: _o.Audience,
            claims: claims,
            notBefore: now,
            expires: now.AddMinutes(_o.AccessTokenMinutes),
            signingCredentials: creds);

        var access = new JwtSecurityTokenHandler().WriteToken(jwt);
        var (raw, hash, exp) = IssueRefreshToken();

        return new TokenPairDto
        {
            AccessToken = access,
            RefreshToken = raw,
            ExpiresAtUtc = jwt.ValidTo
        };
    }

    public (string Raw, string Hash, DateTime ExpiresUtc) IssueRefreshToken()
    {
        var raw = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var expires = DateTime.UtcNow.AddDays(_o.RefreshTokenDays);
        return (raw, HashRefreshToken(raw), expires);
    }

    public string HashRefreshToken(string rawToken)
    {
        // stabilan hash (npr. SHA256) — za demo ok; u produkciji razmotri HMAC s server-secretom
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(rawToken));
        return Convert.ToHexString(bytes);
    }
}
