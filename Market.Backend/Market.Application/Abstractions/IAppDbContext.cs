using Market.Domain.Entities.Catalog;

namespace Market.Application.Abstractions
{
    // Application sloj
    public interface IAppDbContext
    {
        DbSet<ProductEntity> Products { get; }
        DbSet<ProductCategoryEntity> ProductCategories { get; }
        DbSet<MarketUserEntity> Users { get; }
        DbSet<RefreshTokenEntity> RefreshTokens { get; }
        Task<int> SaveChangesAsync(CancellationToken ct);
    }
}
