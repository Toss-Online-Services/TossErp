namespace Identity.Application.Services;

public interface IAuditService
{
    Task LogActionAsync(
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
        string? correlationId = null,
        CancellationToken cancellationToken = default);

    Task LogUserActionAsync(
        Guid userId,
        string action,
        string ipAddress,
        string userAgent,
        string tenantId,
        string? sessionId = null,
        string? correlationId = null,
        CancellationToken cancellationToken = default);
}
