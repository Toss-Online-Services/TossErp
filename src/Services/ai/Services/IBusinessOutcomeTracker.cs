namespace TossErp.AI.Services;

/// <summary>
/// Tracks and measures business outcomes delivered by autonomous services
/// </summary>
public interface IBusinessOutcomeTracker
{
    /// <summary>
    /// Gets business outcomes for a user
    /// </summary>
    Task<BusinessOutcomes> GetBusinessOutcomesAsync(string userId);
    
    /// <summary>
    /// Records a business outcome
    /// </summary>
    Task RecordOutcomeAsync(BusinessOutcome outcome);
    
    /// <summary>
    /// Gets ROI analysis for autonomous services
    /// </summary>
    Task<ROIAnalysis> GetROIAnalysisAsync(string userId);
    
    /// <summary>
    /// Gets service performance metrics
    /// </summary>
    Task<ServicePerformanceMetrics> GetServicePerformanceAsync(string userId);
}

public class BusinessOutcomes
{
    public decimal TotalMoneySaved { get; set; }
    public int TotalTimeSavedHours { get; set; }
    public int TasksAutomated { get; set; }
    public int ErrorsPrevented { get; set; }
    public decimal RevenueIncrease { get; set; }
    public List<BusinessOutcome> RecentOutcomes { get; set; } = new();
    public Dictionary<string, decimal> ServiceValue { get; set; } = new();
}

public class BusinessOutcome
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public string Service { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public decimal MoneySaved { get; set; }
    public TimeSpan TimeSaved { get; set; }
    public string Outcome { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object> Metrics { get; set; } = new();
}

public class ROIAnalysis
{
    public decimal TotalInvestment { get; set; }
    public decimal TotalReturn { get; set; }
    public decimal ROIPercentage { get; set; }
    public TimeSpan PaybackPeriod { get; set; }
    public List<ServiceROI> ServiceROIs { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
}

public class ServiceROI
{
    public string Service { get; set; } = string.Empty;
    public decimal Investment { get; set; }
    public decimal Return { get; set; }
    public decimal ROIPercentage { get; set; }
    public bool IsProfitable { get; set; }
}

public class ServicePerformanceMetrics
{
    public int TotalServices { get; set; }
    public int ActiveServices { get; set; }
    public decimal UptimePercentage { get; set; }
    public TimeSpan AverageResponseTime { get; set; }
    public int SuccessfulExecutions { get; set; }
    public int FailedExecutions { get; set; }
    public decimal SuccessRate { get; set; }
    public List<ServiceMetric> ServiceMetrics { get; set; } = new();
}

public class ServiceMetric
{
    public string Service { get; set; } = string.Empty;
    public int Executions { get; set; }
    public int Successes { get; set; }
    public int Failures { get; set; }
    public decimal SuccessRate { get; set; }
    public TimeSpan AverageExecutionTime { get; set; }
    public decimal ValueGenerated { get; set; }
}

