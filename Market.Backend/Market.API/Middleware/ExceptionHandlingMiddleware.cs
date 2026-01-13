using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Hosting; // Dodaj ovaj using

namespace Market.API.Middleware;

// Dodali smo IHostEnvironment env u primarni konstruktor
public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment env)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Desila se neočekivana greška.");
            await HandleExceptionAsync(context, ex, env); // Prosljeđujemo env
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception, IHostEnvironment env)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        // LOGIKA: Ako je "Development" (tvoj PC), prikaži detalje. 
        // Ako je "Production" (Azure/Internet), sakrij ih.
        var response = new
        {
            StatusCode = context.Response.StatusCode,
            Message = "Došlo je do greške na serveru. Molimo pokušajte kasnije.",
            Detailed = env.IsDevelopment() ? exception.Message : "Detalji su skriveni iz sigurnosnih razloga."
        };

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        return context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}