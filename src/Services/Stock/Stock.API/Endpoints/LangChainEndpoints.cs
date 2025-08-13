using Microsoft.AspNetCore.Http.HttpResults;
// using TossErp.Stock.Agent;

namespace TossErp.Stock.API.Endpoints;

/// <summary>
/// LangChain AI Agent endpoints for natural language processing and automation
/// </summary>
public class LangChainEndpoints : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        // Temporarily disabled until agent is stabilized
    }

    // Temporarily disabled endpoints
}

public class ProcessQueryRequest
{
    public string Query { get; set; } = string.Empty;
    public string? UserId { get; set; }
}

public class AutomateOperationRequest
{
    public string Operation { get; set; } = string.Empty;
    public string? UserId { get; set; }
}

public class AgentCapabilities
{
    public string[] AvailableIntents { get; set; } = Array.Empty<string>();
    public string[] SupportedOperations { get; set; } = Array.Empty<string>();
    public string[] SupportedInsights { get; set; } = Array.Empty<string>();
}
