using FluentValidation;
using SharedKernel.Exceptions;
using System.Net;
using System.Text.Json;

namespace ProposalService.API.Middleware;

/// <summary>
/// Global exception handling middleware
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
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

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        object problemDetails;
        int statusCode;

        switch (exception)
        {
            case ValidationException validationEx:
                statusCode = (int)HttpStatusCode.BadRequest;
                problemDetails = new
                {
                    status = statusCode,
                    title = "Validation Error",
                    detail = "One or more validation errors occurred",
                    errors = validationEx.Errors.GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.ErrorMessage).ToArray()
                        )
                };
                break;

            case DomainException domainEx:
                statusCode = (int)HttpStatusCode.BadRequest;
                problemDetails = new
                {
                    status = statusCode,
                    title = "Business Rule Violation",
                    detail = domainEx.Message
                };
                break;

            default:
                statusCode = (int)HttpStatusCode.InternalServerError;
                problemDetails = new
                {
                    status = statusCode,
                    title = "Internal Server Error",
                    detail = "An unexpected error occurred. Please try again later."
                };
                break;
        }

        context.Response.StatusCode = statusCode;

        var json = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
