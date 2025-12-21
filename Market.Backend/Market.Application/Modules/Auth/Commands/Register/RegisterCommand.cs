using MediatR;

namespace Market.Application.Modules.Auth.Commands.Register;

public record RegisterCommand(
    string Username,
    string Email,
    string Password
) : IRequest<RegisterCommandDto>;