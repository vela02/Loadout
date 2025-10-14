namespace Market.Application.Features.Auth.Commands.Logout;

/// <summary>
/// FluentValidation validator za <see cref="LogoutCommand"/>.
/// </summary>
public sealed class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token je obavezan.");
    }
}
