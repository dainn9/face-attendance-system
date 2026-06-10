using Microsoft.AspNetCore.Builder;

namespace BuildingBlocks.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
            => app.UseMiddleware<GlobalExceptionMiddleware>();
    }
}