using Market.Domain.Entities;
using Market.Domain.Entities.Identity;

namespace Market.Application.Abstractions
{
    // Application sloj
    public interface IAppDbContext
    {
        DbSet<ProductEntity> Products { get; }
        DbSet<ProductCategoryEntity> ProductCategories { get; }
        DbSet<UserEntity> Users { get; }
        DbSet<RefreshTokenEntity> RefreshTokens { get; }
        Task<int> SaveChangesAsync(CancellationToken ct);
    }
}
