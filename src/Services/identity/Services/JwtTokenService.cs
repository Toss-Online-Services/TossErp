using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TossErp.Configuration;

namespace TossErp.Identity.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions _jwtOptions;
    private readonly ILogger<JwtTokenService> _logger;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public JwtTokenService(
        IOptions<JwtOptions> jwtOptions,
        ILogger<JwtTokenService> logger,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _jwtOptions = jwtOptions.Value;
        _logger = logger;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<TokenResponse> GenerateTokensAsync(User user, CancellationToken cancellationToken = default)
    {
        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken();
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes);

        // Store refresh token
        await _refreshTokenRepository.StoreRefreshTokenAsync(
            user.Id,
            refreshToken,
            DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays),
            cancellationToken);

        _logger.LogInformation("Generated tokens for user {UserId} in tenant {TenantId}", 
            user.Id, user.TenantId);

        return new TokenResponse(accessToken, refreshToken, expiresAt);
    }

    public async Task<TokenResponse> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var storedToken = await _refreshTokenRepository.GetRefreshTokenAsync(refreshToken, cancellationToken);
        
        if (storedToken == null || storedToken.ExpiresAt <= DateTime.UtcNow || storedToken.RevokedAt.HasValue)
        {
            _logger.LogWarning("Invalid or expired refresh token");
            throw new UnauthorizedAccessException("Invalid refresh token");
        }

        // Get user details (in a real implementation, this would come from a user repository)
        var user = await GetUserByIdAsync(storedToken.UserId, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("User not found for refresh token: {UserId}", storedToken.UserId);
            throw new UnauthorizedAccessException("Invalid refresh token");
        }

        // Generate new tokens
        var newTokens = await GenerateTokensAsync(user, cancellationToken);

        // Revoke old refresh token
        await _refreshTokenRepository.RevokeRefreshTokenAsync(refreshToken, cancellationToken);

        _logger.LogInformation("Refreshed tokens for user {UserId}", user.Id);

        return newTokens;
    }

    public async Task RevokeTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        await _refreshTokenRepository.RevokeRefreshTokenAsync(refreshToken, cancellationToken);
        _logger.LogInformation("Revoked refresh token");
    }

    public async Task<bool> ValidateTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtOptions.SigningKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = _jwtOptions.ValidateIssuer,
                ValidIssuer = _jwtOptions.Issuer,
                ValidateAudience = _jwtOptions.ValidateAudience,
                ValidAudience = _jwtOptions.Audience,
                ValidateLifetime = _jwtOptions.ValidateLifetime,
                ClockSkew = TimeSpan.FromMinutes(_jwtOptions.ClockSkewMinutes)
            };

            tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Token validation failed");
            return false;
        }
    }

    private string GenerateAccessToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtOptions.SigningKey);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.GivenName, user.FirstName),
            new(ClaimTypes.Surname, user.LastName),
            new("tenant_id", user.TenantId),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        // Add role claims
        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes),
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        using var rng = RandomNumberGenerator.Create();
        var randomBytes = new byte[64];
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    // Placeholder - in real implementation, this would be injected
    private async Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        // This is a placeholder - in a real implementation, this would come from a user repository
        await Task.Delay(1, cancellationToken);
        
        return new User(
            userId,
            "dev-tenant",
            "admin@toss-erp.local",
            "System",
            "Administrator",
            new[] { "admin" }
        );
    }
}

public interface IRefreshTokenRepository
{
    Task StoreRefreshTokenAsync(Guid userId, string token, DateTime expiresAt, CancellationToken cancellationToken = default);
    Task<RefreshTokenEntity?> GetRefreshTokenAsync(string token, CancellationToken cancellationToken = default);
    Task RevokeRefreshTokenAsync(string token, CancellationToken cancellationToken = default);
}

public record RefreshTokenEntity(
    Guid Id,
    Guid UserId,
    string TokenHash,
    DateTime ExpiresAt,
    DateTime CreatedAt,
    DateTime? RevokedAt
);
