namespace Toss.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when a request is malformed or contains invalid data that cannot be processed.
/// </summary>
/// <remarks>
/// Use this exception for:
/// <list type="bullet">
/// <item><description>Invalid request parameters or payload format</description></item>
/// <item><description>Business rule violations that prevent processing</description></item>
/// <item><description>Data integrity issues that make the request invalid</description></item>
/// </list>
/// This typically maps to HTTP 400 Bad Request in API responses.
/// </remarks>
public class BadRequestException : Exception
{
    /// <summary>
    /// Initializes a new instance of <see cref="BadRequestException"/> with a default message.
    /// </summary>
    public BadRequestException()
        : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="BadRequestException"/> with a specific error message.
    /// </summary>
    /// <param name="message">The error message explaining why the request is invalid.</param>
    public BadRequestException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="BadRequestException"/> with a specific error message and inner exception.
    /// </summary>
    /// <param name="message">The error message explaining why the request is invalid.</param>
    /// <param name="innerException">The exception that caused this bad request exception.</param>
    public BadRequestException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

