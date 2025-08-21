using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TossErp.Security.Domain.Entities;
using TossErp.Security.Domain.Services;

namespace TossErp.Security.Infrastructure.Services;

/// <summary>
/// Advanced threat detection service with behavioral analysis and automated response
/// </summary>
public class ThreatDetectionService : IThreatDetectionService
{
    private readonly ILogger<ThreatDetectionService> _logger;
    private readonly IBehaviorAnalyticsService _behaviorAnalytics;
    private readonly IThreatIntelligenceService _threatIntelligence;
    private readonly ISecurityEventRepository _securityEventRepository;
    private readonly IIncidentResponseService _incidentResponse;

    // Threat detection thresholds
    private const decimal HighSeverityThreshold = 0.8m;
    private const decimal MediumSeverityThreshold = 0.5m;
    private const int MaxFailedAttemptsPerHour = 10;
    private const int MaxAnomalousActivitiesPerSession = 5;

    public ThreatDetectionService(
        ILogger<ThreatDetectionService> logger,
        IBehaviorAnalyticsService behaviorAnalytics,
        IThreatIntelligenceService threatIntelligence,
        ISecurityEventRepository securityEventRepository,
        IIncidentResponseService incidentResponse)
    {
        _logger = logger;
        _behaviorAnalytics = behaviorAnalytics;
        _threatIntelligence = threatIntelligence;
        _securityEventRepository = securityEventRepository;
        _incidentResponse = incidentResponse;
    }

    public async Task<ThreatAssessment> AnalyzeThreatAsync(
        string userId,
        SecurityEvent securityEvent,
        CancellationToken cancellationToken = default)
    {
        var assessmentId = Guid.NewGuid().ToString();
        _logger.LogInformation("Starting threat analysis for event {EventId}, User: {UserId}, Assessment: {AssessmentId}",
            securityEvent.Id, userId, assessmentId);

        var assessment = new ThreatAssessment
        {
            Id = assessmentId,
            UserId = userId,
            EventId = securityEvent.Id,
            AssessmentTime = DateTime.UtcNow,
            Threats = new List<SecurityThreat>()
        };

        try
        {
            // Step 1: Behavioral Analysis
            var behaviorThreats = await AnalyzeBehavioralThreatsAsync(userId, securityEvent, cancellationToken);
            assessment.Threats.AddRange(behaviorThreats);

            // Step 2: Pattern Analysis
            var patternThreats = await AnalyzePatternThreatsAsync(userId, securityEvent, cancellationToken);
            assessment.Threats.AddRange(patternThreats);

            // Step 3: Threat Intelligence Check
            var intelligenceThreats = await CheckThreatIntelligenceAsync(securityEvent, cancellationToken);
            assessment.Threats.AddRange(intelligenceThreats);

            // Step 4: Network Analysis
            var networkThreats = await AnalyzeNetworkThreatsAsync(securityEvent, cancellationToken);
            assessment.Threats.AddRange(networkThreats);

            // Step 5: Calculate Risk Score
            assessment.RiskScore = CalculateRiskScore(assessment.Threats);
            assessment.RiskLevel = DetermineRiskLevel(assessment.RiskScore);

            // Step 6: Generate Automated Response
            assessment.AutomatedActions = await GenerateAutomatedResponseAsync(assessment, cancellationToken);

            // Step 7: Create Security Incident if needed
            if (assessment.RiskLevel >= ThreatRiskLevel.High)
            {
                await CreateSecurityIncidentAsync(assessment, cancellationToken);
            }

            _logger.LogInformation("Threat analysis completed for assessment {AssessmentId}. Risk Level: {RiskLevel}, Score: {RiskScore}",
                assessmentId, assessment.RiskLevel, assessment.RiskScore);

            return assessment;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during threat analysis for assessment {AssessmentId}", assessmentId);
            
            // Create high-risk assessment on error for safety
            assessment.RiskLevel = ThreatRiskLevel.Critical;
            assessment.RiskScore = 1.0m;
            assessment.Threats.Add(new SecurityThreat
            {
                Id = Guid.NewGuid().ToString(),
                Type = ThreatType.SystemFailure,
                Severity = ThreatSeverity.High,
                Description = "Threat analysis system failure",
                DetectedAt = DateTime.UtcNow
            });

            return assessment;
        }
    }

