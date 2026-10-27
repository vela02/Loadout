using MediatR;
namespace Market.Application.Modules.Users.Commands.ChangePassword;

public record ChangePasswordCommand(
    string OldPassword,
    string NewPassword,
    // Refresh token of the current session - it stays valid, all other sessions are logged out
    string? CurrentRefreshToken = null
) : IRequest<bool>;
