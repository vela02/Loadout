using Market.Domain.Models;

namespace Market.Application.Abstractions;

public interface IAppDbContext
{
    // catalog
    DbSet<Game> Products { get; }
    DbSet<GameCategory> ProductCategories { get; }
    DbSet<GameTag> GameTags { get; }

    // 2. users and roles
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<RefreshTokenEntity> RefreshTokens { get; }

    // 3. cart and wishlist
    DbSet<Cart> Carts { get; }
    DbSet<CartItem> CartItems { get; }
    DbSet<Wishlist> Wishlists { get; }

    // 4. oreders and payments
    DbSet<Order> Orders { get; }
    DbSet<OrderGame> OrderItems { get; }   // order items are called OrderGame in the domain model
    DbSet<OrderStatus> OrderStatuses { get; }
    DbSet<Payment> Payments { get; }
    DbSet<PaymentStatus> PaymentStatuses { get; }

    // 5. interaction
    DbSet<Comment> Comments { get; }
    DbSet<Rating> Ratings { get; }
    DbSet<Notification> Notifications { get; }

    // marketing
    DbSet<Coupon> Coupons { get; }
    DbSet<Discount> Discounts { get; }
    DbSet<License> Licenses { get; }
    DbSet<Refund> Refunds { get; }
    DbSet<RefundStatus> RefundStatuses { get; }

    
    Task<int> SaveChangesAsync(CancellationToken ct);
}