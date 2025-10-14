namespace Market.Application.Features.Auth.Commands.Refresh;

/// <summary>
/// Rezultat rotacije: novi Access i Refresh token.
/// </summary>
public sealed record RefreshTokenCommandDto(string AccessToken, string RefreshToken);