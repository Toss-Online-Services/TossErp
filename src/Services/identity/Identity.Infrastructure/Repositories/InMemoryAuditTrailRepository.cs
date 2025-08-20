using Identity.Domain.Entities;
using Identity.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Identity.Infrastructure.Repositories;

public class InMemoryAuditTrailRepository : IAuditTrailRepository
{
    private readonly Dictionary<Guid, AuditTrail> _auditTrails = new();
    private readonly ILogger<InMemoryAuditTrailRepository> _logger;

    public InMemoryAuditTrailRepository(ILogger<InMemoryAuditTrailRepository> logger)
    {
        _logger = logger;
    }

    public Task<AuditTrail?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _auditTrails.TryGetValue(id, out var auditTrail);
        return Task.FromResult(auditTrail);
    }

    public Task<IEnumerable<AuditTrail>> GetUserAuditTrailAsync(Guid userId, string tenantId, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default)
    {
        var query = _auditTrails.Values.Where(at => at.TenantId == tenantId);

        if (userId != Guid.Empty)
        {
            query = query.Where(at => at.UserId == userId);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(at => at.Timestamp >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(at => at.Timestamp <= toDate.Value);
        }

        var auditTrails = query.OrderByDescending(at => at.Timestamp);
        return Task.FromResult(auditTrails);
    }

    public Task<IEnumerable<AuditTrail>> GetEntityAuditTrailAsync(string entityType, string entityId, string tenantId, CancellationToken cancellationToken = default)
    {
        var auditTrails = _auditTrails.Values.Where(at => 
            at.EntityType == entityType && 
            at.EntityId == entityId && 
            at.TenantId == tenantId)
            .OrderByDescending(at => at.Timestamp);
        
        return Task.FromResult(auditTrails);
    }

    public Task<IEnumerable<AuditTrail>> GetAuditTrailByActionAsync(string action, string tenantId, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default)
    {
        var query = _auditTrails.Values.Where(at => 
            at.Action == action && 
            at.TenantId == tenantId);

        if (fromDate.HasValue)
        {
            query = query.Where(at => at.Timestamp >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(at => at.Timestamp <= toDate.Value);
        }

        var auditTrails = query.OrderByDescending(at => at.Timestamp);
        return Task.FromResult(auditTrails);
    }

    public Task<IEnumerable<AuditTrail>> GetAuditTrailBySessionAsync(string sessionId, string tenantId, CancellationToken cancellationToken = default)
    {
        var auditTrails = _auditTrails.Values.Where(at => 
            at.SessionId == sessionId && 
            at.TenantId == tenantId)
            .OrderByDescending(at => at.Timestamp);
        
        return Task.FromResult(auditTrails);
    }

    public Task<IEnumerable<AuditTrail>> GetAuditTrailByCorrelationAsync(string correlationId, string tenantId, CancellationToken cancellationToken = default)
    {
        var auditTrails = _auditTrails.Values.Where(at => 
            at.CorrelationId == correlationId && 
            at.TenantId == tenantId)
            .OrderByDescending(at => at.Timestamp);
        
        return Task.FromResult(auditTrails);
    }

    public Task AddAsync(AuditTrail auditTrail, CancellationToken cancellationToken = default)
    {
        _auditTrails[auditTrail.Id] = auditTrail;
        _logger.LogDebug("Added audit trail {AuditTrailId} for user {UserId}, action {Action}", 
            auditTrail.Id, auditTrail.UserId, auditTrail.Action);
        return Task.CompletedTask;
    }

    public Task AddRangeAsync(IEnumerable<AuditTrail> auditTrails, CancellationToken cancellationToken = default)
    {
        foreach (var auditTrail in auditTrails)
        {
            _auditTrails[auditTrail.Id] = auditTrail;
        }
        
        _logger.LogDebug("Added {Count} audit trail entries", auditTrails.Count());
        return Task.CompletedTask;
    }

    public Task<int> GetAuditTrailCountAsync(string tenantId, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default)
    {
        var query = _auditTrails.Values.Where(at => at.TenantId == tenantId);

        if (fromDate.HasValue)
        {
            query = query.Where(at => at.Timestamp >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(at => at.Timestamp <= toDate.Value);
        }

        var count = query.Count();
        return Task.FromResult(count);
    }
}
