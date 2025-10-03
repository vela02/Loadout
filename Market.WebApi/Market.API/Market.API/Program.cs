using Market.Infrastructure.Database;
using Market.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
var connectionString = builder.Configuration.GetConnectionString(ConfigurationValues.ConnectionString);
builder.Services.AddDbContext<DatabaseContext>(options =>
{
    if (builder.Environment.IsEnvironment("IntegrationTests"))
    {
        // za integracijske testove, koristi InMemory
        options.UseInMemoryDatabase("IntegrationTestsDb");
    }
    else
    {
        // za normalni runtime, koristi SQL Server
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
});
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Market.Features.ProductCategories.CreateProductCategory.CreateProductCategoryCommandHandler).Assembly);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program
{ }