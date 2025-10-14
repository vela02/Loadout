using Market.Domain.Common;

namespace Market.Domain.Entities.Catalog;

/// <summary>
/// Predstavlja proizvod u sistemu.
/// </summary>
public class ProductEntity : BaseEntity
{
    /// <summary>
    /// Naziv proizvoda.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Opis proizvoda. (optional)
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Cijena proizvoda.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Količina proizvoda dostupna na stanju.
    /// </summary>
    public int StockQuantity { get; set; }

    /// <summary>
    /// Identifikator kategorije kojoj proizvod pripada.
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// Navigacijska referenca na kategoriju proizvoda.
    /// </summary>
    public ProductCategoryEntity? Category { get; set; }

    /// <summary>
    /// IsEnabled
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// Jedan izvor istine za tehnička/poslovna ograničenja.
    /// Koristi se u validatorima i EF konfiguraciji.
    /// </summary>
    public static class Constraints
    {
        public const int NameMaxLength = 150;

        public const int DescriptionMaxLength = 1000;
    }
}
