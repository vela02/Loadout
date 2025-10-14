namespace Market.Application.Features.Auth.Commands.Refresh;

/// <summary>
/// DTO koji se vraća nakon uspješnog osvježavanja (rotacije) tokena.
/// </summary>
public sealed class RefreshTokenCommandDto
{
    /// <summary>
    /// Novi access token koji klijent treba koristiti za autentifikaciju.
    /// </summary>
    public required string AccessToken { get; init; }

    /// <summary>
    /// Novi refresh token koji zamjenjuje prethodni.
    /// </summary>
    public required string RefreshToken { get; init; }

    /// <summary>
    /// Datum isteka access tokena u UTC formatu.
    /// </summary>
    public required DateTime AccessTokenExpiresAtUtc { get; init; }

    /// <summary>
    /// Datum isteka refresh tokena u UTC formatu.
    /// </summary>
    public required DateTime RefreshTokenExpiresAtUtc { get; init; }
}
