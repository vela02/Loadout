using Market.Domain.Models;

namespace Market.Application.Modules.Catalog.Promotions.Commands.Create;

public sealed class CreatePromotionCommandValidator : AbstractValidator<CreatePromotionCommand>
{
    public CreatePromotionCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Naziv promocije je obavezan.")
            .MaximumLength(150).WithMessage("Naziv promocije je predug.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Opis je predug.");

        RuleFor(x => x.DiscountPercentage)
            .InclusiveBetween(1, 100).WithMessage("Postotak popusta mora biti između 1 i 100.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Datum početka je obavezan.");

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .GreaterThan(x => x.StartDate).WithMessage("Datum završetka mora biti nakon datuma početka.");

        RuleFor(x => x.TargetType)
            .Must(t => PromotionTargetTypes.All.Contains(t))
            .WithMessage("Nepoznat tip promocije.");

        RuleFor(x => x.GameIds)
            .NotEmpty().WithMessage("Odaberite barem jednu igru.")
            .When(x => x.TargetType == PromotionTargetTypes.Games);

        RuleFor(x => x.TargetId)
            .NotNull().WithMessage("Odaberite kategoriju ili tag.")
            .When(x => x.TargetType is PromotionTargetTypes.Category or PromotionTargetTypes.Tag);

        RuleFor(x => x.TargetText)
            .NotEmpty().WithMessage("Odaberite žanr, izdavača ili developera.")
            .When(x => x.TargetType is PromotionTargetTypes.Genre
                or PromotionTargetTypes.Publisher
                or PromotionTargetTypes.Developer);
    }
}
