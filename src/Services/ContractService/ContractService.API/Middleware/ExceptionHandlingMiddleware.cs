using FluentValidation;
using SharedKernel.Exceptions;
using System.Net;
using System.Text.Json;

namespace ContractService.API.Middleware;

/// <summary>
/// Global exception handling middleware
/// </summary>
public class ExceptionHandlingMiddleware
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
        catch (Exception exception)
        {
            _logger.LogError(exception, "An unhandled exception occurred");
            await HandleExceptionAsync(context, exception);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        object problemDetails;
        int statusCode;

        switch (exception)
        {
            case ValidationException validationEx:
                statusCode = (int)HttpStatusCode.BadRequest;
                var errors = validationEx.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );

                problemDetails = new
                {
                    status = statusCode,
                    title = "Validation Error",
                    errors
                };
                break;

            case DomainException domainEx:
                statusCode = (int)HttpStatusCode.BadRequest;
                problemDetails = new
                {
                    status = statusCode,
                    title = "Domain Error",
                    detail = domainEx.Message
                };
                break;

            default:
                statusCode = (int)HttpStatusCode.InternalServerError;
                problemDetails = new
                {
                    status = statusCode,
                    title = "Internal Server Error",
                    detail = "An unexpected error occurred"
                };
                break;
        }

        context.Response.StatusCode = statusCode;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, options));
    }
}
