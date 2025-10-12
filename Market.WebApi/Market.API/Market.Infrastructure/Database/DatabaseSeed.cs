using Market.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Market.Infrastructure.Database;

public partial class DatabaseContext
{
    public DateTime dateTime { get; set; } = new DateTime(2022, 4, 13, 1, 22, 18, 866, DateTimeKind.Local);

    private void SeedData(ModelBuilder modelBuilder)
    {
        SeedProductCategories(modelBuilder);
    }

    private void SeedProductCategories(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductCategoryEntity>().HasData(new List<ProductCategoryEntity>
        {
            new ProductCategoryEntity{
                Id = 1,
                Name = "Računari",
                CreatedAt = dateTime,
                ModifiedAt = null,
            },
            new ProductCategoryEntity{
                Id = 1,
                Name = "Bijela tehnika",
                CreatedAt = dateTime,
                ModifiedAt = null,
            },
        });
    }
}