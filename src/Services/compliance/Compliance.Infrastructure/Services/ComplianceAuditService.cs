using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TossErp.Compliance.Domain.Entities;
using TossErp.Compliance.Domain.Services;

namespace TossErp.Compliance.Infrastructure.Services;

/// <summary>
/// Enterprise audit and compliance system with automated monitoring and reporting
/// </summary>
public class ComplianceAuditService : IComplianceAuditService
{
    private readonly ILogger<ComplianceAuditService> _logger;
    private readonly IComplianceRepository _complianceRepository;
    private readonly IAuditEventRepository _auditRepository;
    private readonly IComplianceRuleEngine _ruleEngine;
    private readonly IComplianceNotificationService _notificationService;
    private readonly IDataRetentionService _dataRetentionService;
    private readonly IEncryptionService _encryptionService;
    private readonly Timer _complianceMonitoringTimer;
    private readonly Timer _retentionEnforcementTimer;

    public ComplianceAuditService(
        ILogger<ComplianceAuditService> logger,
        IComplianceRepository complianceRepository,
        IAuditEventRepository auditRepository,
        IComplianceRuleEngine ruleEngine,
        IComplianceNotificationService notificationService,
        IDataRetentionService dataRetentionService,
        IEncryptionService encryptionService)
    {
        _logger = logger;
        _complianceRepository = complianceRepository;
        _auditRepository = auditRepository;
        _ruleEngine = ruleEngine;
        _notificationService = notificationService;
        _dataRetentionService = dataRetentionService;
        _encryptionService = encryptionService;

        // Initialize background monitoring
        _complianceMonitoringTimer = new Timer(MonitorCompliance, null, TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(10));
        _retentionEnforcementTimer = new Timer(EnforceDataRetention, null, TimeSpan.FromHours(1), TimeSpan.FromHours(1));
    }

    public async Task<ComplianceAuditEvent> LogAuditEventAsync(
        string tenantId,
        AuditEventRequest request,
        CancellationToken cancellationToken = default)
    {
        var eventId = Guid.NewGuid().ToString();
        _logger.LogDebug("Logging audit event {EventId} for tenant {TenantId}, action {Action}", 
            eventId, tenantId, request.Action);

        try
        {
            // Create audit event
            var auditEvent = new ComplianceAuditEvent
            {
                Id = eventId,
                TenantId = tenantId,
                EventType = request.EventType,
                Action = request.Action,
                UserId = request.UserId,
                UserEmail = request.UserEmail,
                UserRole = request.UserRole,
                ResourceType = request.ResourceType,
                ResourceId = request.ResourceId,
                ResourceName = request.ResourceName,
                Timestamp = DateTime.UtcNow,
                SourceIP = request.SourceIP,
                UserAgent = request.UserAgent,
                SessionId = request.SessionId,
                CorrelationId = request.CorrelationId ?? Guid.NewGuid().ToString(),
                Severity = DetermineSeverity(request),
                ComplianceFrameworks = await GetApplicableFrameworksAsync(tenantId, request.EventType, cancellationToken),
                Metadata = request.Metadata ?? new Dictionary<string, object>(),
                Changes = request.Changes?.Select(c => new AuditDataChange
                {
                    FieldName = c.FieldName,
                    OldValue = c.OldValue != null ? _encryptionService.EncryptSensitiveData(c.OldValue.ToString()) : null,
                    NewValue = c.NewValue != null ? _encryptionService.EncryptSensitiveData(c.NewValue.ToString()) : null,
                    ChangeType = c.ChangeType
                }).ToList() ?? new List<AuditDataChange>()
            };

            // Apply compliance rules
            var ruleResults = await _ruleEngine.EvaluateEventAsync(auditEvent, cancellationToken);
            auditEvent.ComplianceStatus = DetermineComplianceStatus(ruleResults);
            auditEvent.RuleViolations = ruleResults.Where(r => !r.IsCompliant).ToList();

            // Check for suspicious activity
            auditEvent.RiskScore = await CalculateRiskScoreAsync(auditEvent, cancellationToken);
            auditEvent.IsSuspicious = auditEvent.RiskScore >= 70; // High risk threshold

            // Serialize and hash for integrity
            var eventJson = JsonSerializer.Serialize(auditEvent, new JsonSerializerOptions { WriteIndented = false });
            auditEvent.IntegrityHash = _encryptionService.GenerateHash(eventJson);

            // Save audit event
            await _auditRepository.SaveAuditEventAsync(auditEvent, cancellationToken);

            // Handle immediate compliance violations
            if (auditEvent.ComplianceStatus == ComplianceStatus.Violation)
            {
                await HandleComplianceViolationAsync(auditEvent, cancellationToken);
            }

            // Handle suspicious activity
            if (auditEvent.IsSuspicious)
            {
                await HandleSuspiciousActivityAsync(auditEvent, cancellationToken);
            }

            _logger.LogInformation("Audit event {EventId} logged successfully with compliance status {Status} and risk score {RiskScore}",
                eventId, auditEvent.ComplianceStatus, auditEvent.RiskScore);

            return auditEvent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log audit event for tenant {TenantId}, action {Action}", tenantId, request.Action);
            throw;
        }
    }

