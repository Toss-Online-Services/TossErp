using TossErp.CRM.Domain.Enums;
using TossErp.CRM.Domain.ValueObjects;

namespace TossErp.CRM.Application.Services;

/// <summary>
/// Service interface for opportunity management operations
/// Handles opportunity lifecycle, forecasting, and reporting
/// </summary>
public interface IOpportunityManagementService
{
    Task<OpportunityManagementView> GetOpportunityManagementAsync(Guid tenantId, string userId);
    Task<OpportunityDetailsView> GetOpportunityDetailsAsync(Guid opportunityId, Guid tenantId);
    Task<OpportunityForecastView> GetOpportunityForecastAsync(Guid tenantId, int months = 12);
    Task<IEnumerable<OpportunityView>> GetOpportunitiesByStageAsync(Guid tenantId, OpportunityStage stage);
    Task<IEnumerable<OpportunityView>> GetOverdueOpportunitiesAsync(Guid tenantId);
    Task<IEnumerable<OpportunityView>> GetClosingSoonOpportunitiesAsync(Guid tenantId, int days = 30);
    Task<OpportunityMetrics> GetOpportunityMetricsAsync(Guid tenantId, DateTime? from = null, DateTime? to = null);
    Task<Guid> ConvertLeadToOpportunityAsync(Guid leadId, ConvertLeadRequest request, Guid tenantId, string userId);
}

/// <summary>
/// View model for opportunity management dashboard
/// </summary>
public class OpportunityManagementView
{
    public OpportunityMetrics Metrics { get; set; } = null!;
    public IEnumerable<OpportunityStageView> Stages { get; set; } = new List<OpportunityStageView>();
    public IEnumerable<OpportunityView> RecentOpportunities { get; set; } = new List<OpportunityView>();
    public IEnumerable<OpportunityView> OverdueOpportunities { get; set; } = new List<OpportunityView>();
    public IEnumerable<OpportunityView> ClosingSoonOpportunities { get; set; } = new List<OpportunityView>();
    public OpportunityForecastView Forecast { get; set; } = null!;
}

/// <summary>
/// View model for opportunity stage in the pipeline
/// </summary>
public class OpportunityStageView
{
    public OpportunityStage Stage { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Color { get; set; } = null!;
    public string Icon { get; set; } = null!;
    public int OpportunityCount { get; set; }
    public decimal TotalValue { get; set; }
    public decimal WeightedValue { get; set; }
    public decimal AverageProbability { get; set; }
    public int AverageDaysInStage { get; set; }
    public IEnumerable<OpportunityView> Opportunities { get; set; } = new List<OpportunityView>();
}

/// <summary>
/// View model for individual opportunity cards
/// </summary>
public class OpportunityView
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = null!;
    public OpportunityStage Stage { get; set; }
    public string StageName { get; set; } = null!;
    public OpportunityType Type { get; set; }
    public OpportunityPriority Priority { get; set; }
    public decimal EstimatedValue { get; set; }
    public string Currency { get; set; } = null!;
    public decimal Probability { get; set; }
    public decimal WeightedValue { get; set; }
    public DateTime ExpectedCloseDate { get; set; }
    public DateTime? ActualCloseDate { get; set; }
    public string? AssignedTo { get; set; }
    public string? SalesTeam { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastActivityDate { get; set; }
    public DateTime? NextFollowUp { get; set; }
    public bool IsOverdue { get; set; }
    public bool IsClosingSoon { get; set; }
    public bool IsHighPriority { get; set; }
    public bool IsStale { get; set; }
    public int DaysInPipeline { get; set; }
    public int? DaysSinceLastActivity { get; set; }
    public int DaysUntilExpectedClose { get; set; }
    public LeadSource? Source { get; set; }
    public string? CampaignName { get; set; }
}

/// <summary>
/// Detailed view model for individual opportunity
/// </summary>
public class OpportunityDetailsView : OpportunityView
{
    public string CreatedBy { get; set; } = null!;
    public DateTime? ModifiedAt { get; set; }
    public string? ModifiedBy { get; set; }
    public string? WinReason { get; set; }
    public string? LossReason { get; set; }
    public string? CompetitorName { get; set; }
    public decimal? ActualValue { get; set; }
    public string? Remarks { get; set; }
    public int ContactAttempts { get; set; }
    public IEnumerable<ActivityView> Activities { get; set; } = new List<ActivityView>();
    public IEnumerable<CommunicationView> Communications { get; set; } = new List<CommunicationView>();
    public IEnumerable<DocumentView> Documents { get; set; } = new List<DocumentView>();
    public IEnumerable<NoteView> Notes { get; set; } = new List<NoteView>();
}

/// <summary>
/// Opportunity metrics and KPIs
/// </summary>
public class OpportunityMetrics
{
    public int TotalOpportunities { get; set; }
    public int OpenOpportunities { get; set; }
    public int WonOpportunities { get; set; }
    public int LostOpportunities { get; set; }
    public decimal TotalPipelineValue { get; set; }
    public decimal WeightedPipelineValue { get; set; }
    public decimal WonValue { get; set; }
    public decimal LostValue { get; set; }
    public decimal WinRate { get; set; }
    public decimal LossRate { get; set; }
    public decimal AverageDealSize { get; set; }
    public decimal AverageSalesCycle { get; set; }
    public decimal ConversionRate { get; set; }
    public int OverdueOpportunities { get; set; }
    public int ClosingSoonOpportunities { get; set; }
    public int StaleOpportunities { get; set; }
    public Dictionary<OpportunityStage, int> OpportunitiesByStage { get; set; } = new();
    public Dictionary<OpportunityStage, decimal> ValueByStage { get; set; } = new();
    public Dictionary<string, decimal> ValueBySource { get; set; } = new();
    public Dictionary<string, decimal> ValueByAssignee { get; set; } = new();
}

/// <summary>
/// Opportunity forecast data
/// </summary>
public class OpportunityForecastView
{
    public decimal TotalForecast { get; set; }
    public decimal ConservativeForecast { get; set; }
    public decimal OptimisticForecast { get; set; }
    public decimal CommittedForecast { get; set; }
    public decimal BestCaseForecast { get; set; }
    public IEnumerable<MonthlyForecast> MonthlyForecasts { get; set; } = new List<MonthlyForecast>();
    public IEnumerable<OpportunityView> ForecastOpportunities { get; set; } = new List<OpportunityView>();
}

/// <summary>
/// Monthly forecast breakdown
/// </summary>
public class MonthlyForecast
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string MonthName { get; set; } = null!;
    public decimal ExpectedValue { get; set; }
    public decimal WeightedValue { get; set; }
    public decimal CommittedValue { get; set; }
    public int OpportunityCount { get; set; }
    public IEnumerable<OpportunityView> Opportunities { get; set; } = new List<OpportunityView>();
}

