using Market.Infrastructure.Database.Seeders;
using Market.Shared.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;        // TokenValidationParameters, SymmetricSecurityKey
using System.Text;
using Market.Application.Abstractions;
using Market.Application.Common.Behaviors;
using Market.Shared;
using Market.Application.Features.ProductCategories.Commands.Create;                            // Encoding.UTF8.GetBytes(...)

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

        builder.Services.AddOptions<JwtOptions>()
            .Bind(builder.Configuration.GetSection("Jwt"))
            .Validate(o => !string.IsNullOrWhiteSpace(o.Key), "Jwt:Key is missing.")
            .Validate(o => !string.IsNullOrWhiteSpace(o.Issuer), "Jwt:Issuer is missing.")
            .Validate(o => !string.IsNullOrWhiteSpace(o.Audience), "Jwt:Audience is missing.")
            .ValidateOnStart();

        // nakon .AddOptions<JwtOptions>()...
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            var jwtSection = builder.Configuration.GetSection("Jwt");
            var key = jwtSection["Key"];
            var issuer = jwtSection["Issuer"];
            var audience = jwtSection["Audience"];

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        builder.Services.AddAuthorization(o =>
        {
            o.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        // FluentValidation — automatsko registrovanje svih validatora iz Features sklopa
        builder.Services.AddValidatorsFromAssembly(typeof(CreateProductCategoryCommand).Assembly);

        // Swagger/OpenAPI
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Market API", Version = "v1" });

            // XML komentari (ako koristiš)
            var xml = Path.Combine(AppContext.BaseDirectory, "Market.API.xml");
            if (File.Exists(xml)) c.IncludeXmlComments(xml, includeControllerXmlComments: true);

            // JWT Bearer security definicija
            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Unesi JWT token. Format: **Bearer {token}**",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            };
            c.AddSecurityDefinition("Bearer", securityScheme);

            // Globalni zahtjev: svi endpointi očekuju Bearer, osim onih s [AllowAnonymous]
            var securityRequirement = new OpenApiSecurityRequirement
            {
                { securityScheme, Array.Empty<string>() }
            };
            c.AddSecurityRequirement(securityRequirement);
        });

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

        builder.Services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<DatabaseContext>());

        // MediatR — registruj servise iz API i Features projekata
        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(
                typeof(CreateProductCategoryCommand).Assembly         // Market.Features
            );
        });

        // Pipeline behavior: FluentValidation pre MediatR handlera
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        builder.Services.AddTransient(typeof(IJwtTokenService), typeof(JwtTokenService));

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
        app.UseAuthentication();   // VAŽNO: prije Authorization
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
