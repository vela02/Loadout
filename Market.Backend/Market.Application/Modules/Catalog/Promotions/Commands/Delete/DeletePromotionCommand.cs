namespace Market.Application.Modules.Catalog.Promotions.Commands.Delete;

public sealed class DeletePromotionCommand : IRequest<Unit>
{
    public int Id { get; set; }
}
