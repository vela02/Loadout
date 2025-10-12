using Market.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Market.Infrastructure.Database.Seeders;

/// <summary>
/// Dynamic seeder koji se pokreće u runtime-u,
/// obično pri startu aplikacije (npr. u Program.cs).
/// Koristi se za unos demo/test podataka koji nisu dio migracije.
/// </summary>
public static class DynamicDataSeeder
{
    public static async Task SeedAsync(DatabaseContext context)
    {
        // Osiguraj da baza postoji (bez migracija)
        await context.Database.EnsureCreatedAsync();

        // Seed kategorija ako ih nema
        await SeedProductCategoriesAsync(context);

        // U budućnosti možeš dodati:
        // await SeedProductsAsync(context);
        // await SeedUsersAsync(context);
    }

    private static async Task SeedProductCategoriesAsync(DatabaseContext context)
    {
        if (!await context.ProductCategories.AnyAsync())
        {
            context.ProductCategories.AddRange(
                new ProductCategoryEntity
                {
                    Name = "Računari (demo)",
                    IsEnabled = true,
                    CreatedAt = DateTime.Now
                },
                new ProductCategoryEntity
                {
                    Name = "Mobilni uređaji (demo)",
                    IsEnabled = true,
                    CreatedAt = DateTime.Now
                }
            );

            await context.SaveChangesAsync();
            Console.WriteLine("✅ Dynamic seed: product categories added.");
        }
    }
}
