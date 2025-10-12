using Market.API.Controllers;
using Market.API.Middleware;
using Market.Features.Common.Behaviors;
using Market.Features.ProductCategories.Commands.Create;
using Market.Infrastructure.Database.Seeders;
using Market.Shared.Constants;
using Market.Shared.Extensions;
// Potrebno za WebApplicationFactory u integracijskim testovima

public partial class Program {
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // ---------------------------------------------------------
        // Services
        // ---------------------------------------------------------
        builder.Services
            .AddControllers()
            .ConfigureApiBehaviorOptions(opts =>
            {
                // Standardizacija odgovora kada model binding/JSON parsing padne
                opts.InvalidModelStateResponseFactory = ctx =>
                {
                    var msg = string.Join("; ",
                        ctx.ModelState.Values.SelectMany(v => v.Errors)
                                             .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage)
                                                 ? "Validation error"
                                                 : e.ErrorMessage));

                    return new BadRequestObjectResult(new ErrorDto
                    {
                        Code = "validation.failed",
                        Message = msg
                    });
                };
            });

        // FluentValidation — automatsko registrovanje svih validatora iz Features sklopa
        builder.Services.AddValidatorsFromAssembly(typeof(CreateProductCategoryCommand).Assembly);

        // Swagger/OpenAPI
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // DbContext (InMemory za IntegrationTests; SQL Server inače)
        builder.Services.AddDbContext<DatabaseContext>(options =>
        {
            if (builder.Environment.IsIntegrationTests())
            {
                options.UseInMemoryDatabase("IntegrationTestsDb");
            }
            else
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString(ConfigurationValues.ConnectionString.Main));
            }
        });

        // MediatR — registruj servise iz API i Features projekata
        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(
                typeof(ProductCategoryController).Assembly,           // Market.API
                typeof(CreateProductCategoryCommand).Assembly         // Market.Features
            );
        });

        // Pipeline behavior: FluentValidation pre MediatR handlera
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        // Custom global exception middleware
        builder.Services.AddTransient<ExceptionMiddleware>();

        var app = builder.Build();

        // ---------------------------------------------------------
        // Middleware pipeline
        // ---------------------------------------------------------
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseMiddleware<ExceptionMiddleware>();

        app.UseHttpsRedirection();
        app.UseAuthorization();

        app.MapControllers();

        // ---------------------------------------------------------
        // Dynamic seeding (runtime)
        // - InMemory: EnsureCreated + uvijek seed (za testove)
        // - SQL Server: Migrate + seed u Development
        // ---------------------------------------------------------
        await using (var scope = app.Services.CreateAsyncScope())
        {
            var ctx = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

            if (app.Environment.IsIntegrationTests())
            {
                await ctx.Database.EnsureCreatedAsync();
                await DynamicDataSeeder.SeedAsync(ctx);
            }
            else
            {
                await ctx.Database.MigrateAsync();

                if (app.Environment.IsDevelopment())
                {
                    await DynamicDataSeeder.SeedAsync(ctx);
                }
            }
        }

        app.Run();
    }
}