    public async Task<ComplianceReport> GenerateComplianceReportAsync(
        string tenantId,
        ComplianceReportRequest request,
        CancellationToken cancellationToken = default)
    {
        var reportId = Guid.NewGuid().ToString();
        _logger.LogInformation("Generating compliance report {ReportId} for tenant {TenantId}, framework {Framework}",
            reportId, tenantId, request.Framework);

        try
        {
            // Get audit events for the period
            var auditEvents = await _auditRepository.GetAuditEventsAsync(
                tenantId, 
                request.StartDate, 
                request.EndDate, 
                new AuditEventFilter { Framework = request.Framework },
                cancellationToken);

            // Generate framework-specific report
            var report = new ComplianceReport
            {
                Id = reportId,
                TenantId = tenantId,
                Framework = request.Framework,
                ReportType = request.ReportType,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                GeneratedAt = DateTime.UtcNow,
                GeneratedBy = request.GeneratedBy,
                TotalEvents = auditEvents.Count,
                ComplianceScore = CalculateComplianceScore(auditEvents, request.Framework),
                Status = ComplianceReportStatus.Generated
            };

            // Framework-specific analysis
            switch (request.Framework)
            {
                case ComplianceFramework.SOC2:
                    await GenerateSOC2ReportAsync(report, auditEvents, cancellationToken);
                    break;
                case ComplianceFramework.GDPR:
                    await GenerateGDPRReportAsync(report, auditEvents, cancellationToken);
                    break;
                case ComplianceFramework.HIPAA:
                    await GenerateHIPAAReportAsync(report, auditEvents, cancellationToken);
                    break;
                case ComplianceFramework.PCI_DSS:
                    await GeneratePCIDSSReportAsync(report, auditEvents, cancellationToken);
                    break;
                case ComplianceFramework.ISO27001:
                    await GenerateISO27001ReportAsync(report, auditEvents, cancellationToken);
                    break;
                default:
                    await GenerateGeneralComplianceReportAsync(report, auditEvents, cancellationToken);
                    break;
            }

            // Save report
            await _complianceRepository.SaveReportAsync(report, cancellationToken);

            // Send notifications if required
            if (request.NotifyStakeholders)
            {
                await _notificationService.SendComplianceReportNotificationAsync(report, cancellationToken);
            }

            _logger.LogInformation("Compliance report {ReportId} generated successfully with score {Score}",
                reportId, report.ComplianceScore);

            return report;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate compliance report for tenant {TenantId}", tenantId);
            throw;
        }
    }

