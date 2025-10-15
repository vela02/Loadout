using Market.Application.Abstractions;
using Market.Infrastructure.Common;
using Market.Infrastructure.Database;
using Market.Shared.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Market.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment env)
    {
        // DbContext (InMemory za IntegrationTests/Testing; SQL Server inače)
        services.AddDbContext<DatabaseContext>(options =>
        {
            if (env.IsEnvironment("IntegrationTests") || env.IsEnvironment("Testing"))
            {
                options.UseInMemoryDatabase("IntegrationTestsDb");
            }
            else
            {
                options.UseSqlServer(configuration.GetConnectionString(ConfigurationValues.ConnectionString.Main));
            }
        });

        // IAppDbContext mapiranje
        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<DatabaseContext>());

        // Identity hasher
        services.AddScoped<IPasswordHasher<MarketUserEntity>, PasswordHasher<MarketUserEntity>>();

        // Token service
        services.AddTransient<IJwtTokenService, JwtTokenService>();

        // HttpContext accessor (ako ga koristiš u AppCurrentUser i sl.)
        services.AddHttpContextAccessor();
        services.AddScoped<IAppCurrentUser, AppCurrentUser>();

        return services;
    }
}
