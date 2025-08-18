namespace TossErp.AI.Services;

/// <summary>
/// Orchestrates conversational interactions and routes them to appropriate autonomous agents
/// </summary>
public interface IConversationalOrchestrator
{
    /// <summary>
    /// Processes natural language conversation and returns autonomous service response
    /// </summary>
    Task<ConversationResponse> ProcessConversationAsync(ConversationRequest request);
    
    /// <summary>
    /// Analyzes conversation intent and determines required services
    /// </summary>
    Task<ServiceIntent> AnalyzeIntentAsync(string userInput, string userId);
    
    /// <summary>
    /// Routes conversation to appropriate autonomous agent
    /// </summary>
    Task<ConversationResponse> RouteToAgentAsync(ServiceIntent intent, string userInput);
}

public class ConversationRequest
{
    public string UserId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Language { get; set; } = "en";
    public string Channel { get; set; } = "web"; // web, mobile, whatsapp, voice
    public Dictionary<string, object> Context { get; set; } = new();
}

public class ConversationResponse
{
    public string Message { get; set; } = string.Empty;
    public List<AutonomousAction> Actions { get; set; } = new();
    public Dictionary<string, object> Data { get; set; } = new();
    public bool RequiresConfirmation { get; set; }
    public string? ConfirmationMessage { get; set; }
}

public class ServiceIntent
{
    public string PrimaryService { get; set; } = string.Empty; // inventory, sales, purchasing, finance
    public List<string> SecondaryServices { get; set; } = new();
    public string Action { get; set; } = string.Empty; // query, create, update, automate
    public Dictionary<string, object> Parameters { get; set; } = new();
    public bool IsAutomationRequest { get; set; }
    public bool RequiresApproval { get; set; }
}

