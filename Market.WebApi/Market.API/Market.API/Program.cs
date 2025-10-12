using FluentValidation.AspNetCore;
using Market.API.Controllers;
using Market.API.Middleware;
using Market.Features.Common.Behaviors;
using Market.Features.ProductCategories.Commands.Create;
using Market.Shared.Constants;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(opts =>
    {
        // Ova opcija definiše šta se dešava kada ASP.NET Core ne uspije validirati model
        // još prije nego što dođe do FluentValidation validacije ili handlera.
        //
        // To se dešava, naprimjer, ako:
        //  - JSON koji je poslao klijent nije ispravan (npr. očekuje se int, a pošalje string)
        //  - obavezno polje (required) uopšte nije poslano
        //  - model binding ne može popuniti objekt iz requesta
        //
        // U tim slučajevima FluentValidation se uopšte ne pokreće,
        // pa ovdje vraćamo standardizovan JSON odgovor (ErrorDto)
        // umjesto defaultnog ASP.NET odgovora.
        //
        // Na ovaj način, sve greške validacije u aplikaciji — bilo da dolaze iz model bindera
        // ili iz FluentValidation-a — imaju isti format i isti ErrorDto izgled.
        opts.InvalidModelStateResponseFactory = ctx =>
        {
            // Izvlačimo sve poruke o greškama iz ModelState-a
            var msg = string.Join("; ",
                ctx.ModelState.Values.SelectMany(v => v.Errors)
                                     .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage)
                                         ? "Validation error"
                                         : e.ErrorMessage));

            // Vraćamo HTTP 400 (Bad Request) sa našim standardnim ErrorDto formatom
            return new BadRequestObjectResult(new ErrorDto
            {
                Code = "validation.failed",
                Message = msg
            });
        };
    });

// svi validatori iz Features sklopa
builder.Services.AddValidatorsFromAssembly(
    typeof(CreateProductCategoryCommand).Assembly
);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

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
        options.UseSqlServer(builder.Configuration.GetConnectionString(ConfigurationValues.ConnectionString.Main));
    }
});
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(
        typeof(ProductCategoryController).Assembly, //ukljucuje sve servise iz projekta gdje se nalazi ProductCategoryController, tj. Market.API
        typeof(CreateProductCategoryCommand).Assembly  //ukljucuje sve servise iz projekta gdje se nalazi CreateProductCategoryCommand, tj. Market.Features
    );
});

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddTransient<ExceptionMiddleware>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program
{ }