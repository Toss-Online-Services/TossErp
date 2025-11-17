using Toss.Application.Common.Models;

namespace Toss.Application.Common.Interfaces;

/// <summary>
/// Service for authentication, authorization, and user management operations.
/// Abstracts ASP.NET Core Identity from the Application layer.
/// </summary>
/// <remarks>
/// Provides essential identity operations:
/// <list type="bullet">
/// <item><description>User creation with flexible parameters</description></item>
/// <item><description>Role-based and policy-based authorization checks</description></item>
/// <item><description>JWT token generation for authentication</description></item>
/// <item><description>User lookup by email, phone, or username</description></item>
/// <item><description>User deletion and role assignment</description></item>
/// </list>
/// </remarks>
public interface IIdentityService
{
    /// <summary>
    /// Gets the username for a given user ID.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>Username if found, null otherwise.</returns>
    Task<string?> GetUserNameAsync(string userId);

    /// <summary>
    /// Checks if a user belongs to a specific role.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="role">The role name to check.</param>
    /// <returns>True if user has the role, false otherwise.</returns>
    Task<bool> IsInRoleAsync(string userId, string role);

    /// <summary>
    /// Checks if a user meets the requirements of a specific authorization policy.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="policyName">The name of the authorization policy to evaluate.</param>
    /// <returns>True if user is authorized for the policy, false otherwise.</returns>
    Task<bool> AuthorizeAsync(string userId, string policyName);

    /// <summary>
    /// Creates a new user with minimal information (username and password).
    /// </summary>
    /// <param name="userName">The username for the new user.</param>
    /// <param name="password">The password for the new user.</param>
    /// <returns>A tuple containing the operation result and the new user's ID if successful.</returns>
    Task<(Result Result, string? UserId)> CreateUserAsync(string userName, string password);
    
    /// <summary>
    /// Creates a new user with comprehensive information including email, phone, and name.
    /// </summary>
    /// <param name="userName">The username for the new user.</param>
    /// <param name="email">The email address for the new user.</param>
    /// <param name="password">The password for the new user.</param>
    /// <param name="phoneNumber">Optional phone number for the new user.</param>
    /// <param name="firstName">Optional first name for the new user.</param>
    /// <param name="lastName">Optional last name for the new user.</param>
    /// <returns>A tuple containing the operation result and the new user's ID if successful.</returns>
    Task<(Result Result, string? UserId)> CreateUserAsync(
        string userName, 
        string? email, 
        string password,
        string? phoneNumber = null,
        string? firstName = null,
        string? lastName = null);

    /// <summary>
    /// Permanently deletes a user from the system.
    /// </summary>
    /// <param name="userId">The unique identifier of the user to delete.</param>
    /// <returns>A result indicating success or failure with error messages.</returns>
    Task<Result> DeleteUserAsync(string userId);
    
    /// <summary>
    /// Assigns a role to a user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="role">The role name to assign.</param>
    /// <returns>A result indicating success or failure with error messages.</returns>
    Task<Result> AddToRoleAsync(string userId, string role);
    
    /// <summary>
    /// Generates a JWT authentication token for a user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A JWT token string for use in bearer authentication.</returns>
    Task<string> GenerateTokenAsync(string userId);
    
    /// <summary>
    /// Finds a user by their email address.
    /// </summary>
    /// <param name="email">The email address to search for.</param>
    /// <returns>The user ID if found, null otherwise.</returns>
    Task<string?> GetUserByEmailAsync(string email);
    
    /// <summary>
    /// Finds a user by their phone number.
    /// </summary>
    /// <param name="phone">The phone number to search for.</param>
    /// <returns>The user ID if found, null otherwise.</returns>
    Task<string?> GetUserByPhoneAsync(string phone);
}
