using System.Net;
using System.Text.Json;

namespace OrderManagement.API.Middlewares;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlerMiddleware> _logger;

    public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
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
            _logger.LogError(ex, "Excepción no controlada: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message) = exception switch
        {
            InvalidOperationException => (HttpStatusCode.UnprocessableEntity, exception.Message),
            ApplicationException      => (HttpStatusCode.BadGateway, exception.Message),
            ArgumentException         => (HttpStatusCode.BadRequest, exception.Message),
            _                         => (HttpStatusCode.InternalServerError, "Ocurrió un error inesperado en el servidor.")
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var body = JsonSerializer.Serialize(new
        {
            status = (int)statusCode,
            error = statusCode.ToString(),
            message
        });

        await context.Response.WriteAsync(body);
    }
}
