using System.Net;
using System.Net.Mime;
using FlightScanner.DTOs.Exceptions;

namespace FlightScanner.WebApi.Middleware;

/// <summary>
/// Global exception middleware handler which applies to all requests, including non-MVC
/// ones, e.g. APIs, WebSocket connections, other middleware exceptions...
/// </summary>
public class GlobalExceptionHandlerMiddleware : IMiddleware
{
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "An error occurred while processing request: {@exception.Message}", exception);

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = MediaTypeNames.Application.Json;

            var response = new ExceptionDto(
                title: "An unexpected error occurred.",
                exceptionMessage: exception.Message,
                stackTrace: exception.StackTrace ?? string.Empty,
                innerExceptionStackTrace: exception.InnerException?.StackTrace);

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
