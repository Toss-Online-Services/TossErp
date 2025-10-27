using Toss.Application.Common.Models;

namespace Toss.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<string?> GetUserNameAsync(string userId);

    Task<bool> IsInRoleAsync(string userId, string role);

    Task<bool> AuthorizeAsync(string userId, string policyName);

    Task<(Result Result, string? UserId)> CreateUserAsync(string userName, string password);
    
    Task<(Result Result, string? UserId)> CreateUserAsync(
        string userName, 
        string? email, 
        string password,
        string? phoneNumber = null,
        string? firstName = null,
        string? lastName = null);

    Task<Result> DeleteUserAsync(string userId);
    
    Task<Result> AddToRoleAsync(string userId, string role);
    
    Task<string> GenerateTokenAsync(string userId);
    
    Task<string?> GetUserByEmailAsync(string email);
    
    Task<string?> GetUserByPhoneAsync(string phone);
}
