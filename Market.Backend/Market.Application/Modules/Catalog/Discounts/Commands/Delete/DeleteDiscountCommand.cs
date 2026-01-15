

namespace Market.Application.Modules.Catalog.Discounts.Commands.Delete
{
    public class DeleteDiscountCommand : IRequest<Unit>
    {
        public int Id { get; set; }
    }
}
