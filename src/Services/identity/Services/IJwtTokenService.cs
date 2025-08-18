namespace TossErp.Identity.Services;

public interface IJwtTokenService
{
    Task<TokenResponse> GenerateTokensAsync(User user, CancellationToken cancellationToken = default);
    Task<TokenResponse> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task RevokeTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<bool> ValidateTokenAsync(string token, CancellationToken cancellationToken = default);
}

public record TokenResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    string TokenType = "Bearer"
);

public record User(
    Guid Id,
    string TenantId,
    string Email,
    string FirstName,
    string LastName,
    IReadOnlyList<string> Roles
);
