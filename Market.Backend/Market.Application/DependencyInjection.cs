using Market.Application.Common.Behaviors;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Market.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // MediatR samo iz Application sloja
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        // FluentValidation iz Application sloja
        services.AddValidatorsFromAssembly(assembly);

        // Pipeline behaviors (npr. ValidationBehavior)
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        // TimeProvider — ako ga koriste handleri
        services.AddSingleton(TimeProvider.System);

        return services;
    }
}
