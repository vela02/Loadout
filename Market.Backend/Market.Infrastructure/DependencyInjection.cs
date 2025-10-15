using Market.Application.Abstractions;
using Market.Infrastructure.Common;
using Market.Infrastructure.Database;
using Market.Shared.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Market.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment env)
    {
        // Tipizirane ConnectionStrings + validacija
        services.AddOptions<ConnectionStringsOptions>()
            .Bind(configuration.GetSection(ConnectionStringsOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // DbContext: InMemory za test okoline; SQL Server inače
        services.AddDbContext<DatabaseContext>((sp, options) =>
        {
            if (env.IsEnvironment("IntegrationTests") || env.IsEnvironment("Testing"))
            {
                options.UseInMemoryDatabase("IntegrationTestsDb");
                return;
            }

            var cs = sp.GetRequiredService<IOptions<ConnectionStringsOptions>>().Value.Main;
            options.UseSqlServer(cs);
        });

        // IAppDbContext mapiranje
        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<DatabaseContext>());

        // Identity hasher
        services.AddScoped<IPasswordHasher<MarketUserEntity>, PasswordHasher<MarketUserEntity>>();

        // Token service (čita JwtOptions preko IOptions<JwtOptions>)
        services.AddTransient<IJwtTokenService, JwtTokenService>();

        // HttpContext accessor + current user
        services.AddHttpContextAccessor();
        services.AddScoped<IAppCurrentUser, AppCurrentUser>();

        // TimeProvider (ako ga koristiš u handlerima/servisima)
        services.AddSingleton<TimeProvider>(TimeProvider.System);

        return services;
    }
}
