namespace Market.Application.Modules.Catalog.Promotions.Commands.Create;

/// <summary>
/// Creates a promotion and a discount for every game that matches the target at this moment.
/// </summary>
public sealed class CreatePromotionCommand : IRequest<int>
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal DiscountPercentage { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    /// <summary>
    /// One of Market.Domain.Models.PromotionTargetTypes (Games, Category, Genre, Publisher, Developer, Tag).
    /// </summary>
    public string TargetType { get; set; } = string.Empty;

    /// <summary>
    /// Category or tag id (for TargetType Category / Tag).
    /// </summary>
    public int? TargetId { get; set; }

    /// <summary>
    /// Genre, publisher or developer name (for TargetType Genre / Publisher / Developer).
    /// </summary>
    public string? TargetText { get; set; }

    /// <summary>
    /// Individually picked games (for TargetType Games).
    /// </summary>
    public List<int>? GameIds { get; set; }
}
