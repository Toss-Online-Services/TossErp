using Microsoft.Extensions.Logging;
using Setup.Application.Common.Interfaces;
using Setup.Domain.ValueObjects;
using System.Text.Json;

namespace Setup.Infrastructure.Services;

public class AuditService : IAuditService
{
    private readonly ISetupUnitOfWork _unitOfWork;
    private readonly ILogger<AuditService> _logger;

    public AuditService(ISetupUnitOfWork unitOfWork, ILogger<AuditService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task LogEventAsync(string entityType, string entityId, string action, string? userId = null,
        object? oldValue = null, object? newValue = null, Dictionary<string, object>? metadata = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Logging audit event: {EntityType} {EntityId} {Action}", entityType, entityId, action);

            var auditEntry = new AuditEntry
            {
                Id = Guid.NewGuid().ToString(),
                EntityType = entityType,
                EntityId = entityId,
                Action = action,
                UserId = userId ?? "system",
                Timestamp = DateTime.UtcNow,
                OldValue = oldValue != null ? JsonSerializer.Serialize(oldValue) : null,
                NewValue = newValue != null ? JsonSerializer.Serialize(newValue) : null,
                Metadata = metadata ?? new Dictionary<string, object>(),
                IpAddress = GetCurrentIpAddress(),
                UserAgent = GetCurrentUserAgent(),
                TenantId = GetCurrentTenantId(),
                SessionId = GetCurrentSessionId(),
                CorrelationId = GetCurrentCorrelationId(),
                Severity = GetAuditSeverity(action),
                Source = "Setup.Infrastructure",
                Tags = GenerateAuditTags(entityType, action),
                IsProcessed = false,
                ProcessedAt = null,
                RetentionDate = CalculateRetentionDate(action)
            };

            await _unitOfWork.SystemConfigRepository.AddAuditEntryAsync(auditEntry, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogDebug("Audit event logged successfully: {AuditId}", auditEntry.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging audit event for {EntityType} {EntityId}", entityType, entityId);
            // Don't throw - audit logging failures shouldn't break business operations
        }
    }

    public async Task<IEnumerable<AuditEntry>> GetAuditTrailAsync(string entityType, string entityId,
        DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Retrieving audit trail for {EntityType} {EntityId}", entityType, entityId);

            fromDate ??= DateTime.UtcNow.AddDays(-30);
            toDate ??= DateTime.UtcNow;

            return await _unitOfWork.SystemConfigRepository.GetAuditTrailAsync(
                entityType, entityId, fromDate.Value, toDate.Value, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving audit trail for {EntityType} {EntityId}", entityType, entityId);
            return Enumerable.Empty<AuditEntry>();
        }
    }

    public async Task<IEnumerable<AuditEntry>> SearchAuditLogsAsync(AuditSearchCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Searching audit logs with criteria: {Criteria}", JsonSerializer.Serialize(criteria));

            return await _unitOfWork.SystemConfigRepository.SearchAuditLogsAsync(criteria, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching audit logs");
            return Enumerable.Empty<AuditEntry>();
        }
    }

    public async Task<AuditStatistics> GetAuditStatisticsAsync(DateTime? fromDate = null, DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            fromDate ??= DateTime.UtcNow.AddDays(-30);
            toDate ??= DateTime.UtcNow;

            _logger.LogDebug("Calculating audit statistics from {FromDate} to {ToDate}", fromDate, toDate);

            var statistics = await _unitOfWork.SystemConfigRepository.GetAuditStatisticsAsync(
                fromDate.Value, toDate.Value, cancellationToken);

            return new AuditStatistics
            {
                TotalEvents = statistics.TotalEvents,
                EventsByAction = statistics.EventsByAction,
                EventsByEntityType = statistics.EventsByEntityType,
                EventsByUser = statistics.EventsByUser,
                EventsByDay = statistics.EventsByDay,
                TopUsers = statistics.TopUsers,
                TopEntityTypes = statistics.TopEntityTypes,
                CriticalEvents = statistics.CriticalEvents,
                WarningEvents = statistics.WarningEvents,
                InfoEvents = statistics.InfoEvents,
                PeriodStart = fromDate.Value,
                PeriodEnd = toDate.Value
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating audit statistics");
            return new AuditStatistics();
        }
    }

    public async Task<bool> ArchiveOldAuditLogsAsync(DateTime cutoffDate, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Archiving audit logs older than {CutoffDate}", cutoffDate);

            var archivedCount = await _unitOfWork.SystemConfigRepository.ArchiveOldAuditLogsAsync(cutoffDate, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully archived {Count} audit log entries", archivedCount);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error archiving old audit logs");
            return false;
        }
    }

    public async Task<bool> PurgeArchivedAuditLogsAsync(DateTime cutoffDate, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Purging archived audit logs older than {CutoffDate}", cutoffDate);

            var purgedCount = await _unitOfWork.SystemConfigRepository.PurgeArchivedAuditLogsAsync(cutoffDate, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully purged {Count} archived audit log entries", purgedCount);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error purging archived audit logs");
            return false;
        }
    }

