using Market.Application.Abstractions;
using Market.Domain.Models;


namespace Market.Application;

public partial class LoadoutDbContext : DbContext, IAppDbContext
{
    public LoadoutDbContext()
    {
    }

    public LoadoutDbContext(DbContextOptions<LoadoutDbContext> options)
        : base(options)
    {
    }
    // next 3 added temporarily to satisfy IAppDbContext, to be removed later
    
    public virtual DbSet<Game> Products => Games;
    public virtual DbSet<GameCategory> ProductCategories => GameCategories;
    public virtual DbSet<OrderGame> OrderItems => OrderGames;



    public virtual DbSet<RefreshTokenEntity> RefreshTokens { get; set; }
    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Coupon> Coupons { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<GameCategory> GameCategories { get; set; }

    public virtual DbSet<GameTag> GameTags { get; set; }

    public virtual DbSet<License> Licenses { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderGame> OrderGames { get; set; }

    public virtual DbSet<OrderStatus> OrderStatuses { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PaymentStatus> PaymentStatuses { get; set; }

    public virtual DbSet<Rating> Ratings { get; set; }

    public virtual DbSet<Refund> Refunds { get; set; }

    public virtual DbSet<RefundStatus> RefundStatuses { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Wishlist> Wishlists { get; set; }
    public virtual DbSet<LogAction> LogActions { get; set; }

    public Task<int> SaveChangesAsync(CancellationToken ct) => base.SaveChangesAsync(ct);

    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    //        => optionsBuilder.UseSqlServer("Server=DESKTOP-JT2J2EF;Database=LoadoutDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cart__3214EC07F442E365");

            entity.ToTable("Cart");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.Carts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Cart__UserId__49C3F6B7");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CartItem__3214EC07ABB17501");

            entity.ToTable("CartItem");

            entity.Property(e => e.AddedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Cart).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.CartId)
                .HasConstraintName("FK__CartItem__CartId__4CA06362");

            entity.HasOne(d => d.Game).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.GameId)
                .HasConstraintName("FK__CartItem__GameId__4D94879B");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Comment__3214EC07CC3074F0");

            entity.ToTable("Comment");

            entity.Property(e => e.Content).HasColumnType("text");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Game).WithMany(p => p.Comments)
                .HasForeignKey(d => d.GameId)
                .HasConstraintName("FK__Comment__GameId__6754599E");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Comment__UserId__66603565");
        });

        modelBuilder.Entity<Coupon>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Coupon__3214EC0767568CB8");

            entity.ToTable("Coupon");

            entity.Property(e => e.Code)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.DiscountPercentage).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.ExpiryDate).HasColumnType("datetime");
            entity.Property(e => e.IsEnabled).HasDefaultValue(true);

            entity.HasOne(d => d.User).WithMany(p => p.Coupons)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Coupon__UserId__787EE5A0");
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Discount__3214EC0781DF98F2");

            entity.ToTable("Discount");

            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.DiscountPercentage).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");

            entity.HasOne(d => d.Category).WithMany(p => p.Discounts)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__Discount__Catego__75A278F5");

            entity.HasOne(d => d.Game).WithMany(p => p.Discounts)
                .HasForeignKey(d => d.GameId)
                .HasConstraintName("FK__Discount__GameId__74AE54BC");
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Game__3214EC074BE90CF5");

            entity.ToTable("Game");

            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.Developer)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Genre)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.IsEnabled).HasDefaultValue(true);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Publisher)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Category).WithMany(p => p.Games)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__Game__CategoryId__412EB0B6");

            entity.HasMany(d => d.Tags).WithMany(p => p.Games)
                .UsingEntity<Dictionary<string, object>>(
                    "GameGameTag",
                    r => r.HasOne<GameTag>().WithMany()
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Game_Game__TagId__46E78A0C"),
                    l => l.HasOne<Game>().WithMany()
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Game_Game__GameI__45F365D3"),
                    j =>
                    {
                        j.HasKey("GameId", "TagId").HasName("PK__Game_Gam__FCEF5867EC40DEDD");
                        j.ToTable("Game_GameTag");
                    });
        });

        modelBuilder.Entity<GameCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__GameCate__3214EC0785AFCF0F");

            entity.ToTable("GameCategory");

            entity.Property(e => e.IsEnabled).HasDefaultValue(true);
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.ParentCategory).WithMany(p => p.InverseParentCategory)
                .HasForeignKey(d => d.ParentCategoryId)
                .HasConstraintName("FK__GameCateg__Paren__3E52440B");
        });

        modelBuilder.Entity<GameTag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__GameTag__3214EC07D4C3723B");

            entity.ToTable("GameTag");

            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<License>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__License__3214EC07EF747891");

            entity.ToTable("License");

            entity.Property(e => e.ExpiryDate).HasColumnType("datetime");
            entity.Property(e => e.IssueDate).HasColumnType("datetime");
            entity.Property(e => e.LicenseKey)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Game).WithMany(p => p.Licenses)
                .HasForeignKey(d => d.GameId)
                .HasConstraintName("FK__License__GameId__6383C8BA");

            entity.HasOne(d => d.User).WithMany(p => p.Licenses)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__License__UserId__628FA481");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Notifica__3214EC076EBE9EB1");

            entity.ToTable("Notification");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Message).HasColumnType("text");
            entity.Property(e => e.Type)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Notificat__UserI__71D1E811");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Order__3214EC078C83949E");

            entity.ToTable("Order");

            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Status).WithMany(p => p.Orders)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__Order__StatusId__534D60F1");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Order__UserId__52593CB8");
        });

        modelBuilder.Entity<OrderGame>(entity =>
        {
            entity.HasKey(e => new { e.OrderId, e.GameId }).HasName("PK__OrderGam__013BD2B0DB54FA62");

            entity.ToTable("OrderGame");

            entity.Property(e => e.PriceAtPurchase).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Game).WithMany(p => p.OrderGames)
                .HasForeignKey(d => d.GameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderGame__GameI__571DF1D5");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderGames)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderGame__Order__5629CD9C");
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrderSta__3214EC07F29F9D67");

            entity.ToTable("OrderStatus");

            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Payment__3214EC076942E066");

            entity.ToTable("Payment");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PaymentDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Order).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__Payment__OrderId__5BE2A6F2");

            entity.HasOne(d => d.Status).WithMany(p => p.Payments)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__Payment__StatusI__5CD6CB2B");
        });

        modelBuilder.Entity<PaymentStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PaymentS__3214EC07B3992356");

            entity.ToTable("PaymentStatus");

            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Rating__3214EC0760817282");

            entity.ToTable("Rating");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Game).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.GameId)
                .HasConstraintName("FK__Rating__GameId__6B24EA82");

            entity.HasOne(d => d.User).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Rating__UserId__6A30C649");
        });

        modelBuilder.Entity<Refund>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Refund__3214EC07EFB25DFD");

            entity.ToTable("Refund");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Reason).HasColumnType("text");
            entity.Property(e => e.RefundDate).HasColumnType("datetime");

            entity.HasOne(d => d.Payment).WithMany(p => p.Refunds)
                .HasForeignKey(d => d.PaymentId)
                .HasConstraintName("FK__Refund__PaymentI__08B54D69");

            entity.HasOne(d => d.Status).WithMany(p => p.Refunds)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__Refund__StatusId__09A971A2");
        });

        modelBuilder.Entity<RefundStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RefundSt__3214EC07E201D43A");

            entity.ToTable("RefundStatus");

            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Role__3214EC075345DCB1");

            entity.ToTable("Role");

            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC07B014BA68");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.IsEnabled).HasDefaultValue(true);
            entity.Property(e => e.LastLogin).HasColumnType("datetime");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__User__RoleId__3B75D760");
        });

        modelBuilder.Entity<Wishlist>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Wishlist__3214EC072604A562");

            entity.ToTable("Wishlist");

            entity.Property(e => e.AddedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Game).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.GameId)
                .HasConstraintName("FK__Wishlist__GameId__6EF57B66");

            entity.HasOne(d => d.User).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Wishlist__UserId__6E01572D");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