    public async Task<List<BehaviorAnomaly>> DetectBehavioralAnomaliesAsync(
        string userId,
        TimeSpan lookbackPeriod,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Detecting behavioral anomalies for user {UserId} over period {Period}", userId, lookbackPeriod);

        var anomalies = new List<BehaviorAnomaly>();
        var startTime = DateTime.UtcNow.Subtract(lookbackPeriod);

        try
        {
            // Get user's recent activity
            var recentEvents = await _securityEventRepository.GetEventsByUserAsync(userId, startTime, DateTime.UtcNow, cancellationToken);
            
            // Get user's behavioral baseline
            var baseline = await _behaviorAnalytics.GetUserBaselineAsync(userId, cancellationToken);

            // Analyze login patterns
            var loginAnomalies = await AnalyzeLoginPatternsAsync(userId, recentEvents, baseline, cancellationToken);
            anomalies.AddRange(loginAnomalies);

            // Analyze access patterns
            var accessAnomalies = await AnalyzeAccessPatternsAsync(userId, recentEvents, baseline, cancellationToken);
            anomalies.AddRange(accessAnomalies);

            // Analyze temporal patterns
            var temporalAnomalies = await AnalyzeTemporalPatternsAsync(userId, recentEvents, baseline, cancellationToken);
            anomalies.AddRange(temporalAnomalies);

            // Analyze geographical patterns
            var geoAnomalies = await AnalyzeGeographicalPatternsAsync(userId, recentEvents, baseline, cancellationToken);
            anomalies.AddRange(geoAnomalies);

            _logger.LogDebug("Detected {Count} behavioral anomalies for user {UserId}", anomalies.Count, userId);
            return anomalies.OrderByDescending(a => a.Severity).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error detecting behavioral anomalies for user {UserId}", userId);
            return anomalies;
        }
    }

    public async Task<SecurityIncident> CreateSecurityIncidentAsync(
        ThreatAssessment assessment,
        CancellationToken cancellationToken = default)
    {
        var incident = new SecurityIncident
        {
            Id = Guid.NewGuid().ToString(),
            Title = GenerateIncidentTitle(assessment),
            Description = GenerateIncidentDescription(assessment),
            Severity = DetermineIncidentSeverity(assessment.RiskLevel),
            Status = IncidentStatus.Open,
            CreatedAt = DateTime.UtcNow,
            UserId = assessment.UserId,
            ThreatAssessmentId = assessment.Id,
            Threats = assessment.Threats.ToList()
        };

        try
        {
            // Save incident
            await _securityEventRepository.SaveIncidentAsync(incident, cancellationToken);

            // Trigger automated response
            await _incidentResponse.InitiateResponseAsync(incident, cancellationToken);

            _logger.LogWarning("Security incident created: {IncidentId} for user {UserId} with severity {Severity}",
                incident.Id, assessment.UserId, incident.Severity);

            return incident;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating security incident for assessment {AssessmentId}", assessment.Id);
            throw;
        }
    }

