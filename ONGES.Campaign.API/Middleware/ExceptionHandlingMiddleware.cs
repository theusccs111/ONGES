namespace ONGES.Campaign.API.Middleware;

using Application.DTOs;
using System.Text.Json;

/// <summary>
/// Middleware para tratamento global de exceções.
/// </summary>
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            KeyNotFoundException => new { StatusCode = 404, message = exception.Message },
            ArgumentException => new { StatusCode = 400, message = exception.Message },
            InvalidOperationException => new { StatusCode = 400, message = exception.Message },
            _ => new { StatusCode = 500, message = "An internal server error occurred" }
        };

        context.Response.StatusCode = response.StatusCode;
        return context.Response.WriteAsJsonAsync(new ApiResponse<object>(
            false,
            response.message,
            null,
            [exception.Message]));
    }
}

/// <summary>
/// Extensões para registrar middlewares.
/// </summary>
public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
