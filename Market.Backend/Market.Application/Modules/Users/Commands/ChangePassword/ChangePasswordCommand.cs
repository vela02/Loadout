using MediatR;
namespace Market.Application.Modules.Users.Commands.ChangePassword;

public record ChangePasswordCommand(
    int UserId,
    string OldPassword,
    string NewPassword
) : IRequest<bool>;