    public async Task<List<AutomatedResponse>> GenerateAutomatedResponseAsync(
        ThreatAssessment assessment,
        CancellationToken cancellationToken = default)
    {
        var responses = new List<AutomatedResponse>();

        foreach (var threat in assessment.Threats.OrderByDescending(t => t.Severity))
        {
            var response = await GenerateResponseForThreatAsync(threat, assessment, cancellationToken);
            if (response != null)
            {
                responses.Add(response);
            }
        }

        // Add general responses based on risk level
        if (assessment.RiskLevel >= ThreatRiskLevel.High)
        {
            responses.Add(new AutomatedResponse
            {
                Id = Guid.NewGuid().ToString(),
                Type = ResponseType.UserSuspension,
                Action = "Suspend user account pending investigation",
                Priority = ResponsePriority.High,
                IsAutoExecuted = true,
                ExecuteAt = DateTime.UtcNow
            });
        }

        if (assessment.RiskLevel >= ThreatRiskLevel.Medium)
        {
            responses.Add(new AutomatedResponse
            {
                Id = Guid.NewGuid().ToString(),
                Type = ResponseType.EnhancedMonitoring,
                Action = "Enable enhanced monitoring for user activities",
                Priority = ResponsePriority.Medium,
                IsAutoExecuted = true,
                ExecuteAt = DateTime.UtcNow
            });
        }

        return responses.OrderByDescending(r => r.Priority).ToList();
    }

    private async Task<List<SecurityThreat>> AnalyzeBehavioralThreatsAsync(
        string userId,
        SecurityEvent securityEvent,
        CancellationToken cancellationToken)
    {
        var threats = new List<SecurityThreat>();

        // Get current behavior context
        var currentBehavior = await _behaviorAnalytics.GetCurrentBehaviorAsync(userId, cancellationToken);
        
        // Check for anomalous behavior
        if (currentBehavior.IsAnomalousActivity)
        {
            threats.Add(new SecurityThreat
            {
                Id = Guid.NewGuid().ToString(),
                Type = ThreatType.BehavioralAnomaly,
                Severity = ThreatSeverity.Medium,
                Description = "Unusual user behavior pattern detected",
                DetectedAt = DateTime.UtcNow,
                SourceEventId = securityEvent.Id,
                Indicators = new List<string> { $"Behavior score: {currentBehavior.BehaviorScore}" }
            });
        }

        // Check failed login attempts
        if (currentBehavior.FailedLoginAttempts > MaxFailedAttemptsPerHour)
        {
            threats.Add(new SecurityThreat
            {
                Id = Guid.NewGuid().ToString(),
                Type = ThreatType.BruteForceAttack,
                Severity = ThreatSeverity.High,
                Description = $"Excessive failed login attempts: {currentBehavior.FailedLoginAttempts}",
                DetectedAt = DateTime.UtcNow,
                SourceEventId = securityEvent.Id
            });
        }

        return threats;
    }

    private async Task<List<SecurityThreat>> AnalyzePatternThreatsAsync(
        string userId,
        SecurityEvent securityEvent,
        CancellationToken cancellationToken)
    {
        var threats = new List<SecurityThreat>();
        var recentEvents = await _securityEventRepository.GetRecentEventsByUserAsync(userId, TimeSpan.FromHours(1), cancellationToken);

        // Check for rapid succession of events
        var rapidEvents = recentEvents
            .Where(e => e.Timestamp > DateTime.UtcNow.AddMinutes(-5))
            .Count();

        if (rapidEvents > 50) // More than 50 events in 5 minutes
        {
            threats.Add(new SecurityThreat
            {
                Id = Guid.NewGuid().ToString(),
                Type = ThreatType.AutomatedAttack,
                Severity = ThreatSeverity.High,
                Description = $"Rapid succession of security events: {rapidEvents} in 5 minutes",
                DetectedAt = DateTime.UtcNow,
                SourceEventId = securityEvent.Id
            });
        }

        // Check for privilege escalation attempts
        var privilegeEvents = recentEvents
            .Where(e => e.EventType == "PrivilegeEscalation" || e.EventType == "UnauthorizedAccess")
            .Count();

        if (privilegeEvents > 3)
        {
            threats.Add(new SecurityThreat
            {
                Id = Guid.NewGuid().ToString(),
                Type = ThreatType.PrivilegeEscalation,
                Severity = ThreatSeverity.High,
                Description = $"Multiple privilege escalation attempts detected: {privilegeEvents}",
                DetectedAt = DateTime.UtcNow,
                SourceEventId = securityEvent.Id
            });
        }

        return threats;
    }

