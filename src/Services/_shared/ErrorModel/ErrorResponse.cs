using Microsoft.AspNetCore.Mvc;

namespace TossErp.ErrorModel;

/// <summary>
/// Standardized error response model
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Error code for categorization
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable error message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Additional error details
    /// </summary>
    public object? Details { get; set; }

    /// <summary>
    /// Correlation ID for tracing
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// Timestamp when the error occurred
    /// </summary>
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Convert to ProblemDetails
    /// </summary>
    public ProblemDetails ToProblemDetails(int statusCode = 400)
    {
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = Code,
            Detail = Message,
            Instance = CorrelationId
        };

        if (Details != null)
        {
            problemDetails.Extensions["details"] = Details;
        }

        problemDetails.Extensions["timestamp"] = Timestamp;

        return problemDetails;
    }
}

/// <summary>
/// Validation error response with field-specific errors
/// </summary>
public class ValidationErrorResponse : ErrorResponse
{
    /// <summary>
    /// Field-specific validation errors
    /// </summary>
    public IDictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();

    public ValidationErrorResponse()
    {
        Code = "VALIDATION_FAILED";
        Message = "One or more validation errors occurred.";
    }

    /// <summary>
    /// Convert to ValidationProblemDetails
    /// </summary>
    public ValidationProblemDetails ToValidationProblemDetails()
    {
        return new ValidationProblemDetails(Errors)
        {
            Status = 400,
            Title = Code,
            Detail = Message,
            Instance = CorrelationId
        };
    }
}
