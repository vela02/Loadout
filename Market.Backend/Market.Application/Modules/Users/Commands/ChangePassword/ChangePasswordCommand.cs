using MediatR;
namespace Market.Application.Modules.Users.Commands.ChangePassword;

public record ChangePasswordCommand(
    string OldPassword,
    string NewPassword
) : IRequest<bool>;