    private async Task<List<SecurityThreat>> CheckThreatIntelligenceAsync(
        SecurityEvent securityEvent,
        CancellationToken cancellationToken)
    {
        var threats = new List<SecurityThreat>();

        // Check IP reputation
        if (!string.IsNullOrEmpty(securityEvent.IpAddress))
        {
            var ipThreats = await _threatIntelligence.CheckIpReputationAsync(securityEvent.IpAddress, cancellationToken);
            threats.AddRange(ipThreats);
        }

        // Check user agent patterns
        if (!string.IsNullOrEmpty(securityEvent.UserAgent))
        {
            var uaThreats = await _threatIntelligence.CheckUserAgentAsync(securityEvent.UserAgent, cancellationToken);
            threats.AddRange(uaThreats);
        }

        return threats;
    }

    private async Task<List<SecurityThreat>> AnalyzeNetworkThreatsAsync(
        SecurityEvent securityEvent,
        CancellationToken cancellationToken)
    {
        var threats = new List<SecurityThreat>();

        await Task.Delay(1, cancellationToken); // Placeholder for async operation

        // Check for suspicious network patterns
        if (!string.IsNullOrEmpty(securityEvent.IpAddress))
        {
            // Check for known malicious IP ranges
            if (IsFromSuspiciousNetwork(securityEvent.IpAddress))
            {
                threats.Add(new SecurityThreat
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = ThreatType.MaliciousNetwork,
                    Severity = ThreatSeverity.High,
                    Description = $"Access from suspicious network: {securityEvent.IpAddress}",
                    DetectedAt = DateTime.UtcNow,
                    SourceEventId = securityEvent.Id
                });
            }

            // Check for TOR exit nodes
            if (IsTorExitNode(securityEvent.IpAddress))
            {
                threats.Add(new SecurityThreat
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = ThreatType.AnonymousNetwork,
                    Severity = ThreatSeverity.Medium,
                    Description = $"Access from TOR exit node: {securityEvent.IpAddress}",
                    DetectedAt = DateTime.UtcNow,
                    SourceEventId = securityEvent.Id
                });
            }
        }

        return threats;
    }

    private decimal CalculateRiskScore(List<SecurityThreat> threats)
    {
        if (!threats.Any()) return 0;

        var totalScore = threats.Sum(t => GetThreatWeight(t.Type, t.Severity));
        var averageScore = totalScore / threats.Count;

        // Apply multiplier for multiple threats
        var multiplier = Math.Min(1.5m, 1 + (threats.Count - 1) * 0.1m);
        
        return Math.Min(1.0m, averageScore * multiplier);
    }

    private decimal GetThreatWeight(ThreatType type, ThreatSeverity severity)
    {
        var baseWeight = severity switch
        {
            ThreatSeverity.Critical => 1.0m,
            ThreatSeverity.High => 0.8m,
            ThreatSeverity.Medium => 0.5m,
            ThreatSeverity.Low => 0.2m,
            _ => 0.1m
        };

        var typeMultiplier = type switch
        {
            ThreatType.DataBreach => 1.5m,
            ThreatType.PrivilegeEscalation => 1.3m,
            ThreatType.BruteForceAttack => 1.2m,
            ThreatType.MaliciousNetwork => 1.1m,
            ThreatType.AutomatedAttack => 1.1m,
            _ => 1.0m
        };

        return baseWeight * typeMultiplier;
    }

    private ThreatRiskLevel DetermineRiskLevel(decimal riskScore)
    {
        return riskScore switch
        {
            >= 0.9m => ThreatRiskLevel.Critical,
            >= 0.7m => ThreatRiskLevel.High,
            >= 0.5m => ThreatRiskLevel.Medium,
            >= 0.2m => ThreatRiskLevel.Low,
            _ => ThreatRiskLevel.Minimal
        };
    }

    private bool IsFromSuspiciousNetwork(string ipAddress)
    {
        // Implementation would check against threat intelligence feeds
        // For demo purposes, return false
        return false;
    }

    private bool IsTorExitNode(string ipAddress)
    {
        // Implementation would check against TOR exit node lists
        // For demo purposes, return false
        return false;
    }

    private async Task<List<BehaviorAnomaly>> AnalyzeLoginPatternsAsync(
        string userId,
        List<SecurityEvent> events,
        UserBehaviorBaseline baseline,
        CancellationToken cancellationToken)
    {
        await Task.Delay(1, cancellationToken);
        var anomalies = new List<BehaviorAnomaly>();

        var loginEvents = events.Where(e => e.EventType == "Login").ToList();
        
        // Check for unusual login times
        foreach (var loginEvent in loginEvents)
        {
            if (IsUnusualLoginTime(loginEvent.Timestamp, baseline.TypicalLoginHours))
            {
                anomalies.Add(new BehaviorAnomaly
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = "UnusualLoginTime",
                    Severity = 0.6m,
                    Description = $"Login at unusual time: {loginEvent.Timestamp:HH:mm}",
                    DetectedAt = DateTime.UtcNow,
                    EventId = loginEvent.Id
                });
            }
        }

        return anomalies;
    }

    private async Task<List<BehaviorAnomaly>> AnalyzeAccessPatternsAsync(
        string userId,
        List<SecurityEvent> events,
        UserBehaviorBaseline baseline,
        CancellationToken cancellationToken)
    {
        await Task.Delay(1, cancellationToken);
        var anomalies = new List<BehaviorAnomaly>();

        var accessEvents = events.Where(e => e.EventType == "ResourceAccess").ToList();
        
        // Check for access to unusual resources
        var unusualResources = accessEvents
            .Where(e => !baseline.TypicalResources.Contains(e.ResourceId ?? ""))
            .ToList();

        if (unusualResources.Count > 5)
        {
            anomalies.Add(new BehaviorAnomaly
            {
                Id = Guid.NewGuid().ToString(),
                Type = "UnusualResourceAccess",
                Severity = 0.7m,
                Description = $"Access to {unusualResources.Count} unusual resources",
                DetectedAt = DateTime.UtcNow
            });
        }

        return anomalies;
    }

    private async Task<List<BehaviorAnomaly>> AnalyzeTemporalPatternsAsync(
        string userId,
        List<SecurityEvent> events,
        UserBehaviorBaseline baseline,
        CancellationToken cancellationToken)
    {
        await Task.Delay(1, cancellationToken);
        var anomalies = new List<BehaviorAnomaly>();

        // Check for unusual activity volume
        var hourlyActivity = events
            .GroupBy(e => e.Timestamp.Hour)
            .ToDictionary(g => g.Key, g => g.Count());

        foreach (var hourActivity in hourlyActivity)
        {
            var expectedActivity = baseline.TypicalHourlyActivity.GetValueOrDefault(hourActivity.Key, 0);
            if (hourActivity.Value > expectedActivity * 3) // More than 3x normal activity
            {
                anomalies.Add(new BehaviorAnomaly
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = "UnusualActivityVolume",
                    Severity = 0.5m,
                    Description = $"High activity at hour {hourActivity.Key}: {hourActivity.Value} events",
                    DetectedAt = DateTime.UtcNow
                });
            }
        }

        return anomalies;
    }

    private async Task<List<BehaviorAnomaly>> AnalyzeGeographicalPatternsAsync(
        string userId,
        List<SecurityEvent> events,
        UserBehaviorBaseline baseline,
        CancellationToken cancellationToken)
    {
        await Task.Delay(1, cancellationToken);
        var anomalies = new List<BehaviorAnomaly>();

        var uniqueLocations = events
            .Where(e => !string.IsNullOrEmpty(e.Location))
            .Select(e => e.Location)
            .Distinct()
            .ToList();

        // Check for access from unusual locations
        var unusualLocations = uniqueLocations
            .Where(l => !baseline.TypicalLocations.Contains(l!))
            .ToList();

        if (unusualLocations.Any())
        {
            anomalies.Add(new BehaviorAnomaly
            {
                Id = Guid.NewGuid().ToString(),
                Type = "UnusualLocation",
                Severity = 0.8m,
                Description = $"Access from unusual locations: {string.Join(", ", unusualLocations)}",
                DetectedAt = DateTime.UtcNow
            });
        }

        return anomalies;
    }

    private bool IsUnusualLoginTime(DateTime loginTime, List<int> typicalHours)
    {
        return !typicalHours.Contains(loginTime.Hour);
    }

    private string GenerateIncidentTitle(ThreatAssessment assessment)
    {
        var primaryThreat = assessment.Threats.OrderByDescending(t => t.Severity).FirstOrDefault();
        return primaryThreat?.Type switch
        {
            ThreatType.BruteForceAttack => "Brute Force Attack Detected",
            ThreatType.DataBreach => "Potential Data Breach",
            ThreatType.PrivilegeEscalation => "Privilege Escalation Attempt",
            ThreatType.MaliciousNetwork => "Malicious Network Activity",
            ThreatType.BehavioralAnomaly => "Behavioral Anomaly Detected",
            _ => "Security Threat Detected"
        };
    }

    private string GenerateIncidentDescription(ThreatAssessment assessment)
    {
        var threatDescriptions = assessment.Threats.Select(t => t.Description);
        return $"Multiple security threats detected (Risk Score: {assessment.RiskScore:F2}):\n" +
               string.Join("\n- ", threatDescriptions);
    }

    private IncidentSeverity DetermineIncidentSeverity(ThreatRiskLevel riskLevel)
    {
        return riskLevel switch
        {
            ThreatRiskLevel.Critical => IncidentSeverity.Critical,
            ThreatRiskLevel.High => IncidentSeverity.High,
            ThreatRiskLevel.Medium => IncidentSeverity.Medium,
            ThreatRiskLevel.Low => IncidentSeverity.Low,
            _ => IncidentSeverity.Informational
        };
    }

    private async Task<AutomatedResponse?> GenerateResponseForThreatAsync(
        SecurityThreat threat,
        ThreatAssessment assessment,
        CancellationToken cancellationToken)
    {
        await Task.Delay(1, cancellationToken);

        return threat.Type switch
        {
            ThreatType.BruteForceAttack => new AutomatedResponse
            {
                Id = Guid.NewGuid().ToString(),
                Type = ResponseType.IpBlocking,
                Action = "Block source IP address",
                Priority = ResponsePriority.High,
                IsAutoExecuted = true,
                ExecuteAt = DateTime.UtcNow
            },
            ThreatType.MaliciousNetwork => new AutomatedResponse
            {
                Id = Guid.NewGuid().ToString(),
                Type = ResponseType.NetworkIsolation,
                Action = "Isolate user session",
                Priority = ResponsePriority.High,
                IsAutoExecuted = true,
                ExecuteAt = DateTime.UtcNow
            },
            ThreatType.PrivilegeEscalation => new AutomatedResponse
            {
                Id = Guid.NewGuid().ToString(),
                Type = ResponseType.PrivilegeRevocation,
                Action = "Revoke elevated privileges",
                Priority = ResponsePriority.Critical,
                IsAutoExecuted = true,
                ExecuteAt = DateTime.UtcNow
            },
            _ => null
        };
    }
}

