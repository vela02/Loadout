using MediatR;
using Microsoft.EntityFrameworkCore;
using Market.Application.Abstractions;

namespace Market.Application.Modules.Users.Queries.GetProfile;

public sealed class GetProfileHandler(IAppDbContext ctx,IAppCurrentUser currentUser) : IRequestHandler<GetProfileQuery, GetProfileDto>
{
    public async Task<GetProfileDto> Handle(GetProfileQuery request, CancellationToken ct)
    {
        //if (currentUser.UserId is null)
        //{
        //    throw new Exception("Korisnik nije prijavljen.");
        //}
        return await ctx.Users
            .Where(u => u.Id == currentUser.UserId)
            .Select(u => new GetProfileDto
            {         
                Username = u.Username,
                Email = u.Email,
                CreatedAt = u.CreatedAt
            })
            .FirstOrDefaultAsync(ct)
            ?? throw new Exception("Korisnik nije pronađen.");
    }
}