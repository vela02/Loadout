using MediatR;
namespace Market.Application.Modules.Users.Queries.GetProfile;

public record GetProfileQuery(int UserId) : IRequest<GetProfileDto>;