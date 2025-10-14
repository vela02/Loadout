namespace Market.Application.Abstractions;

/// <summary>
/// Predstavlja trenutno prijavljenog korisnika u sistemu.
/// </summary>
public interface IAppCurrentUser
{
    /// <summary>
    /// Identifikator korisnika (UserId).
    /// </summary>
    int? UserId { get; }

    /// <summary>
    /// Email korisnika. (optional)
    /// </summary>
    string? Email { get; }

    /// <summary>
    /// Označava da li je korisnik prijavljen.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Da li je korisnik administrator.
    /// </summary>
    bool IsAdmin { get; }

    /// <summary>
    /// Da li je korisnik menadžer.
    /// </summary>
    bool IsManager { get; }

    /// <summary>
    /// Da li je korisnik obični zaposlenik.
    /// </summary>
    bool IsEmployee { get; }
}
