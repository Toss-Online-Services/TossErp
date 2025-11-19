namespace Toss.Application.Common.Interfaces;

/// <summary>
/// Represents the currently authenticated user in the application context.
/// </summary>
/// <remarks>
/// This abstraction allows the Application layer to access current user information
/// without coupling to ASP.NET Core or any specific authentication framework.
/// Typically implemented by infrastructure to extract claims from HTTP context.
/// </remarks>
public interface IUser
{
    /// <summary>
    /// Gets the unique identifier of the currently authenticated user.
    /// </summary>
    /// <value>User ID if authenticated, null if anonymous.</value>
    string? Id { get; }
    
    /// <summary>
    /// Gets the list of roles assigned to the currently authenticated user.
    /// </summary>
    /// <value>List of role names if authenticated, null if anonymous.</value>
    List<string>? Roles { get; }

}
