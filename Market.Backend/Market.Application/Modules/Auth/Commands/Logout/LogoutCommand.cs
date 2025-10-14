namespace Market.Application.Features.Auth.Commands.Logout;

/// <summary>
/// Command za odjavu korisnika i poništavanje (revokaciju) refresh tokena.
/// </summary>
public sealed class LogoutCommand : IRequest
{
    /// <summary>
    /// Refresh token koji se poništava.
    /// </summary>
    public required string RefreshToken { get; init; }
}