    public async Task<bool> ExportAuditLogsAsync(string filePath, AuditSearchCriteria criteria,
        string format = "json", CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Exporting audit logs to {FilePath} in {Format} format", filePath, format);

            var auditLogs = await SearchAuditLogsAsync(criteria, cancellationToken);

            switch (format.ToLowerInvariant())
            {
                case "json":
                    await ExportToJsonAsync(filePath, auditLogs, cancellationToken);
                    break;
                case "csv":
                    await ExportToCsvAsync(filePath, auditLogs, cancellationToken);
                    break;
                default:
                    throw new ArgumentException($"Unsupported export format: {format}");
            }

            _logger.LogInformation("Successfully exported {Count} audit log entries", auditLogs.Count());
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting audit logs to {FilePath}", filePath);
            return false;
        }
    }

    public async Task ProcessUnprocessedAuditEntriesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Processing unprocessed audit entries");

            var unprocessedEntries = await _unitOfWork.SystemConfigRepository.GetUnprocessedAuditEntriesAsync(cancellationToken);

            foreach (var entry in unprocessedEntries)
            {
                await ProcessAuditEntryAsync(entry, cancellationToken);
                
                entry.IsProcessed = true;
                entry.ProcessedAt = DateTime.UtcNow;
                _unitOfWork.SystemConfigRepository.UpdateAuditEntry(entry);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Processed {Count} audit entries", unprocessedEntries.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing unprocessed audit entries");
        }
    }

    private async Task ProcessAuditEntryAsync(AuditEntry entry, CancellationToken cancellationToken)
    {
        try
        {
            // Check for security violations
            if (IsSecurityViolation(entry))
            {
                await TriggerSecurityAlertAsync(entry, cancellationToken);
            }

            // Check for compliance requirements
            if (IsComplianceEvent(entry))
            {
                await RecordComplianceEventAsync(entry, cancellationToken);
            }

            // Check for suspicious activity patterns
            if (await IsSuspiciousActivityAsync(entry, cancellationToken))
            {
                await FlagSuspiciousActivityAsync(entry, cancellationToken);
            }

            // Update metrics
            await UpdateAuditMetricsAsync(entry, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing audit entry {EntryId}", entry.Id);
        }
    }

    private static bool IsSecurityViolation(AuditEntry entry)
    {
        var securityActions = new[] { "LOGIN_FAILED", "UNAUTHORIZED_ACCESS", "PERMISSION_DENIED", "DATA_BREACH" };
        return securityActions.Contains(entry.Action.ToUpperInvariant());
    }

    private static bool IsComplianceEvent(AuditEntry entry)
    {
        var complianceActions = new[] { "DATA_ACCESS", "DATA_EXPORT", "USER_DELETE", "TENANT_DELETE" };
        return complianceActions.Contains(entry.Action.ToUpperInvariant());
    }

    private async Task<bool> IsSuspiciousActivityAsync(AuditEntry entry, CancellationToken cancellationToken)
    {
        // Check for multiple failed attempts from same user/IP in short timeframe
        if (entry.Action.ToUpperInvariant().Contains("FAILED"))
        {
            var recentFailures = await _unitOfWork.SystemConfigRepository.GetRecentFailedAttemptsAsync(
                entry.UserId, entry.IpAddress, DateTime.UtcNow.AddMinutes(-5), cancellationToken);
            
            return recentFailures.Count() >= 5;
        }

        return false;
    }

    private async Task TriggerSecurityAlertAsync(AuditEntry entry, CancellationToken cancellationToken)
    {
        _logger.LogWarning("Security violation detected: {Action} by {UserId} from {IpAddress}", 
            entry.Action, entry.UserId, entry.IpAddress);

        var alert = new SecurityAlert
        {
            Id = Guid.NewGuid().ToString(),
            Type = "SECURITY_VIOLATION",
            Severity = "HIGH",
            Description = $"Security violation: {entry.Action}",
            UserId = entry.UserId,
            IpAddress = entry.IpAddress,
            AuditEntryId = entry.Id,
            CreatedAt = DateTime.UtcNow,
            IsResolved = false
        };

        await _unitOfWork.SystemConfigRepository.AddSecurityAlertAsync(alert, cancellationToken);
    }

    private async Task RecordComplianceEventAsync(AuditEntry entry, CancellationToken cancellationToken)
    {
        var complianceEvent = new ComplianceEvent
        {
            Id = Guid.NewGuid().ToString(),
            Type = entry.Action,
            EntityType = entry.EntityType,
            EntityId = entry.EntityId,
            UserId = entry.UserId,
            Timestamp = entry.Timestamp,
            AuditEntryId = entry.Id,
            ComplianceFramework = "GDPR", // TODO: Determine based on tenant settings
            IsCompliant = true
        };

        await _unitOfWork.SystemConfigRepository.AddComplianceEventAsync(complianceEvent, cancellationToken);
    }

    private async Task FlagSuspiciousActivityAsync(AuditEntry entry, CancellationToken cancellationToken)
    {
        _logger.LogWarning("Suspicious activity detected: {Action} by {UserId} from {IpAddress}", 
            entry.Action, entry.UserId, entry.IpAddress);

        var suspiciousActivity = new SuspiciousActivity
        {
            Id = Guid.NewGuid().ToString(),
            Type = "MULTIPLE_FAILURES",
            Description = "Multiple failed attempts detected",
            UserId = entry.UserId,
            IpAddress = entry.IpAddress,
            AuditEntryId = entry.Id,
            RiskScore = 8.5m,
            CreatedAt = DateTime.UtcNow,
            IsInvestigated = false
        };

        await _unitOfWork.SystemConfigRepository.AddSuspiciousActivityAsync(suspiciousActivity, cancellationToken);
    }

    private async Task UpdateAuditMetricsAsync(AuditEntry entry, CancellationToken cancellationToken)
    {
        // Update daily audit metrics
        var date = entry.Timestamp.Date;
        var metrics = await _unitOfWork.SystemConfigRepository.GetDailyAuditMetricsAsync(date, cancellationToken)
                     ?? new DailyAuditMetrics
                     {
                         Date = date,
                         TotalEvents = 0,
                         SecurityEvents = 0,
                         ComplianceEvents = 0,
                         ErrorEvents = 0
                     };

        metrics.TotalEvents++;

        if (IsSecurityViolation(entry))
            metrics.SecurityEvents++;

        if (IsComplianceEvent(entry))
            metrics.ComplianceEvents++;

        if (entry.Severity == "ERROR" || entry.Severity == "CRITICAL")
            metrics.ErrorEvents++;

        await _unitOfWork.SystemConfigRepository.UpsertDailyAuditMetricsAsync(metrics, cancellationToken);
    }

    private async Task ExportToJsonAsync(string filePath, IEnumerable<AuditEntry> auditLogs, 
        CancellationToken cancellationToken)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(auditLogs, options);
        await File.WriteAllTextAsync(filePath, json, cancellationToken);
    }

    private async Task ExportToCsvAsync(string filePath, IEnumerable<AuditEntry> auditLogs, 
        CancellationToken cancellationToken)
    {
        var csv = new System.Text.StringBuilder();
        csv.AppendLine("Id,EntityType,EntityId,Action,UserId,Timestamp,IpAddress,UserAgent,Severity,Source");

        foreach (var entry in auditLogs)
        {
            csv.AppendLine($"{entry.Id},{entry.EntityType},{entry.EntityId},{entry.Action}," +
                          $"{entry.UserId},{entry.Timestamp:yyyy-MM-dd HH:mm:ss},{entry.IpAddress}," +
                          $"{entry.UserAgent},{entry.Severity},{entry.Source}");
        }

        await File.WriteAllTextAsync(filePath, csv.ToString(), cancellationToken);
    }

    private static string GetCurrentIpAddress()
    {
        // TODO: Get from HTTP context
        return "127.0.0.1";
    }

    private static string GetCurrentUserAgent()
    {
        // TODO: Get from HTTP context
        return "TOSS-ERP-System";
    }

    private static string GetCurrentTenantId()
    {
        // TODO: Get from current context
        return "system";
    }

    private static string GetCurrentSessionId()
    {
        // TODO: Get from current context
        return Guid.NewGuid().ToString();
    }

    private static string GetCurrentCorrelationId()
    {
        // TODO: Get from current context
        return Guid.NewGuid().ToString();
    }

    private static string GetAuditSeverity(string action)
    {
        return action.ToUpperInvariant() switch
        {
            var a when a.Contains("DELETE") || a.Contains("REMOVE") => "CRITICAL",
            var a when a.Contains("FAILED") || a.Contains("ERROR") => "ERROR",
            var a when a.Contains("CREATE") || a.Contains("UPDATE") => "INFO",
            _ => "INFO"
        };
    }

    private static List<string> GenerateAuditTags(string entityType, string action)
    {
        var tags = new List<string> { entityType.ToLowerInvariant(), action.ToLowerInvariant() };

        if (action.Contains("CREATE"))
            tags.Add("creation");
        else if (action.Contains("UPDATE"))
            tags.Add("modification");
        else if (action.Contains("DELETE"))
            tags.Add("deletion");

        return tags;
    }

    private static DateTime CalculateRetentionDate(string action)
    {
        // Security and compliance events: 7 years
        // Regular operations: 1 year
        // Info events: 6 months
        return action.ToUpperInvariant() switch
        {
            var a when a.Contains("SECURITY") || a.Contains("COMPLIANCE") => DateTime.UtcNow.AddYears(7),
            var a when a.Contains("DELETE") || a.Contains("CREATE") => DateTime.UtcNow.AddYears(1),
            _ => DateTime.UtcNow.AddMonths(6)
        };
    }
}

// Supporting classes for audit processing
public class SecurityAlert
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string? IpAddress { get; set; }
    public string? AuditEntryId { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsResolved { get; set; }
}

public class ComplianceEvent
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public DateTime Timestamp { get; set; }
    public string? AuditEntryId { get; set; }
    public string ComplianceFramework { get; set; } = string.Empty;
    public bool IsCompliant { get; set; }
}

public class SuspiciousActivity
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string? IpAddress { get; set; }
    public string? AuditEntryId { get; set; }
    public decimal RiskScore { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsInvestigated { get; set; }
}

public class DailyAuditMetrics
{
    public DateTime Date { get; set; }
    public int TotalEvents { get; set; }
    public int SecurityEvents { get; set; }
    public int ComplianceEvents { get; set; }
    public int ErrorEvents { get; set; }
}