// Supporting interfaces and entities
public interface IThreatDetectionService
{
    Task<ThreatAssessment> AnalyzeThreatAsync(string userId, SecurityEvent securityEvent, CancellationToken cancellationToken = default);
    Task<List<BehaviorAnomaly>> DetectBehavioralAnomaliesAsync(string userId, TimeSpan lookbackPeriod, CancellationToken cancellationToken = default);
    Task<SecurityIncident> CreateSecurityIncidentAsync(ThreatAssessment assessment, CancellationToken cancellationToken = default);
    Task<List<AutomatedResponse>> GenerateAutomatedResponseAsync(ThreatAssessment assessment, CancellationToken cancellationToken = default);
}

public interface ISecurityEventRepository
{
    Task<List<SecurityEvent>> GetEventsByUserAsync(string userId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken);
    Task<List<SecurityEvent>> GetRecentEventsByUserAsync(string userId, TimeSpan lookback, CancellationToken cancellationToken);
    Task SaveIncidentAsync(SecurityIncident incident, CancellationToken cancellationToken);
}

public interface IIncidentResponseService
{
    Task InitiateResponseAsync(SecurityIncident incident, CancellationToken cancellationToken);
}

public class SecurityThreat
{
    public string Id { get; set; } = string.Empty;
    public ThreatType Type { get; set; }
    public ThreatSeverity Severity { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime DetectedAt { get; set; }
    public string? SourceEventId { get; set; }
    public List<string> Indicators { get; set; } = new();
}

public class ThreatAssessment
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string EventId { get; set; } = string.Empty;
    public DateTime AssessmentTime { get; set; }
    public List<SecurityThreat> Threats { get; set; } = new();
    public decimal RiskScore { get; set; }
    public ThreatRiskLevel RiskLevel { get; set; }
    public List<AutomatedResponse> AutomatedActions { get; set; } = new();
}

