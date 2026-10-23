namespace Market.Application.Modules.Catalog.Promotions.Queries.List;

public sealed class ListPromotionsQueryDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public decimal DiscountPercentage { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public string TargetType { get; init; } = string.Empty;
    public string? TargetValue { get; init; }
    public int GameCount { get; init; }
    public DateTime CreatedAt { get; init; }
}
