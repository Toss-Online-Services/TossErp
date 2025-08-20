namespace Identity.Application.DTOs;

public record AuditTrailDto(
    Guid Id,
    Guid UserId,
    string Action,
    string EntityType,
    string EntityId,
    string? OldValues,
    string? NewValues,
    string IpAddress,
    string UserAgent,
    string TenantId,
    DateTime Timestamp,
    string? SessionId,
    string? CorrelationId);

public record CreateAuditTrailDto(
    Guid UserId,
    string Action,
    string EntityType,
    string EntityId,
    string IpAddress,
    string UserAgent,
    string TenantId,
    string? OldValues = null,
    string? NewValues = null,
    string? SessionId = null,
    string? CorrelationId = null);

public record AuditTrailFilterDto(
    Guid? UserId = null,
    string? Action = null,
    string? EntityType = null,
    string? EntityId = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    string? SessionId = null,
    string? CorrelationId = null,
    int Page = 1,
    int PageSize = 50);