    public async Task<List<ComplianceViolation>> GetComplianceViolationsAsync(
        string tenantId,
        ComplianceViolationFilter filter,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting compliance violations for tenant {TenantId}", tenantId);

        try
        {
            var violations = await _complianceRepository.GetViolationsAsync(tenantId, filter, cancellationToken);

            // Apply additional filtering
            if (filter.Severity.HasValue)
            {
                violations = violations.Where(v => v.Severity == filter.Severity.Value).ToList();
            }

            if (filter.Status.HasValue)
            {
                violations = violations.Where(v => v.Status == filter.Status.Value).ToList();
            }

            if (filter.Framework.HasValue)
            {
                violations = violations.Where(v => v.Framework == filter.Framework.Value).ToList();
            }

            if (!string.IsNullOrEmpty(filter.UserId))
            {
                violations = violations.Where(v => v.UserId == filter.UserId).ToList();
            }

            // Apply date filters
            if (filter.OccurredAfter.HasValue)
            {
                violations = violations.Where(v => v.OccurredAt >= filter.OccurredAfter.Value).ToList();
            }

            if (filter.OccurredBefore.HasValue)
            {
                violations = violations.Where(v => v.OccurredAt <= filter.OccurredBefore.Value).ToList();
            }

            // Apply pagination
            if (filter.Skip.HasValue)
            {
                violations = violations.Skip(filter.Skip.Value).ToList();
            }

            if (filter.Take.HasValue)
            {
                violations = violations.Take(filter.Take.Value).ToList();
            }

            return violations.OrderByDescending(v => v.OccurredAt).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get compliance violations for tenant {TenantId}", tenantId);
            throw;
        }
    }

    public async Task<ComplianceDashboard> GetComplianceDashboardAsync(
        string tenantId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting compliance dashboard for tenant {TenantId}", tenantId);

        try
        {
            var endDate = DateTime.UtcNow;
            var startDate = endDate.AddDays(-30); // Last 30 days

            // Get recent audit events
            var recentEvents = await _auditRepository.GetAuditEventsAsync(
                tenantId, 
                startDate, 
                endDate, 
                new AuditEventFilter(),
                cancellationToken);

            // Get active violations
            var activeViolations = await _complianceRepository.GetViolationsAsync(
                tenantId, 
                new ComplianceViolationFilter { Status = ViolationStatus.Active },
                cancellationToken);

            // Calculate metrics
            var dashboard = new ComplianceDashboard
            {
                TenantId = tenantId,
                PeriodStart = startDate,
                PeriodEnd = endDate,
                GeneratedAt = DateTime.UtcNow,
                TotalAuditEvents = recentEvents.Count,
                ActiveViolations = activeViolations.Count,
                OverallComplianceScore = CalculateOverallComplianceScore(recentEvents),
                RiskLevel = DetermineRiskLevel(activeViolations, recentEvents),
                FrameworkScores = new Dictionary<ComplianceFramework, decimal>(),
                RecentActivity = recentEvents.OrderByDescending(e => e.Timestamp).Take(10).ToList(),
                TopViolations = activeViolations.OrderByDescending(v => v.Severity).Take(5).ToList(),
                ComplianceTrends = await CalculateComplianceTrendsAsync(tenantId, startDate, endDate, cancellationToken)
            };

            // Calculate framework-specific scores
            foreach (var framework in Enum.GetValues<ComplianceFramework>())
            {
                var frameworkEvents = recentEvents.Where(e => e.ComplianceFrameworks.Contains(framework)).ToList();
                if (frameworkEvents.Any())
                {
                    dashboard.FrameworkScores[framework] = CalculateComplianceScore(frameworkEvents, framework);
                }
            }

            return dashboard;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get compliance dashboard for tenant {TenantId}", tenantId);
            throw;
        }
    }

