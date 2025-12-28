using Market.Domain.Models;

namespace Market.Infrastructure.Database.Configurations;

public class RefundConfiguration : IEntityTypeConfiguration<Refund>
{
    public void Configure(EntityTypeBuilder<Refund> builder)
    {
        
        builder.HasKey(x => x.Id);

        // FK: Refund belongs to an Order
        builder.HasOne(r => r.Order)
               .WithMany() 
               .HasForeignKey(r => r.OrderId)
               .OnDelete(DeleteBehavior.Restrict); //  Don't delete refund if order is deleted 

        
        builder.HasOne(r => r.Status)
               .WithMany()
               .HasForeignKey(r => r.StatusId);

        
        builder.Property(r => r.Reason)
               .IsRequired()
               .HasMaxLength(500);

        builder.Property(r => r.AdminResponse)
               .IsRequired(false) 
               .HasMaxLength(500);

        builder.Property(r => r.Amount)
               .HasColumnType("decimal(18,2)");
    }
}