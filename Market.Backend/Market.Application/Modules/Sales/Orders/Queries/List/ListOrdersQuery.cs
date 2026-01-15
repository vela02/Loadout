namespace Market.Application.Modules.Sales.Orders.Queries.List;

public sealed class ListOrdersQuery : BasePagedQuery<ListOrdersQueryDto>
{
    public string? Search { get; init; }
    public int? StatusId { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}
