using Microsoft.Extensions.Logging;
using Toss.Application.Common.Interfaces;

namespace Toss.Application.Audit.Commands.LogAuditEvent;

public record LogAuditEventCommand : IRequest<string>
{
    public string Action { get; init; } = string.Empty;
    public string Severity { get; init; } = string.Empty;
    public string? UserId { get; init; }
    public string? UserEmail { get; init; }
    public string? IpAddress { get; init; }
    public string? UserAgent { get; init; }
    public string? Resource { get; init; }
    public string? ResourceId { get; init; }
    public Dictionary<string, object>? Details { get; init; }
    public bool Success { get; init; } = true;
    public string? ErrorMessage { get; init; }
}

public class LogAuditEventCommandHandler : IRequestHandler<LogAuditEventCommand, string>
{
    private readonly ILogger<LogAuditEventCommandHandler> _logger;

    public LogAuditEventCommandHandler(ILogger<LogAuditEventCommandHandler> logger)
    {
        _logger = logger;
    }

    public async Task<string> Handle(LogAuditEventCommand request, CancellationToken cancellationToken)
    {
        // TODO: Store audit events in database (create AuditLog entity)
        // For now, log to application logger
        
        var auditId = Guid.NewGuid().ToString();
        
        var logMessage = $"AUDIT [{request.Severity}] {request.Action} | " +
                        $"User: {request.UserId ?? "Anonymous"} ({request.UserEmail}) | " +
                        $"Resource: {request.Resource}/{request.ResourceId} | " +
                        $"Success: {request.Success} | " +
                        $"IP: {request.IpAddress}";

        switch (request.Severity.ToLower())
        {
            case "critical":
                _logger.LogCritical("AuditId: {AuditId} | {Message} | Details: {@Details} | Error: {Error}",
                    auditId, logMessage, request.Details, request.ErrorMessage);
                break;
            case "error":
                _logger.LogError("AuditId: {AuditId} | {Message} | Details: {@Details} | Error: {Error}",
                    auditId, logMessage, request.Details, request.ErrorMessage);
                break;
            case "warning":
                _logger.LogWarning("AuditId: {AuditId} | {Message} | Details: {@Details}",
                    auditId, logMessage, request.Details);
                break;
            default:
                _logger.LogInformation("AuditId: {AuditId} | {Message} | Details: {@Details}",
                    auditId, logMessage, request.Details);
                break;
        }

        // In production, this would:
        // 1. Store in AuditLogs table
        // 2. Send critical events to monitoring service (e.g., Sentry)
        // 3. Trigger alerts for security events

        await Task.CompletedTask; // Placeholder for async database operations

        return auditId;
    }
}

