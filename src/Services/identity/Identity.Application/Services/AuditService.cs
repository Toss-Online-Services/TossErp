using Identity.Application.Commands;
using Identity.Application.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Services;

public class AuditService : IAuditService
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuditService> _logger;

    public AuditService(IMediator mediator, ILogger<AuditService> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task LogActionAsync(
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
        CancellationToken cancellationToken = default)
    {
        try
        {
            var dto = new CreateAuditTrailDto(
                userId,
                action,
                entityType,
                entityId,
                ipAddress,
                userAgent,
                tenantId,
                oldValues,
                newValues,
                sessionId,
                correlationId);

            var command = new CreateAuditTrailCommand(dto);
            await _mediator.Send(command, cancellationToken);

            _logger.LogDebug("Audit trail logged: {Action} on {EntityType}:{EntityId} by user {UserId}", 
                action, entityType, entityId, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log audit trail for action {Action} by user {UserId}", 
                action, userId);
            // Don't throw - audit logging should not break the main flow
        }
    }

    public async Task LogUserActionAsync(
        Guid userId,
        string action,
        string ipAddress,
        string userAgent,
        string tenantId,
        string? sessionId = null,
        string? correlationId = null,
        CancellationToken cancellationToken = default)
    {
        await LogActionAsync(
            userId,
            action,
            "User",
            userId.ToString(),
            ipAddress,
            userAgent,
            tenantId,
            sessionId: sessionId,
            correlationId: correlationId,
            cancellationToken: cancellationToken);
    }
}
