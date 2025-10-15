namespace Market.Application.Modules.Auth.Commands.Login;

/// <summary>
/// Predstavlja par tokena (access + refresh) koji klijent dobija prilikom prijave ili osvježavanja.
/// </summary>
public sealed class LoginCommandDto
{
    /// <summary>
    /// JWT access token – koristi se za autorizovane API pozive.
    /// </summary>
    public required string AccessToken { get; set; }

    /// <summary>
    /// Osvježavajući token (refresh token) koji klijent čuva lokalno i koristi za dobijanje novog access tokena.
    /// </summary>
    public required string RefreshToken { get; set; }

    /// <summary>
    /// Vrijeme isteka access tokena u UTC formatu.
    /// </summary>
    public required DateTime ExpiresAtUtc { get; set; }
}