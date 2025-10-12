namespace Market.API.Middleware;

public sealed class ExceptionMiddleware(ILogger<ExceptionMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext ctx, RequestDelegate next)
    {
        try
        {
            await next(ctx);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");

            ctx.Response.ContentType = "application/json";
            ctx.Response.StatusCode = ex switch
            {
                MarketNotFoundException => StatusCodes.Status404NotFound,
                MarketConflictException => StatusCodes.Status409Conflict,
                ValidationException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };

            string code = "internal.error";
            string message = "Došlo je do greške. Pokušajte ponovo.";

            if (ex is MarketNotFoundException or MarketConflictException)
            {
                code = "entity.error";
                message = ex.Message;
            }
            else if (ex is ValidationException vex)
            {
                code = "validation.error";
                message = string.Join(" ", vex.Errors.Select(e => e.ErrorMessage));
            }

            var error = new ErrorDto
            {
                Code = code,
                Message = message
            };

            await ctx.Response.WriteAsJsonAsync(error);
        }
    }
}
