namespace TossErp.AI.Services;

/// <summary>
/// Manages autonomous agents that deliver business services
/// </summary>
public interface IAutonomousAgentManager
{
    /// <summary>
    /// Gets the status of all autonomous services
    /// </summary>
    Task<ServiceStatusResponse> GetServiceStatusAsync();
    
    /// <summary>
    /// Executes an autonomous service action
    /// </summary>
    Task<AutonomousActionResult> ExecuteActionAsync(AutonomousAction action);
    
    /// <summary>
    /// Gets available autonomous services for a user
    /// </summary>
    Task<List<AutonomousService>> GetAvailableServicesAsync(string userId);
    
    /// <summary>
    /// Enables or disables autonomous services
    /// </summary>
    Task<bool> ConfigureServiceAsync(string serviceId, bool enabled, Dictionary<string, object> settings);
}

public class ServiceStatusResponse
{
    public List<AutonomousServiceStatus> Services { get; set; } = new();
    public int ActiveServices { get; set; }
    public int CompletedActionsToday { get; set; }
    public decimal MoneySaved { get; set; }
    public int TimeSavedHours { get; set; }
}

public class AutonomousServiceStatus
{
    public string ServiceId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string Status { get; set; } = string.Empty; // running, idle, error
    public DateTime LastAction { get; set; }
    public int ActionsCompleted { get; set; }
    public decimal ValueGenerated { get; set; }
}

public class AutonomousAction
{
    public string ActionId { get; set; } = Guid.NewGuid().ToString();
    public string Service { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public Dictionary<string, object> Parameters { get; set; } = new();
    public bool RequiresApproval { get; set; }
    public string? ApprovalMessage { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class AutonomousActionResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Dictionary<string, object> Data { get; set; } = new();
    public decimal ValueGenerated { get; set; }
    public TimeSpan TimeSaved { get; set; }
    public List<string> ActionsPerformed { get; set; } = new();
}

public class AutonomousService
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public Dictionary<string, object> Settings { get; set; } = new();
    public List<string> Capabilities { get; set; } = new();
    public decimal EstimatedMonthlyValue { get; set; }
    public int EstimatedTimeSavedHours { get; set; }
}

