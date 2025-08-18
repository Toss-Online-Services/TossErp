using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;

namespace TossErp.Identity.Services;

/// <summary>
/// In-memory implementation of refresh token repository for development/testing
/// In production, this should be replaced with a database implementation
/// </summary>
public class InMemoryRefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ConcurrentDictionary<string, RefreshTokenEntity> _tokens = new();
    private readonly ILogger<InMemoryRefreshTokenRepository> _logger;

    public InMemoryRefreshTokenRepository(ILogger<InMemoryRefreshTokenRepository> logger)
    {
        _logger = logger;
    }

    public Task StoreRefreshTokenAsync(Guid userId, string token, DateTime expiresAt, CancellationToken cancellationToken = default)
    {
        var tokenHash = ComputeHash(token);
        var tokenEntity = new RefreshTokenEntity(
            Guid.NewGuid(),
            userId,
            tokenHash,
            expiresAt,
            DateTime.UtcNow,
            null
        );

        _tokens[tokenHash] = tokenEntity;
        
        _logger.LogDebug("Stored refresh token for user {UserId}", userId);
        
        return Task.CompletedTask;
    }

    public Task<RefreshTokenEntity?> GetRefreshTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        var tokenHash = ComputeHash(token);
        
        _tokens.TryGetValue(tokenHash, out var tokenEntity);
        
        if (tokenEntity != null)
        {
            _logger.LogDebug("Retrieved refresh token for user {UserId}", tokenEntity.UserId);
        }
        else
        {
            _logger.LogDebug("Refresh token not found");
        }

        return Task.FromResult(tokenEntity);
    }

    public Task RevokeRefreshTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        var tokenHash = ComputeHash(token);
        
        if (_tokens.TryGetValue(tokenHash, out var existingToken))
        {
            var revokedToken = existingToken with { RevokedAt = DateTime.UtcNow };
            _tokens[tokenHash] = revokedToken;
            
            _logger.LogDebug("Revoked refresh token for user {UserId}", existingToken.UserId);
        }
        else
        {
            _logger.LogDebug("Attempted to revoke non-existent refresh token");
        }

        return Task.CompletedTask;
    }

    private static string ComputeHash(string input)
    {
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(hashBytes);
    }
}
