namespace Market.Application.Features.Auth.Commands.Login;

/// <summary>
/// Obrada prijave: validira korisnika, verifikuje lozinku,
/// izdaje access/refresh token, upisuje refresh hash u bazu.
/// </summary>
public sealed class LoginCommandHandler
    : IRequestHandler<LoginCommand, LoginCommandDto>
{
    private readonly IAppDbContext _ctx;
    private readonly IJwtTokenService _tokens;
    private readonly IPasswordHasher<UserEntity> _hasher;
    private readonly TimeProvider _time;

    public LoginCommandHandler(
        IAppDbContext ctx,
        IJwtTokenService tokens,
        IPasswordHasher<UserEntity> hasher,
        TimeProvider timeProvider // registruj: services.AddSingleton<TimeProvider>(TimeProvider.System);
    )
    {
        _ctx = ctx;
        _tokens = tokens;
        _hasher = hasher;
        _time = timeProvider;
    }

    public async Task<LoginCommandDto> Handle(LoginCommand request, CancellationToken ct)
    {
        // Normalizuj email za pretragu (po potrebi trim/lower)
        var email = request.Email.Trim();

        var user = await _ctx.Users
            .FirstOrDefaultAsync(x =>
                x.Email == email &&
                x.IsEnabled &&
                !x.IsDeleted, ct);

        if (user is null)
            throw new MarketNotFoundException("Korisnik nije pronađen ili je onemogućen.");

        // Verifikacija lozinke
        var vr = _hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (vr == PasswordVerificationResult.Failed)
            throw new MarketConflictException("Pogrešni kredencijali.");

        // Izdaj par (access + refresh)
        var pair = _tokens.Issue(user);

        // Persistiraj refresh token u bazu (samo HASH!)
        var nowUtc = _time.GetUtcNow().UtcDateTime;

        var refreshEntity = new RefreshTokenEntity
        {
            TokenHash = _tokens.HashRefreshToken(pair.RefreshToken),
            ExpiresAtUtc = nowUtc.AddDays(14), // ili čitaj iz options (npr. JwtOptions.RefreshTokenDays)
            UserId = user.Id,
            Fingerprint = request.Fingerprint
        };

        _ctx.RefreshTokens.Add(refreshEntity);
        await _ctx.SaveChangesAsync(ct);

        return pair;
    }
}
