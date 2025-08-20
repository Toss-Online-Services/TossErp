namespace Identity.Domain.Entities;

public class AuditTrail : Entity
{
    public Guid UserId { get; private set; }
    public string Action { get; private set; }
    public string EntityType { get; private set; }
    public string EntityId { get; private set; }
    public string? OldValues { get; private set; }
    public string? NewValues { get; private set; }
    public string IpAddress { get; private set; }
    public string UserAgent { get; private set; }
    public string TenantId { get; private set; }
    public DateTime Timestamp { get; private set; }
    public string? SessionId { get; private set; }
    public string? CorrelationId { get; private set; }

    public AuditTrail(
        Guid userId,
        string action,
        string entityType,
        string entityId,
        string ipAddress,
        string userAgent,
        string tenantId,
        string? oldValues = null,
        string? newValues = null,
        string? sessionId = null,
        string? correlationId = null)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Action = action ?? throw new ArgumentNullException(nameof(action));
        EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
        EntityId = entityId ?? throw new ArgumentNullException(nameof(entityId));
        OldValues = oldValues;
        NewValues = newValues;
        IpAddress = ipAddress ?? throw new ArgumentNullException(nameof(ipAddress));
        UserAgent = userAgent ?? throw new ArgumentNullException(nameof(userAgent));
        TenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
        Timestamp = DateTime.UtcNow;
        SessionId = sessionId;
        CorrelationId = correlationId;
    }

    private AuditTrail() { }
}