    public async Task<bool> RemediateViolationAsync(
        string tenantId,
        string violationId,
        ViolationRemediationRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Remediating violation {ViolationId} for tenant {TenantId}", violationId, tenantId);

        try
        {
            var violation = await _complianceRepository.GetViolationAsync(violationId, cancellationToken);
            if (violation == null)
            {
                _logger.LogWarning("Violation {ViolationId} not found", violationId);
                return false;
            }

            if (violation.TenantId != tenantId)
            {
                throw new UnauthorizedAccessException($"Violation {violationId} does not belong to tenant {tenantId}");
            }

            // Update violation status
            violation.Status = ViolationStatus.Remediated;
            violation.RemediatedAt = DateTime.UtcNow;
            violation.RemediatedBy = request.RemediatedBy;
            violation.RemediationNotes = request.RemediationNotes;
            violation.RemediationActions = request.RemediationActions;

            // Validate remediation
            if (request.RequireValidation)
            {
                var validationResult = await ValidateRemediationAsync(violation, request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    violation.Status = ViolationStatus.RemediationFailed;
                    violation.RemediationNotes += $"\nValidation failed: {validationResult.Reason}";
                }
            }

            // Save violation
            await _complianceRepository.SaveViolationAsync(violation, cancellationToken);

            // Log remediation action
            await LogAuditEventAsync(tenantId, new AuditEventRequest
            {
                EventType = AuditEventType.ComplianceRemediation,
                Action = "ViolationRemediated",
                UserId = request.RemediatedBy,
                ResourceType = "ComplianceViolation",
                ResourceId = violationId,
                Metadata = new Dictionary<string, object>
                {
                    ["violationType"] = violation.ViolationType.ToString(),
                    ["framework"] = violation.Framework.ToString(),
                    ["remediationActions"] = request.RemediationActions
                }
            }, cancellationToken);

            _logger.LogInformation("Violation {ViolationId} remediated successfully", violationId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remediate violation {ViolationId} for tenant {TenantId}", violationId, tenantId);
            throw;
        }
    }

    private async Task<List<ComplianceFramework>> GetApplicableFrameworksAsync(
        string tenantId,
        AuditEventType eventType,
        CancellationToken cancellationToken)
    {
        // Get tenant's compliance configuration
        var config = await _complianceRepository.GetTenantConfigurationAsync(tenantId, cancellationToken);
        if (config == null) return new List<ComplianceFramework>();

        // Filter frameworks based on event type relevance
        return config.EnabledFrameworks.Where(framework => IsFrameworkApplicable(framework, eventType)).ToList();
    }

    private bool IsFrameworkApplicable(ComplianceFramework framework, AuditEventType eventType)
    {
        return framework switch
        {
            ComplianceFramework.GDPR => eventType == AuditEventType.DataAccess || 
                                       eventType == AuditEventType.DataModification || 
                                       eventType == AuditEventType.DataDeletion ||
                                       eventType == AuditEventType.UserDataExport,
            
            ComplianceFramework.HIPAA => eventType == AuditEventType.DataAccess || 
                                        eventType == AuditEventType.DataModification ||
                                        eventType == AuditEventType.SystemAccess,
            
            ComplianceFramework.SOC2 => eventType == AuditEventType.SystemAccess || 
                                       eventType == AuditEventType.ConfigurationChange ||
                                       eventType == AuditEventType.SecurityEvent,
            
            ComplianceFramework.PCI_DSS => eventType == AuditEventType.PaymentProcessing || 
                                          eventType == AuditEventType.DataAccess ||
                                          eventType == AuditEventType.SecurityEvent,
            
            _ => true // ISO27001 and others apply to most events
        };
    }

    private AuditSeverity DetermineSeverity(AuditEventRequest request)
    {
        return request.EventType switch
        {
            AuditEventType.SecurityEvent => AuditSeverity.High,
            AuditEventType.AuthenticationFailure => AuditSeverity.Medium,
            AuditEventType.ConfigurationChange => AuditSeverity.Medium,
            AuditEventType.DataDeletion => AuditSeverity.High,
            AuditEventType.SystemAccess => AuditSeverity.Low,
            AuditEventType.DataAccess => AuditSeverity.Low,
            _ => AuditSeverity.Low
        };
    }

    private ComplianceStatus DetermineComplianceStatus(List<ComplianceRuleResult> ruleResults)
    {
        if (!ruleResults.Any()) return ComplianceStatus.Compliant;
        if (ruleResults.Any(r => !r.IsCompliant && r.Severity == RuleSeverity.Critical)) return ComplianceStatus.Violation;
        if (ruleResults.Any(r => !r.IsCompliant)) return ComplianceStatus.Warning;
        return ComplianceStatus.Compliant;
    }

    private async Task<int> CalculateRiskScoreAsync(ComplianceAuditEvent auditEvent, CancellationToken cancellationToken)
    {
        int riskScore = 0;

        // Base risk by event type
        riskScore += auditEvent.EventType switch
        {
            AuditEventType.SecurityEvent => 40,
            AuditEventType.AuthenticationFailure => 30,
            AuditEventType.DataDeletion => 35,
            AuditEventType.ConfigurationChange => 25,
            AuditEventType.SystemAccess => 10,
            _ => 5
        };

        // Increase risk for repeated events from same user/IP
        var recentSimilarEvents = await _auditRepository.GetRecentSimilarEventsAsync(
            auditEvent.TenantId, 
            auditEvent.UserId, 
            auditEvent.SourceIP, 
            auditEvent.EventType,
            TimeSpan.FromHours(1),
            cancellationToken);

        if (recentSimilarEvents.Count > 5) riskScore += 30;
        else if (recentSimilarEvents.Count > 2) riskScore += 15;

        // Increase risk for compliance violations
        if (auditEvent.ComplianceStatus == ComplianceStatus.Violation) riskScore += 25;

        // Increase risk for off-hours access
        var hour = auditEvent.Timestamp.Hour;
        if (hour < 6 || hour > 22) riskScore += 10;

        return Math.Min(riskScore, 100);
    }

    private async Task HandleComplianceViolationAsync(ComplianceAuditEvent auditEvent, CancellationToken cancellationToken)
    {
        // Create violation record
        var violation = new ComplianceViolation
        {
            Id = Guid.NewGuid().ToString(),
            TenantId = auditEvent.TenantId,
            AuditEventId = auditEvent.Id,
            ViolationType = DetermineViolationType(auditEvent),
            Framework = auditEvent.ComplianceFrameworks.First(),
            Severity = MapSeverity(auditEvent.Severity),
            Description = GenerateViolationDescription(auditEvent),
            OccurredAt = auditEvent.Timestamp,
            UserId = auditEvent.UserId,
            ResourceType = auditEvent.ResourceType,
            ResourceId = auditEvent.ResourceId,
            Status = ViolationStatus.Active,
            RiskScore = auditEvent.RiskScore
        };

        await _complianceRepository.SaveViolationAsync(violation, cancellationToken);

        // Send immediate notification for critical violations
        if (violation.Severity == ViolationSeverity.Critical)
        {
            await _notificationService.SendImmediateViolationNotificationAsync(violation, cancellationToken);
        }
    }

    private async Task HandleSuspiciousActivityAsync(ComplianceAuditEvent auditEvent, CancellationToken cancellationToken)
    {
        // Log suspicious activity
        _logger.LogWarning("Suspicious activity detected: {EventType} by {UserId} from {SourceIP} with risk score {RiskScore}",
            auditEvent.EventType, auditEvent.UserId, auditEvent.SourceIP, auditEvent.RiskScore);

        // Send security alert
        await _notificationService.SendSecurityAlertAsync(auditEvent, cancellationToken);
    }

    private async Task GenerateSOC2ReportAsync(ComplianceReport report, List<ComplianceAuditEvent> auditEvents, CancellationToken cancellationToken)
    {
        var soc2Events = auditEvents.Where(e => e.ComplianceFrameworks.Contains(ComplianceFramework.SOC2)).ToList();
        
        report.Sections = new List<ComplianceReportSection>
        {
            await GenerateSecuritySection(soc2Events, cancellationToken),
            await GenerateAvailabilitySection(soc2Events, cancellationToken),
            await GenerateProcessingIntegritySection(soc2Events, cancellationToken),
            await GenerateConfidentialitySection(soc2Events, cancellationToken),
            await GeneratePrivacySection(soc2Events, cancellationToken)
        };
    }

    private async Task GenerateGDPRReportAsync(ComplianceReport report, List<ComplianceAuditEvent> auditEvents, CancellationToken cancellationToken)
    {
        var gdprEvents = auditEvents.Where(e => e.ComplianceFrameworks.Contains(ComplianceFramework.GDPR)).ToList();
        
        report.Sections = new List<ComplianceReportSection>
        {
            await GenerateDataProtectionSection(gdprEvents, cancellationToken),
            await GenerateConsentManagementSection(gdprEvents, cancellationToken),
            await GenerateDataSubjectRightsSection(gdprEvents, cancellationToken),
            await GenerateDataBreachSection(gdprEvents, cancellationToken),
            await GenerateDataRetentionSection(gdprEvents, cancellationToken)
        };
    }

    private decimal CalculateComplianceScore(List<ComplianceAuditEvent> events, ComplianceFramework framework)
    {
        if (!events.Any()) return 100m;

        var frameworkEvents = events.Where(e => e.ComplianceFrameworks.Contains(framework)).ToList();
        if (!frameworkEvents.Any()) return 100m;

        var compliantEvents = frameworkEvents.Count(e => e.ComplianceStatus == ComplianceStatus.Compliant);
        return Math.Round((decimal)compliantEvents / frameworkEvents.Count * 100, 2);
    }

    private decimal CalculateOverallComplianceScore(List<ComplianceAuditEvent> events)
    {
        if (!events.Any()) return 100m;

        var compliantEvents = events.Count(e => e.ComplianceStatus == ComplianceStatus.Compliant);
        return Math.Round((decimal)compliantEvents / events.Count * 100, 2);
    }

    private ComplianceRiskLevel DetermineRiskLevel(List<ComplianceViolation> violations, List<ComplianceAuditEvent> events)
    {
        var criticalViolations = violations.Count(v => v.Severity == ViolationSeverity.Critical);
        var highRiskEvents = events.Count(e => e.RiskScore >= 70);

        if (criticalViolations > 0 || highRiskEvents > 10) return ComplianceRiskLevel.High;
        if (violations.Count > 5 || highRiskEvents > 5) return ComplianceRiskLevel.Medium;
        return ComplianceRiskLevel.Low;
    }

    private void MonitorCompliance(object? state)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                await _ruleEngine.RunScheduledComplianceChecksAsync(CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during compliance monitoring");
            }
        });
    }

    private void EnforceDataRetention(object? state)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                await _dataRetentionService.EnforceRetentionPoliciesAsync(CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during data retention enforcement");
            }
        });
    }

    public void Dispose()
    {
        _complianceMonitoringTimer?.Dispose();
        _retentionEnforcementTimer?.Dispose();
    }
}

// Supporting enums and classes
public enum ComplianceFramework
{
    SOC2,
    GDPR,
    HIPAA,
    PCI_DSS,
    ISO27001,
    NIST,
    CCPA
}

public enum AuditEventType
{
    SystemAccess,
    DataAccess,
    DataModification,
    DataDeletion,
    ConfigurationChange,
    SecurityEvent,
    AuthenticationFailure,
    PaymentProcessing,
    UserDataExport
}

public enum ComplianceStatus
{
    Compliant,
    Warning,
    Violation
}

public enum ViolationStatus
{
    Active,
    Remediated,
    RemediationFailed,
    Acknowledged
}

public enum ComplianceRiskLevel
{
    Low,
    Medium,
    High,
    Critical
}
