namespace Market.Application.Modules.Catalog.Promotions.Queries.GetOptions;

public sealed class GetPromotionOptionsQueryDto
{
    public List<PromotionOptionDto> Categories { get; init; } = new();
    public List<PromotionOptionDto> Tags { get; init; } = new();
    public List<string> Genres { get; init; } = new();
    public List<string> Publishers { get; init; } = new();
    public List<string> Developers { get; init; } = new();
    public List<PromotionGameDto> Games { get; init; } = new();
}

public sealed class PromotionOptionDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
}

public sealed class PromotionGameDto
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int? CategoryId { get; init; }
    public string? Genre { get; init; }
    public string? Publisher { get; init; }
    public string? Developer { get; init; }
    public List<int> TagIds { get; init; } = new();
}
