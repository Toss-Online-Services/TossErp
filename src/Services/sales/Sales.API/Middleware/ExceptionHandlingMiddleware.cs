using System.Net;
using System.Text.Json;
using TossErp.Sales.Application.Common.Exceptions;
using TossErp.Sales.Application.Common.Models;

namespace TossErp.Sales.API.Middleware;

/// <summary>
/// Middleware for handling exceptions and returning standardized error responses
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = context.TraceIdentifier;
        ErrorResponse errorResponse;

        switch (exception)
        {
            case SalesDomainException domainEx:
                _logger.LogWarning(domainEx, "Domain exception occurred: {ErrorCode} - {Message}", 
                    domainEx.ErrorCode, domainEx.Message);
                errorResponse = ErrorResponse.Create(domainEx.ErrorCode, domainEx.Message, 
                    _environment.IsDevelopment() ? domainEx.StackTrace : null, traceId);
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;

            case ValidationException validationEx:
                _logger.LogWarning(validationEx, "Validation exception occurred");
                var validationErrors = validationEx.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => (object)g.Select(e => e.ErrorMessage).ToList());
                errorResponse = ErrorResponse.CreateValidationError(validationErrors, traceId);
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;

            case ArgumentException argEx:
                _logger.LogWarning(argEx, "Argument exception occurred: {Message}", argEx.Message);
                errorResponse = ErrorResponse.Create("SALES-1500", argEx.Message, 
                    _environment.IsDevelopment() ? argEx.StackTrace : null, traceId);
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;

            case InvalidOperationException invalidOpEx:
                _logger.LogWarning(invalidOpEx, "Invalid operation exception occurred: {Message}", invalidOpEx.Message);
                errorResponse = ErrorResponse.Create("SALES-1500", invalidOpEx.Message, 
                    _environment.IsDevelopment() ? invalidOpEx.StackTrace : null, traceId);
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;

            case UnauthorizedAccessException unauthorizedEx:
                _logger.LogWarning(unauthorizedEx, "Unauthorized access exception occurred: {Message}", unauthorizedEx.Message);
                errorResponse = ErrorResponse.Create("SALES-1603", "Unauthorized access", 
                    _environment.IsDevelopment() ? unauthorizedEx.StackTrace : null, traceId);
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                break;

            default:
                _logger.LogError(exception, "Unhandled exception occurred: {Message}", exception.Message);
                errorResponse = ErrorResponse.Create("SALES-1601", "An unexpected error occurred", 
                    _environment.IsDevelopment() ? exception.StackTrace : null, traceId);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                break;
        }

        context.Response.ContentType = "application/json";
        var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}
