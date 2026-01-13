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
                .AddAPI(builder.Configuration, builder.Environment)
                .AddInfrastructure(builder.Configuration, builder.Environment)
                .AddApplication();

            // DODANO: Eksplicitna registracija HttpContextAccessor-a 
            // (Potrebno za naš Audit Log da izvuče IP adresu i korisnika)
            builder.Services.AddHttpContextAccessor();

            // ---------------------------------------------------------
            // CORS policy - Prilagođeno za Angular (localhost:4200)
            // ---------------------------------------------------------
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularDev",
                    policy =>
                    {
                        policy
                            .WithOrigins("http://localhost:4200") // Angular default port
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials(); // Dozvoljava slanje JWT tokena
                    });
            });

            // JSON Options: Spriječavamo kružne reference (iako koristimo DTO modele)
            builder.Services.AddControllers().AddJsonOptions(x =>
            {
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                // Osiguravamo da JSON koristi camelCase (standard za Angular)
                x.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            });

            var app = builder.Build();

            // ---------------------------------------------------------
            // 4. Middleware pipeline
            // ---------------------------------------------------------

            // 1. Redoslijed je bitan: Prvo hvatamo greške
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // 2. Logging i Sigurnost
            app.UseMiddleware<RequestResponseLoggingMiddleware>();
            app.UseHttpsRedirection();

            // 3. CORS MORA biti prije Auth-a
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