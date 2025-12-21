using MediatR;
using Microsoft.EntityFrameworkCore;
using Market.Application.Abstractions;

namespace Market.Application.Modules.Users.Queries.GetProfile;

public sealed class GetProfileHandler(IAppDbContext ctx) : IRequestHandler<GetProfileQuery, GetProfileDto>
{
    public async Task<GetProfileDto> Handle(GetProfileQuery request, CancellationToken ct)
    {
        return await ctx.Users
            .Where(u => u.Id == request.UserId)
            .Select(u => new GetProfileDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                CreatedAt = u.CreatedAt
            })
            .FirstOrDefaultAsync(ct)
            ?? throw new Exception("Korisnik nije pronađen.");
    }
}