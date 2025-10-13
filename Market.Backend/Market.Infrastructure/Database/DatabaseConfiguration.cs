using Market.Core.Entities.Base;
using Market.Infrastructure.Database.Seeders;
using System.Linq.Expressions;

namespace Market.Infrastructure.Database;

public partial class DatabaseContext
{
    private void ModifyTimestamps()
    {
        var entries = ChangeTracker.Entries();

        foreach (var entry in entries)
        {
            var entity = ((BaseEntity)entry.Entity);

            if (entry.State == EntityState.Added)
            {
                entity.CreatedAt = DateTime.Now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entity.ModifiedAt = DateTime.Now;
            }
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ApplyGlobalFielters(modelBuilder);

        StaticDataSeeder.Seed(modelBuilder); // statički podaci
    }

    private void ApplyGlobalFielters(ModelBuilder modelBuilder)
    {
        // Primijeni globalni filter na sve entitete koji nasljeđuju BaseEntity
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var prop = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
                var compare = Expression.Equal(prop, Expression.Constant(false));
                var lambda = Expression.Lambda(compare, parameter);

                modelBuilder.Entity(entityType.ClrType)
                            .HasQueryFilter(lambda);
            }
        }
    }

    public override int SaveChanges()
    {
        ModifyTimestamps();

        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        ModifyTimestamps();

        return base.SaveChangesAsync(cancellationToken);
    }
}