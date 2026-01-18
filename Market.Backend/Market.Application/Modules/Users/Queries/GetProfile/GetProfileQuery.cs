using MediatR;
namespace Market.Application.Modules.Users.Queries.GetProfile;

public record GetProfileQuery() : IRequest<GetProfileDto>;