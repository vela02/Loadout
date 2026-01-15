
namespace Market.Application.Modules.Catalog.Discounts.Commands.Create
{
    public class CreateDiscountCommandValidator : AbstractValidator<CreateDiscountCommand>
    {
        public CreateDiscountCommandValidator()
        {
            
            RuleFor(x => x.DiscountPercentage)
                .InclusiveBetween(1, 100).WithMessage("Postotak popusta mora biti između 1 i 100.");

            
            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Datum početka je obavezan.");

            RuleFor(x => x.EndDate)
                .NotEmpty()
                .GreaterThan(x=>x.StartDate).WithMessage("Datum završetka mora biti nakon datuma početka.");

            
            RuleFor(x => x.Description)
                .MaximumLength(200).WithMessage("Opis je predug.");

            // category or game must be provided
            RuleFor(x => x)
                .Must(x => x.GameId.HasValue || x.CategoryId.HasValue)
                .WithMessage("Morate odabrati ili igru ili kategoriju za popust.");
        }
    }
}
