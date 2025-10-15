namespace Market.Application.Modules.Auth.Commands.Refresh;

/// <summary>
/// Zahtjev za rotaciju refresh tokena i izdavanje novog para tokena.
/// </summary>
public sealed class RefreshTokenCommand : IRequest<RefreshTokenCommandDto>
{
    /// <summary>
    /// Refresh token koji klijent šalje za rotaciju.
    /// </summary>
    public required string RefreshToken { get; init; }

    /// <summary>
    /// (optional) Klijentski “fingerprint”/otisak uređaja za device-bound tokene. (optional)
    /// </summary>
    public string? Fingerprint { get; init; }
}


