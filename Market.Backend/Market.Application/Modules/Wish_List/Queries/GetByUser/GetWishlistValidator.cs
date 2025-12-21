using FluentValidation;

namespace Market.Application.Modules.Wish_List.Queries.GetByUser;

public class GetWishlistValidator : AbstractValidator<GetWishlistQuery>
{
    public GetWishlistValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
    }
}