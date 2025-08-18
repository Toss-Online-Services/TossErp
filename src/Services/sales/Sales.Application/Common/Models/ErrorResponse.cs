namespace TossErp.Sales.Application.Common.Models;

/// <summary>
/// Standardized error response for the Sales API
/// </summary>
public class ErrorResponse
{
    public string ErrorCode { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? TraceId { get; set; }
    public Dictionary<string, object>? ValidationErrors { get; set; }

    public ErrorResponse()
    {
    }

    public ErrorResponse(string errorCode, string message, string? details = null, string? traceId = null)
    {
        ErrorCode = errorCode;
        Message = message;
        Details = details;
        TraceId = traceId;
    }

    public static ErrorResponse Create(string errorCode, string message, string? details = null, string? traceId = null)
    {
        return new ErrorResponse(errorCode, message, details, traceId);
    }

    public static ErrorResponse CreateValidationError(Dictionary<string, object> validationErrors, string? traceId = null)
    {
        return new ErrorResponse
        {
            ErrorCode = "SALES-1500",
            Message = "Validation failed",
            Details = "One or more validation errors occurred",
            TraceId = traceId,
            ValidationErrors = validationErrors
        };
    }
}
