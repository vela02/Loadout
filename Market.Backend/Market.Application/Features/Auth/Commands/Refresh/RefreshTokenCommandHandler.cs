namespace Market.Application.Features.Auth.Commands.Refresh;

public sealed class RefreshTokenCommandHandler(
    IAppDbContext ctx,
    IJwtTokenService tokens,
    TimeProvider timeProvider)
        : IRequestHandler<RefreshTokenCommand, RefreshTokenCommandDto>
{
    public async Task<RefreshTokenCommandDto> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        // Hash primljenog refresh tokena
        var incomingHash = tokens.HashRefreshToken(request.RefreshToken);

        // Pronađi važeći refresh token u bazi
        var rt = await ctx.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x =>
                x.TokenHash == incomingHash &&
                !x.IsRevoked &&
                !x.IsDeleted, ct);

        var nowUtc = timeProvider.GetUtcNow().UtcDateTime;

        if (rt is null || rt.ExpiresAtUtc <= nowUtc)
            throw new MarketConflictException("Refresh token je nevažeći ili je istekao.");

        // (optional) Fingerprint provjera — ako su oba postavljena, moraju se poklapati
        if (rt.Fingerprint is not null &&
            request.Fingerprint is not null &&
            rt.Fingerprint != request.Fingerprint)
        {
            throw new MarketConflictException("Neispravan klijentski otisak.");
        }

        var user = rt.User;
        if (user is null || !user.IsEnabled || user.IsDeleted)
            throw new MarketConflictException("Korisnički nalog je nevažeći.");

        // Rotacija: revoke stari token
        rt.IsRevoked = true;

        // Izdaj novi par (access + refresh)
        var pair = tokens.Issue(user);

        // Upis novog refresh tokena (samo HASH u bazu)
        var newRt = new RefreshTokenEntity
        {
            TokenHash = tokens.HashRefreshToken(pair.RefreshToken),
            ExpiresAtUtc = nowUtc.AddDays(14), // ili iz opcija
            UserId = user.Id,
            Fingerprint = request.Fingerprint
        };

        ctx.RefreshTokens.Add(newRt);
        await ctx.SaveChangesAsync(ct);

        return new RefreshTokenCommandDto(pair.AccessToken, pair.RefreshToken);
    }
}
