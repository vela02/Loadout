using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Market.Application.Abstractions;
using Market.Domain.Models;


namespace Market.Application.Modules.Auth.Commands.Register;

public sealed class RegisterCommandHandler(
    IAppDbContext ctx,               
    IPasswordHasher<User> hasher,
    IJwtTokenService jwt) : IRequestHandler<RegisterCommand, RegisterCommandDto>
{
    public async Task<RegisterCommandDto> Handle(RegisterCommand request, CancellationToken ct)
    {
        // login compares emails in lowercase, so store them the same way
        var email = request.Email.Trim().ToLowerInvariant();
        var username = request.Username.Trim();

        if (await ctx.Users.AnyAsync(x => x.Email.ToLower() == email || x.Username == username, ct))
            throw new MarketConflictException("Korisnik s ovim emailom ili korisničkim imenom već postoji.");


        var user = new User
        {
            Username = username,
            Email = email,
            CreatedAt = DateTime.Now,
            IsEnabled = true,
            IsDeleted = false,
            RoleId = 2
        };

        
        user.PasswordHash = hasher.HashPassword(user, request.Password);

        ctx.Users.Add(user);
        await ctx.SaveChangesAsync(ct);

        
        var tokens = jwt.IssueTokens(user); 

        return new RegisterCommandDto
        {
            Id = user.Id,
            Username = user.Username,
            Token = tokens.AccessToken
        };
    }
}