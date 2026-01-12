using Market.API;
using Market.API.Middleware;
using Market.Application;
using Market.Infrastructure;
using Serilog;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

public partial class Program
{
    private static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        try
        {
            Log.Information("Starting Market API...");

            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog((ctx, services, cfg) =>
            {
                cfg.ReadFrom.Configuration(ctx.Configuration)
                   .ReadFrom.Services(services)
                   .Enrich.FromLogContext()
                   .Enrich.WithThreadId()
                   .Enrich.WithProcessId()
                   .Enrich.WithMachineName();
            });

            builder.Logging.ClearProviders();

            // ---------------------------------------------------------
            // 3. Layer registrations
            // ---------------------------------------------------------
            builder.Services
                .AddAPI(builder.Configuration, builder.Environment) // <--- Ovdje je već Swagger!
                .AddInfrastructure(builder.Configuration, builder.Environment)
                .AddDbContext<LoadoutDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Main")))
                .AddApplication();

            // CORS policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularDev",
                    policy =>
                    {
                        policy
                            .WithOrigins("http://localhost:4200")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    });
            });

            builder.Services.AddControllers().AddJsonOptions(x =>
               x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

            var app = builder.Build();

            // ---------------------------------------------------------
            // 4. Middleware pipeline
            // ---------------------------------------------------------
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseExceptionHandler();
            app.UseMiddleware<RequestResponseLoggingMiddleware>();

            app.UseHttpsRedirection();
            app.UseCors("AllowAngularDev");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            Log.Information("Market API started successfully.");
            app.Run();
        }
        catch (HostAbortedException)
        {
            Log.Information("Host aborted by EF Core tooling (design-time) - its ok.");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Market API terminated unexpectedly.");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}