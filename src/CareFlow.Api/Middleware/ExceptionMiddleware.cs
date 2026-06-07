using System.Text.Json;
using CareFlow.Domain.Exceptions;

namespace CareFlow.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger)
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
        catch (NotFoundException ex)
        {
            await HandleExceptionAsync(
                context,
                StatusCodes.Status404NotFound,
                ex.Message);
        }
        catch (ValidationException ex)
        {
            await HandleExceptionAsync(
                context,
                StatusCodes.Status400BadRequest,
                ex.Message);
        }
        catch (ConflictException ex)
        {
            await HandleExceptionAsync(
                context,
                StatusCodes.Status409Conflict,
                ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            await HandleExceptionAsync(
                context,
                StatusCodes.Status500InternalServerError,
                ex.Message);
        }
    }

    private static Task HandleExceptionAsync(
        HttpContext context,
        int statusCode,
        string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var response = new
        {
            status = statusCode,
            message
        };

        return context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }
}