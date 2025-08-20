namespace Identity.Domain.Repositories;

public interface IAuditTrailRepository
{
    Task<AuditTrail?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditTrail>> GetUserAuditTrailAsync(Guid userId, string tenantId, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditTrail>> GetEntityAuditTrailAsync(string entityType, string entityId, string tenantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditTrail>> GetAuditTrailByActionAsync(string action, string tenantId, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditTrail>> GetAuditTrailBySessionAsync(string sessionId, string tenantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditTrail>> GetAuditTrailByCorrelationAsync(string correlationId, string tenantId, CancellationToken cancellationToken = default);
    Task AddAsync(AuditTrail auditTrail, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<AuditTrail> auditTrails, CancellationToken cancellationToken = default);
    Task<int> GetAuditTrailCountAsync(string tenantId, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default);
}