/// <summary>
/// Request model for converting lead to opportunity
/// </summary>
public class ConvertLeadRequest
{
    public string OpportunityName { get; set; } = null!;
    public string? Description { get; set; }
    public decimal EstimatedValue { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime ExpectedCloseDate { get; set; }
    public OpportunityType Type { get; set; } = OpportunityType.NewBusiness;
    public OpportunityPriority Priority { get; set; } = OpportunityPriority.Medium;
    public string? AssignedTo { get; set; }
    public string? SalesTeam { get; set; }
    public Guid? CampaignId { get; set; }
    public string? CampaignName { get; set; }
}

/// <summary>
/// Activity view model
/// </summary>
public class ActivityView
{
    public Guid Id { get; set; }
    public ActivityType Type { get; set; }
    public string Subject { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime ScheduledAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public ActivityStatus Status { get; set; }
    public string? Outcome { get; set; }
    public string? NextAction { get; set; }
    public string CreatedBy { get; set; } = null!;
    public string? AssignedTo { get; set; }
    public bool IsOverdue { get; set; }
    public int Duration { get; set; }
}

/// <summary>
/// Communication view model
/// </summary>
public class CommunicationView
{
    public Guid Id { get; set; }
    public CommunicationType Type { get; set; }
    public string Subject { get; set; } = null!;
    public string? Content { get; set; }
    public DateTime CommunicatedAt { get; set; }
    public CommunicationDirection Direction { get; set; }
    public string FromContact { get; set; } = null!;
    public string ToContact { get; set; } = null!;
    public CommunicationStatus Status { get; set; }
    public string CreatedBy { get; set; } = null!;
}

/// <summary>
/// Document view model
/// </summary>
public class DocumentView
{
    public Guid Id { get; set; }
    public DocumentType Type { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string FilePath { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public long FileSize { get; set; }
    public DateTime UploadedAt { get; set; }
    public string UploadedBy { get; set; } = null!;
    public bool IsActive { get; set; }
    public string? Version { get; set; }
}

/// <summary>
/// Note view model
/// </summary>
public class NoteView
{
    public Guid Id { get; set; }
    public string Content { get; set; } = null!;
    public NoteType Type { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = null!;
    public DateTime? ModifiedAt { get; set; }
    public string? ModifiedBy { get; set; }
    public bool IsPrivate { get; set; }
}
