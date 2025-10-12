using Market.Shared.Dtos;

namespace Market.API.Middleware
{
    public sealed class ExceptionMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionMiddleware> _logger;
        public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger) => _logger = logger;

        public async Task InvokeAsync(HttpContext ctx, RequestDelegate next)
        {
            try { await next(ctx); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled");
                ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await ctx.Response.WriteAsJsonAsync(new ErrorDto
                {
                    Code = "internal.error",
                    Message = "Došlo je do greške. Pokušajte ponovo."
                });
            }
        }
    }
}