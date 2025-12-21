using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Market.Application.Abstractions;
using Market.Domain.Models;

namespace Market.Application.Modules.Users.Commands.ChangePassword;

public sealed class ChangePasswordHandler(IAppDbContext ctx, IPasswordHasher<User> hasher)
    : IRequestHandler<ChangePasswordCommand, bool>
{
    public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken ct)
    {
        var user = await ctx.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, ct)
            ?? throw new Exception("Korisnik nije pronađen.");

        // check old password
        var result = hasher.VerifyHashedPassword(user, user.PasswordHash, request.OldPassword);
        if (result == PasswordVerificationResult.Failed)
            throw new Exception("Stara lozinka nije ispravna.");

        //  set new hashed password
        user.PasswordHash = hasher.HashPassword(user, request.NewPassword);

        await ctx.SaveChangesAsync(ct);
        return true;
    }
}