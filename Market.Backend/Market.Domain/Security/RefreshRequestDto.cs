namespace Market.Domain.Security;

/// <summary>
/// Zahtjev za osvježavanje JWT tokena pomoću refresh tokena.
/// </summary>
public sealed class RefreshRequestDto
{
    /// <summary>
    /// Refresh token koji je prethodno izdat korisniku.
    /// </summary>
    public required string RefreshToken { get; set; }

    /// <summary>
    /// Klijentski otisak (optional) — koristi se za dodatnu provjeru da osvježavanje dolazi s istog uređaja.
    /// </summary>
    public string? Fingerprint { get; set; }
}
