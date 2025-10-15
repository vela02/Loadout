namespace Market.Application.Modules.Auth.Commands.Refresh;

public sealed class RefreshTokenCommandHandler(
    IAppDbContext ctx,
    IJwtTokenService jwt,
    TimeProvider timeProvider)
    : IRequestHandler<RefreshTokenCommand, RefreshTokenCommandDto>
{
    public async Task<RefreshTokenCommandDto> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        // 1) Hash primljenog refresh tokena
        var incomingHash = jwt.HashRefreshToken(request.RefreshToken);

        // 2) Pronađi važeći refresh token u bazi (TRACKING jer ga mijenjamo)
        var rt = await ctx.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x =>
                x.TokenHash == incomingHash &&
                !x.IsRevoked &&
                !x.IsDeleted, ct);

        var nowUtc = timeProvider.GetUtcNow().UtcDateTime;

        if (rt is null || rt.ExpiresAtUtc <= nowUtc)
            throw new MarketConflictException("Refresh token je nevažeći ili je istekao.");

        // (optional) Fingerprint provjera
        if (rt.Fingerprint is not null &&
            request.Fingerprint is not null &&
            rt.Fingerprint != request.Fingerprint)
        {
            throw new MarketConflictException("Neispravan klijentski otisak.");
        }

        var user = rt.User;
        if (user is null || !user.IsEnabled || user.IsDeleted)
            throw new MarketConflictException("Korisnički nalog je nevažeći.");

        // 3) Rotacija: revoke stari
        rt.IsRevoked = true;
        rt.RevokedAtUtc = nowUtc;

        // 4) Izdaj NOVI par (access + refresh) – servis vraća i RAW i HASH i isteke
        var pair = jwt.IssueTokens(user);

        // 5) Upis NOVOG refresh tokena (samo HASH u bazu)
        var newRt = new RefreshTokenEntity
        {
            TokenHash = pair.RefreshTokenHash,               // ✅ koristimo hash iz servisa
            ExpiresAtUtc = pair.RefreshTokenExpiresAtUtc,    // ✅ bez hard-codiranja
            UserId = user.Id,
            Fingerprint = request.Fingerprint,
        };

        ctx.RefreshTokens.Add(newRt);
        await ctx.SaveChangesAsync(ct);

        // 6) Vraćamo klijentu RAW refresh token i access token
        return new RefreshTokenCommandDto
        {
            AccessToken = pair.AccessToken,
            RefreshToken = pair.RefreshTokenRaw,
            AccessTokenExpiresAtUtc = pair.AccessTokenExpiresAtUtc,
            RefreshTokenExpiresAtUtc = pair.RefreshTokenExpiresAtUtc
        };

    }
}
