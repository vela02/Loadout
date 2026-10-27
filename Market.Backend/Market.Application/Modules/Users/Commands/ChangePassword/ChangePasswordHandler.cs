using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Market.Application.Abstractions;
using Market.Domain.Models;

namespace Market.Application.Modules.Users.Commands.ChangePassword;

public sealed class ChangePasswordHandler(
    IAppDbContext ctx,
    IPasswordHasher<User> hasher,
    IAppCurrentUser currentUser,
    IJwtTokenService jwt,
    TimeProvider timeProvider)
    : IRequestHandler<ChangePasswordCommand, bool>
{
    public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId;
        if (userId is null)
            throw new Exception("Korisnik nije prijavljen.");

        var user = await ctx.Users.FirstOrDefaultAsync(u => u.Id == userId, ct)
            ?? throw new Exception("Korisnik nije pronađen.");

        // check old password
        var result = hasher.VerifyHashedPassword(user, user.PasswordHash, request.OldPassword);
        if (result == PasswordVerificationResult.Failed)
            throw new Exception("Stara lozinka nije ispravna.");

        //  set new hashed password
        user.PasswordHash = hasher.HashPassword(user, request.NewPassword);

        // log out all other sessions (revoke their refresh tokens, keep the current one)
        var currentHash = string.IsNullOrWhiteSpace(request.CurrentRefreshToken)
            ? null
            : jwt.HashRefreshToken(request.CurrentRefreshToken);

        var otherSessions = await ctx.RefreshTokens
            .Where(x => x.UserId == user.Id &&
                        !x.IsRevoked &&
                        !x.IsDeleted &&
                        x.TokenHash != currentHash)
            .ToListAsync(ct);

        var nowUtc = timeProvider.GetUtcNow().UtcDateTime;
        foreach (var rt in otherSessions)
        {
            rt.IsRevoked = true;
            rt.RevokedAtUtc = nowUtc;
        }

        await ctx.SaveChangesAsync(ct);
        return true;
    }
}
