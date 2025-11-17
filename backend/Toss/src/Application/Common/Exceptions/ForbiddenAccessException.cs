namespace Toss.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when an authenticated user attempts to access a resource they don't have permission for.
/// </summary>
/// <remarks>
/// This exception is thrown by <see cref="Behaviours.AuthorizationBehaviour{TRequest, TResponse}"/> when:
/// <list type="bullet">
/// <item><description>User lacks required role(s) specified in [Authorize(Roles = "...")] attribute</description></item>
/// <item><description>User doesn't meet policy requirements specified in [Authorize(Policy = "...")] attribute</description></item>
/// </list>
/// This typically maps to HTTP 403 Forbidden in API responses.
/// Note: Use <see cref="UnauthorizedAccessException"/> for unauthenticated users (HTTP 401).
/// </remarks>
public class ForbiddenAccessException : Exception
{
    /// <summary>
    /// Initializes a new instance of <see cref="ForbiddenAccessException"/> with a default message.
    /// </summary>
    public ForbiddenAccessException() : base() { }
}
