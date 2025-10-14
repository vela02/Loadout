namespace Market.Application.Features.Auth.Commands.Login;

/// <summary>
/// Command za prijavu korisnika i izdavanje access/refresh token para.
/// </summary>
public sealed class LoginCommand : IRequest<LoginCommandDto>
{
    /// <summary>
    /// Email korisnika.
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    /// Lozinka korisnika.
    /// </summary>
    public required string Password { get; init; }

    /// <summary>
    /// (optional) Klijentski “fingerprint”/otisak uređaja za device-bound refresh tokene. (optional)
    /// </summary>
    public string? Fingerprint { get; init; }
}
