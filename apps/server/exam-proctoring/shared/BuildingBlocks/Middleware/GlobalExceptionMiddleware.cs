using BuildingBlocks.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var traceId = context.TraceIdentifier;
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                if (context.Response.HasStarted) return;
                await HanldeValidationExceptionAsync(context, ex);
            }
            catch (BaseException ex)
            {
                if (context.Response.HasStarted) return;
                await HandleBaseException(context, ex);
            }
            catch (Exception ex)
            {
                if (context.Response.HasStarted) return;
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleBaseException(HttpContext context, BaseException ex)
        {
            var traceId = context.TraceIdentifier;

            _logger.LogWarning(ex,
                "Handled Exception: {ErrorCode} - TraceId: {TraceId}",
                ex.ErrorCode,
                traceId);

            context.Response.StatusCode = ex.StatusCode;

            await context.Response.WriteAsJsonAsync(new
            {
                success = false,
                message = ex.Message,
                details = ex.Details,
                errorCode = ex.ErrorCode,
                traceId
            });
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var traceId = context.TraceIdentifier;

            _logger.LogError(ex,
                "Unhandled Exception - TraceId: {TraceId}",
                traceId);

            context.Response.StatusCode = 500;

            await context.Response.WriteAsJsonAsync(new
            {
                success = false,
                message = "Internal Server Error",
                errorCode = "INTERNAL_ERROR",
                traceId
            });
        }

        private async Task HanldeValidationExceptionAsync(HttpContext context, ValidationException ex)
        {
            var traceId = context.TraceIdentifier;

            _logger.LogWarning(ex,
                "Validation Exception - TraceId: {TraceId}",
                traceId);

            context.Response.StatusCode = 400;

            await context.Response.WriteAsJsonAsync(new
            {
                success = false,
                message = "Validation failed",
                errorCode = "VALIDATION_ERROR",
                details = ex.Errors.Select(e => new
                {
                    field = e.PropertyName,
                    error = e.ErrorMessage
                }),
                traceId
            });
        }
    }
}