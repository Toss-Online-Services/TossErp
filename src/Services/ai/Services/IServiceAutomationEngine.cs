namespace TossErp.AI.Services;

/// <summary>
/// Engine that executes automated business processes
/// </summary>
public interface IServiceAutomationEngine
{
    /// <summary>
    /// Executes an automation request
    /// </summary>
    Task<AutomationResult> ExecuteAutomationAsync(AutomationRequest request);
    
    /// <summary>
    /// Schedules recurring automation
    /// </summary>
    Task<string> ScheduleAutomationAsync(ScheduledAutomationRequest request);
    
    /// <summary>
    /// Gets automation history
    /// </summary>
    Task<List<AutomationHistory>> GetAutomationHistoryAsync(string userId);
    
    /// <summary>
    /// Cancels scheduled automation
    /// </summary>
    Task<bool> CancelAutomationAsync(string automationId);
}

public class AutomationRequest
{
    public string UserId { get; set; } = string.Empty;
    public string Service { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public Dictionary<string, object> Parameters { get; set; } = new();
    public bool IsRecurring { get; set; }
    public string? Schedule { get; set; } // cron expression
    public bool RequiresApproval { get; set; }
}

public class ScheduledAutomationRequest : AutomationRequest
{
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Frequency { get; set; } = string.Empty; // daily, weekly, monthly
    public Dictionary<string, object> Conditions { get; set; } = new();
}

public class AutomationResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> ActionsPerformed { get; set; } = new();
    public Dictionary<string, object> Results { get; set; } = new();
    public decimal ValueGenerated { get; set; }
    public TimeSpan TimeSaved { get; set; }
    public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;
}

public class AutomationHistory
{
    public string Id { get; set; } = string.Empty;
    public string Service { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime ExecutedAt { get; set; }
    public decimal ValueGenerated { get; set; }
    public TimeSpan TimeSaved { get; set; }
}

