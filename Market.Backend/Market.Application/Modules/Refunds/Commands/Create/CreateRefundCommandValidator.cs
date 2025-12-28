namespace Market.Application.Modules.Refunds.Commands.Create;

public class CreateRefundCommandValidator : AbstractValidator<CreateRefundCommand>
{
    public CreateRefundCommandValidator()
    {
        RuleFor(x => x.Dto.OrderId)
            .GreaterThan(0).WithMessage("Invalid Order ID.");

        RuleFor(x => x.Dto.Reason)
            .NotEmpty().WithMessage("Reason is required.")
            .MinimumLength(10).WithMessage("Reason must be at least 10 characters long.")
            .MaximumLength(500).WithMessage("Reason cannot exceed 500 characters.");
    }
}