using api_gateway.Exceptions;

namespace api_gateway.Middleware
{
    public class GatewayExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GatewayExceptionMiddleware> _logger;

        public GatewayExceptionMiddleware(
            RequestDelegate next,
            ILogger<GatewayExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (DownstreamApiException ex)
            {
                _logger.LogError("Downstream error {StatusCode}: {Body}", ex.StatusCode, ex.ResponseBody);
                context.Response.StatusCode = ex.StatusCode;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(ex.ResponseBody);
            }
            catch (Exception ex)
            {
                if (context.Response.HasStarted) return;

                _logger.LogError(ex, "Gateway unhandled exception");

                context.Response.StatusCode = 500;
                await context.Response.WriteAsJsonAsync(new
                {
                    success = false,
                    message = "Gateway internal error",
                    errorCode = "GATEWAY_INTERNAL_ERROR",
                    traceId = context.TraceIdentifier
                });
            }
        }
    }
}