using FluentValidation;
using FluentValidation.AspNetCore;
using Market.API.Middleware;
using Market.Infrastructure.Database;
using Market.Shared.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(opts =>
    {
        opts.InvalidModelStateResponseFactory = ctx =>
        {
            var msg = string.Join("; ",
                ctx.ModelState.Values.SelectMany(v => v.Errors)
                                     .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? "Validation error" : e.ErrorMessage));
            return new BadRequestObjectResult(new Market.Shared.Dtos.ErrorDto
            {
                Code = "validation.failed",
                Message = msg
            });
        };
    });

builder.Services.AddFluentValidationAutoValidation(o =>
{
    // o.DisableDataAnnotationsValidation = true
    // Isključuje staru .NET validaciju putem[Required], [MaxLength], [Range], itd.
    // Da se ne miješaju dvije validacije (DataAnnotations + FluentValidation).
    o.DisableDataAnnotationsValidation = true;
});

// svi validatori iz Features sklopa
builder.Services.AddValidatorsFromAssembly(
    typeof(Market.Features.ProductCategories.CreateProductCategory.CreateProductCategoryCommand).Assembly
);

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
        options.UseSqlServer(builder.Configuration.GetConnectionString("Main"));
    }
});
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Market.Features.ProductCategories.CreateProductCategory.CreateProductCategoryCommandHandler).Assembly);
});

builder.Services.AddTransient<ExceptionMiddleware>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

app.Run();

public partial class Program
{ }