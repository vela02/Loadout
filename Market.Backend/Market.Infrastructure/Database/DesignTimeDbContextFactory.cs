using Market.Shared.Options;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Market.Infrastructure.Database;

/// <summary>
/// 🇬🇧 Design-time factory used by EF Core CLI (e.g., `dotnet ef`) to create
/// a <see cref="DatabaseContext"/> without starting the ASP.NET Core host.
/// </summary>
/// <remarks>
/// 🇧🇦 <b>Šta je design-time?</b>
/// Design-time je poseban režim rada kada EF Core alatke (CLI ili Visual Studio) trebaju
/// instancu DbContext-a za <i>generisanje</i> i/ili <i>primjenu</i> migracija
/// (npr. komande: <c>dotnet ef migrations add ...</c>, <c>dotnet ef database update</c>).
/// U tom režimu <b>ASP.NET Core web host se ne pokreće</b>, pa nema DI pipeline-a
/// (nema <c>Program.cs</c>, <c>WebApplication.CreateBuilder</c>, middleware-a,
/// registriranih servisa, niti <c>IHostEnvironment</c> iz DI-ja).
///
/// <b>Zašto treba ova fabrika?</b>
/// Ako postoji <see cref="IDesignTimeDbContextFactory{TContext}"/>, EF Core će
/// <b>direktno</b> pozvati ovu fabriku da dobije DbContext. Tako izbjegavamo
/// pokretanje web hosta (koje često završi sa HostAbortedException tokom migracija),
/// a imamo potpunu kontrolu nad konfiguracijom (učitavanje <c>appsettings*.json</c>,
/// čitanje <c>ASPNETCORE_ENVIRONMENT</c>, biranje providera).
///
/// <b>Razlika: design-time vs runtime</b>
/// • <u>Design-time</u>: nema ASP.NET hosta. Konfiguraciju <b>ručno</b> gradimo ovdje
///   (učitamo <c>appsettings.json</c> + <c>appsettings.{ENV}.json</c> + env varijable),
///   odaberemo <b>SQL provider</b> (migracije ne rade nad InMemory), i vratimo DbContext
///   samo za potrebe EF alata. <b>Ne smijemo</b> pokretati seed, hosted servise,
///   niti mrežne/queue poslove.
/// • <u>Runtime</u>: web aplikacija se pokreće preko <c>Program.cs</c>, DI registracija
///   (<c>AddInfrastructure</c>, <c>AddApplication</c>, …), middleware, auth, itd.
///   Tamo je okej (po potrebi) pokrenuti migracije/seed pri startu aplikacije,
///   ali to <b>nije</b> dio design-time režima.
///
/// <b>Praktične napomene</b>
/// • U design-time-u ručno čitamo <c>ASPNETCORE_ENVIRONMENT</c> da znamo koji
///   <c>appsettings.{ENV}.json</c> učitati (npr. Development).
/// • Migracije uvijek ciljaju <b>pravi</b> provider (npr. <c>UseSqlServer</c>),
///   ne InMemory.
/// • Ako runtime kod (npr. <c>InitializeDatabaseAsync</c>) radi seed/migracije,
///   ne duplirati to u design-time fabrici — ovdje DbContext služi isključivo
///   EF alatima.
/// </remarks>
public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
{
    public DatabaseContext CreateDbContext(string[] args)
    {
        // 🇬🇧 Get environment (e.g., Development, Production)
        // 🇧🇦 EF Core CLI ne pokreće naš host, pa sami moramo pročitati varijablu okruženja
        // da bismo znali koji appsettings fajl učitati (npr. appsettings.Development.json).
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        // 🇬🇧 Build configuration manually
        // 🇧🇦 EF Core ne koristi Program.cs tokom migracija, pa moramo ručno izgraditi konfiguraciju:
        // učitavamo appsettings fajlove i environment varijable kako bi connection string bio dostupan.
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{env}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        // 🇬🇧 Bind connection strings section to typed options
        // 🇧🇦 Preuzimamo sekciju "ConnectionStrings" i mapiramo je na klasu ConnectionStringsOptions,
        // jer naš projekat koristi tipske opcije, a ne direktno Configuration["..."].
        var csOptions = new ConnectionStringsOptions();
        configuration.GetSection(ConnectionStringsOptions.SectionName).Bind(csOptions);

        // 🇬🇧 Validate connection string
        // 🇧🇦 Ako connection string nije pronađen, bacamo grešku kako bi developer znao da
        // mora definisati ConnectionStrings:Main u appsettings fajlu ili kao environment varijablu.
        var connectionString = csOptions.Main
            ?? throw new InvalidOperationException($"DesignTimeDbContextFactory: Missing connection string 'ConnectionStrings:Main'.");

        // 🇬🇧 Configure DbContextOptions for SQL Server
        // 🇧🇦 Migracije ne rade nad InMemory bazama, zato ovdje uvijek koristimo SQL Server.
        // Connection string se prosljeđuje direktno u EF Core.
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseSqlServer(connectionString)
            .Options;

        // 🇬🇧 Return new DatabaseContext
        // 🇧🇦 Na kraju vraćamo instancu DatabaseContext-a koju EF koristi isključivo za
        // generisanje i primjenu migracija, bez pokretanja web aplikacije.
        return new DatabaseContext(options);
    }
}
