using Market.API;
using Market.Application;
using Market.Infrastructure;

public partial class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Registracije po slojevima
        builder.Services
            .AddAPI(builder.Configuration, builder.Environment)
            .AddApplication()
            .AddInfrastructure(builder.Configuration, builder.Environment);

        var app = builder.Build();

        // Middleware pipeline
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseMiddleware<ExceptionMiddleware>();

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        // Migracije + seeding (centralizovano u Infrastructure)
        await app.Services.InitializeDatabaseAsync(app.Environment);

        app.Run();
    }
}
