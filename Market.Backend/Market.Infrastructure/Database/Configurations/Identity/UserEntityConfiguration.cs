using Market.Domain.Models;

namespace Market.Infrastructure.Database.Configurations.Identity;

public class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.ToTable("Users");
        b.HasKey(x => x.Id);

        
        b.HasOne(x => x.Role)
         .WithMany(r => r.Users)
         .HasForeignKey(x => x.RoleId)
         .OnDelete(DeleteBehavior.Restrict);

        
        b.HasMany(x => x.RefreshTokens)
         .WithOne(x => x.User)
         .HasForeignKey(x => x.UserId);

        
        b.Property(x => x.Username).IsRequired().HasMaxLength(50);
        b.Property(x => x.Email).IsRequired().HasMaxLength(100);
        b.Property(x => x.IsEnabled).HasDefaultValue(true);
        b.Property(x => x.IsDeleted).HasDefaultValue(false);
    }
}