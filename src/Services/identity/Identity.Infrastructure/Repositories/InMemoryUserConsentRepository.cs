using Identity.Domain.Entities;
using Identity.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Identity.Infrastructure.Repositories;

public class InMemoryUserConsentRepository : IUserConsentRepository
{
    private readonly Dictionary<Guid, UserConsent> _consents = new();
    private readonly ILogger<InMemoryUserConsentRepository> _logger;

    public InMemoryUserConsentRepository(ILogger<InMemoryUserConsentRepository> logger)
    {
        _logger = logger;
    }

    public Task<UserConsent?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _consents.TryGetValue(id, out var consent);
        return Task.FromResult(consent);
    }

    public Task<UserConsent?> GetActiveConsentAsync(Guid userId, string consentType, string tenantId, CancellationToken cancellationToken = default)
    {
        var consent = _consents.Values.FirstOrDefault(c => 
            c.UserId == userId && 
            c.ConsentType == consentType && 
            c.TenantId == tenantId && 
            c.IsActive);
        
        return Task.FromResult(consent);
    }

    public Task<IEnumerable<UserConsent>> GetUserConsentsAsync(Guid userId, string tenantId, CancellationToken cancellationToken = default)
    {
        var consents = _consents.Values.Where(c => 
            c.UserId == userId && 
            c.TenantId == tenantId)
            .OrderByDescending(c => c.GrantedAt);
        
        return Task.FromResult(consents);
    }

    public Task<IEnumerable<UserConsent>> GetConsentsByTypeAsync(string consentType, string tenantId, CancellationToken cancellationToken = default)
    {
        var consents = _consents.Values.Where(c => 
            c.ConsentType == consentType && 
            c.TenantId == tenantId)
            .OrderByDescending(c => c.GrantedAt);
        
        return Task.FromResult(consents);
    }

    public Task<IEnumerable<UserConsent>> GetExpiredConsentsAsync(string tenantId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var expiredConsents = _consents.Values.Where(c => 
            c.TenantId == tenantId && 
            c.ExpiresAt != DateTime.MinValue && 
            c.ExpiresAt < now);
        
        return Task.FromResult(expiredConsents);
    }

    public Task AddAsync(UserConsent consent, CancellationToken cancellationToken = default)
    {
        _consents[consent.Id] = consent;
        _logger.LogDebug("Added consent {ConsentId} for user {UserId}", consent.Id, consent.UserId);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(UserConsent consent, CancellationToken cancellationToken = default)
    {
        if (_consents.ContainsKey(consent.Id))
        {
            _consents[consent.Id] = consent;
            _logger.LogDebug("Updated consent {ConsentId} for user {UserId}", consent.Id, consent.UserId);
        }
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (_consents.Remove(id))
        {
            _logger.LogDebug("Deleted consent {ConsentId}", id);
        }
        return Task.CompletedTask;
    }

    public Task<bool> HasActiveConsentAsync(Guid userId, string consentType, string tenantId, CancellationToken cancellationToken = default)
    {
        var hasActive = _consents.Values.Any(c => 
            c.UserId == userId && 
            c.ConsentType == consentType && 
            c.TenantId == tenantId && 
            c.IsActive);
        
        return Task.FromResult(hasActive);
    }
}