public class BehaviorAnomaly
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public decimal Severity { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime DetectedAt { get; set; }
    public string? EventId { get; set; }
}

public class SecurityEvent
{
    public string Id { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? UserId { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? ResourceId { get; set; }
    public string? Location { get; set; }
}

public class SecurityIncident
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IncidentSeverity Severity { get; set; }
    public IncidentStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string ThreatAssessmentId { get; set; } = string.Empty;
    public List<SecurityThreat> Threats { get; set; } = new();
}

public class AutomatedResponse
{
    public string Id { get; set; } = string.Empty;
    public ResponseType Type { get; set; }
    public string Action { get; set; } = string.Empty;
    public ResponsePriority Priority { get; set; }
    public bool IsAutoExecuted { get; set; }
    public DateTime ExecuteAt { get; set; }
}

public class UserBehaviorBaseline
{
    public List<int> TypicalLoginHours { get; set; } = new();
    public List<string> TypicalResources { get; set; } = new();
    public List<string> TypicalLocations { get; set; } = new();
    public Dictionary<int, int> TypicalHourlyActivity { get; set; } = new();
}

public enum ThreatType
{
    BehavioralAnomaly,
    BruteForceAttack,
    PrivilegeEscalation,
    DataBreach,
    MaliciousNetwork,
    AutomatedAttack,
    AnonymousNetwork,
    SystemFailure
}

public enum ThreatSeverity
{
    Informational,
    Low,
    Medium,
    High,
    Critical
}

public enum ThreatRiskLevel
{
    Minimal,
    Low,
    Medium,
    High,
    Critical
}

public enum IncidentSeverity
{
    Informational,
    Low,
    Medium,
    High,
    Critical
}

public enum IncidentStatus
{
    Open,
    InProgress,
    Resolved,
    Closed
}

public enum ResponseType
{
    UserSuspension,
    IpBlocking,
    NetworkIsolation,
    PrivilegeRevocation,
    EnhancedMonitoring,
    AlertGeneration
}

public enum ResponsePriority
{
    Low,
    Medium,
    High,
    Critical
}
