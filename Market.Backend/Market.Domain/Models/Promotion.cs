using System;
using System.Collections.Generic;

namespace Market.Domain.Models;

/// <summary>
/// A promotion groups the per-game discounts created from a single admin action
/// (e.g. "-30% on all RPG games"). The matching games are resolved once, when the
/// promotion is created, and stored as Discount rows linked to this promotion.
/// </summary>
public partial class Promotion
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal DiscountPercentage { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    /// <summary>
    /// What the promotion targeted, see <see cref="PromotionTargetTypes"/>.
    /// </summary>
    public string TargetType { get; set; } = null!;

    /// <summary>
    /// Human readable target (genre, publisher, category name...). Null for individually picked games.
    /// </summary>
    public string? TargetValue { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Discount> Discounts { get; set; } = new List<Discount>();
}

public static class PromotionTargetTypes
{
    public const string Games = "Games";
    public const string Category = "Category";
    public const string Genre = "Genre";
    public const string Publisher = "Publisher";
    public const string Developer = "Developer";
    public const string Tag = "Tag";

    public static readonly string[] All = { Games, Category, Genre, Publisher, Developer, Tag };
}
