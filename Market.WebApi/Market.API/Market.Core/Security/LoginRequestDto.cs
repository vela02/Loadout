namespace Market.Core.Security;

/// <summary>
/// Zahtjev za prijavu korisnika (login) u sistem.
/// </summary>
public sealed class LoginRequestDto
{
    /// <summary>
    /// Email korisnika koji se prijavljuje.
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// Lozinka korisnika.
    /// </summary>
    public required string Password { get; set; }

    /// <summary>
    /// Klijentski otisak (optional) — može biti hash kombinacije User-Agent + IP adrese.
    /// Koristi se za prepoznavanje uređaja i dodatnu sigurnost refresh tokena.
    /// </summary>
    public string? Fingerprint { get; set; }
}
