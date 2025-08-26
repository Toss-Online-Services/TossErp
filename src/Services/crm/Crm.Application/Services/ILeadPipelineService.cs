namespace TossErp.CRM.Application.Services;

/// <summary>
/// Service for managing visual lead pipeline operations
/// Provides pipeline visualization, stage management, and progress tracking
/// </summary>
public interface ILeadPipelineService
{
    /// <summary>
    /// Get all leads organized by pipeline stage for visual display
    /// </summary>
    Task<LeadPipelineView> GetPipelineViewAsync(Guid tenantId, string? assignedTo = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Move a lead to a different pipeline stage
    /// </summary>
    Task<bool> MoveLeadToStageAsync(Guid tenantId, Guid leadId, string targetStage, string movedBy, string? reason = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get pipeline stage statistics and performance metrics
    /// </summary>
    Task<PipelineMetrics> GetPipelineMetricsAsync(Guid tenantId, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get leads that are overdue in their current pipeline stage
    /// </summary>
    Task<IEnumerable<OverdueLeadInfo>> GetOverdueLeadsAsync(Guid tenantId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Visual representation of the lead pipeline
/// </summary>
public class LeadPipelineView
{
    public List<PipelineStageView> Stages { get; set; } = new();
    public PipelineMetrics Metrics { get; set; } = new();
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Visual representation of a pipeline stage with its leads
/// </summary>
public class PipelineStageView
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public int Order { get; set; }
    public int LeadCount { get; set; }
    public decimal TotalValue { get; set; }
    public decimal AverageScore { get; set; }
    public List<LeadCardView> Leads { get; set; } = new();
}

/// <summary>
/// Card view representation of a lead for pipeline display
/// </summary>
public class LeadCardView
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string? JobTitle { get; set; }
    public int Score { get; set; }
    public string Priority { get; set; } = string.Empty;
    public string? AssignedTo { get; set; }
    public decimal? EstimatedValue { get; set; }
    public DateTime? ExpectedCloseDate { get; set; }
    public int DaysInStage { get; set; }
    public bool IsOverdue { get; set; }
    public bool IsHighPriority { get; set; }
    public string QualificationStatus { get; set; } = string.Empty;
    public DateTime LastContactedAt { get; set; }
    public int DaysSinceLastContact { get; set; }
}

/// <summary>
/// Pipeline performance metrics and statistics
/// </summary>
public class PipelineMetrics
{
    public int TotalLeads { get; set; }
    public int QualifiedLeads { get; set; }
    public int ConvertedLeads { get; set; }
    public decimal ConversionRate { get; set; }
    public decimal AverageScore { get; set; }
    public decimal TotalPipelineValue { get; set; }
    public decimal WeightedPipelineValue { get; set; }
    public double AverageDaysInPipeline { get; set; }
    public Dictionary<string, int> LeadsByStage { get; set; } = new();
    public Dictionary<string, decimal> ValueByStage { get; set; } = new();
}

/// <summary>
/// Information about leads that are overdue in their current stage
/// </summary>
public class OverdueLeadInfo
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string PipelineStage { get; set; } = string.Empty;
    public int DaysInStage { get; set; }
    public int MaxAllowedDays { get; set; }
    public int DaysOverdue { get; set; }
    public string? AssignedTo { get; set; }
    public DateTime? LastContactedAt { get; set; }
    public int Score { get; set; }
}
