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
        
        if (await ctx.Users.AnyAsync(x => x.Email == request.Email || x.Username == request.Username, ct))
            throw new Exception("Korisnik s ovim podacima već postoji.");

        
        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
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