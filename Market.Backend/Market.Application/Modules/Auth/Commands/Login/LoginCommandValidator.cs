namespace Market.Application.Modules.Auth.Commands.Login;

/// <summary>
/// FluentValidation validator za <see cref="LoginCommand"/>.
/// </summary>
public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email je obavezan.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Lozinka je obavezna.")
            .MinimumLength(5).WithMessage("Lozinka mora imati najmanje 6 znakova.");

        // Fingerprint je opcionalan, ali ako dođe, možeš ograničiti dužinu
        RuleFor(x => x.Fingerprint)
            .MaximumLength(256).WithMessage("Fingerprint može imati najviše 256 znakova.")
            .When(x => x.Fingerprint is not null);
    }
}
