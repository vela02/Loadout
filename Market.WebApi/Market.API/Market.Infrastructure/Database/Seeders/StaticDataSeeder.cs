using Microsoft.EntityFrameworkCore;

namespace Market.Infrastructure.Database.Seeders;

public partial class StaticDataSeeder
{
    private static DateTime DateTime { get; set; } = new DateTime(2022, 4, 13, 1, 22, 18, 866, DateTimeKind.Local);

    public static void Seed(ModelBuilder modelBuilder)
    {
        // statički podaci se dodaj u migraciju
        // ako ne postoje u DB-u u trenutku kreiranja migracije
        // primjer statičkih podataka su npr. uloge (roles)
        SeedProductCategories(modelBuilder);
    }

    private static void SeedProductCategories(ModelBuilder modelBuilder)
    {
        // todo: user roles

        //modelBuilder.Entity<UserRoles>().HasData(new List<UserRoleEntity>
        //{
        //    new UserRoleEntity{
        //        Id = 1,
        //        Name = "Admin",
        //        CreatedAt = dateTime,
        //        ModifiedAt = null,
        //    },
        //    new UserRoleEntity{
        //        Id = 2,
        //        Name = "Employee",
        //        CreatedAt = dateTime,
        //        ModifiedAt = null,
        //    },
        //});
    }
}