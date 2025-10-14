using Market.Application.Abstractions;
using Market.Domain.Entities;
using Market.Domain.Entities.Identity;

namespace Market.Infrastructure.Database;

public partial class DatabaseContext : DbContext, IAppDbContext
{
    public DbSet<ProductCategoryEntity> ProductCategories => Set<ProductCategoryEntity>();
    public DbSet<ProductEntity> Products => Set<ProductEntity>();
    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<RefreshTokenEntity> RefreshTokens => Set<RefreshTokenEntity>();

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }
}