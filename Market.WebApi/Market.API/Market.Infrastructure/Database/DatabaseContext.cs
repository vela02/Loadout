using Market.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Market.Infrastructure.Database
{
    public partial class DatabaseContext : DbContext
    {
        public DbSet<ProductCategoryEntity> ProductCategories { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }
    }
}