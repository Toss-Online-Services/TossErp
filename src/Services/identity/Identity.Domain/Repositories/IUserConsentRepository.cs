namespace Identity.Domain.Repositories;

public interface IUserConsentRepository
{
    Task<UserConsent?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserConsent?> GetActiveConsentAsync(Guid userId, string consentType, string tenantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserConsent>> GetUserConsentsAsync(Guid userId, string tenantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserConsent>> GetConsentsByTypeAsync(string consentType, string tenantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserConsent>> GetExpiredConsentsAsync(string tenantId, CancellationToken cancellationToken = default);
    Task AddAsync(UserConsent consent, CancellationToken cancellationToken = default);
    Task UpdateAsync(UserConsent consent, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> HasActiveConsentAsync(Guid userId, string consentType, string tenantId, CancellationToken cancellationToken = default);
}
