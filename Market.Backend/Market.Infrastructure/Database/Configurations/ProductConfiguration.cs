using Market.Domain.Entities;

namespace Market.Infrastructure.Database.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<ProductEntity>
{
    public void Configure(EntityTypeBuilder<ProductEntity> builder)
    {
        builder
            .ToTable("Products");

        builder
            .Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(ProductEntity.Constraints.NameMaxLength);

        builder
            .Property(x => x.Description)
            .HasMaxLength(ProductEntity.Constraints.DescriptionMaxLength);

        builder
            .Property(x => x.Price)
            .HasColumnType("decimal(18,2)");

        builder
            .Property(x => x.StockQuantity)
            .IsRequired();

        builder
            .HasOne(x => x.Category)
            .WithMany(x=>x.Products)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);// Restrict <-- ne dozvoli brisanje kategorije ako postoje proizvodi
    }
}
