using Market.Domain.Entities.Identity;

namespace Market.Infrastructure.Database.Configurations.Identity;

public sealed class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> b)
    {
        b.ToTable("Users");

        b.HasKey(x => x.Id);

        b.HasIndex(x => x.Email)
            .IsUnique();

        b.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(200);

        b.Property(x => x.PasswordHash)
            .IsRequired();

        b.Property(x => x.Role)
            .HasMaxLength(50)
            .HasDefaultValue("User");

        b.Property(x => x.TokenVersion)
            .HasDefaultValue(0);

        b.Property(x => x.IsEnabled)
            .HasDefaultValue(true);

        // Navigacija
        b.HasMany(x => x.RefreshTokens)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId);
    }
}
