using Microsoft.AspNetCore.Http.HttpResults;
using TossErp.Stock.Agent;

namespace TossErp.Stock.API.Endpoints;

/// <summary>
/// LangChain AI Agent endpoints for natural language processing and automation
/// </summary>
public class LangChainEndpoints : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(this);
        
        // Natural language query processing
        group.MapPost(ProcessQuery, "query");
        
        // Automation operations
        group.MapPost(AutomateOperation, "automate");
        
        // AI insights generation
        group.MapGet(GenerateInsights, "insights/{insightType}");
        
        // Agent status and capabilities
        group.MapGet(GetAgentCapabilities, "capabilities");
    }

    /// <summary>
    /// Process natural language query using LangChain agent
    /// </summary>
    public async Task<Results<Ok<LangChainResponse>, BadRequest>> ProcessQuery(
        LangChainAgent agent,
        [FromBody] ProcessQueryRequest request)
    {
        try
        {
            var response = await agent.ProcessQueryAsync(
                request.Query, 
                request.UserId ?? "anonymous",
                CancellationToken.None);
            
            return TypedResults.Ok(response);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest();
        }
    }

    /// <summary>
    /// Automate stock operations using LangChain agent
    /// </summary>
    public async Task<Results<Ok<AutomationResult>, BadRequest>> AutomateOperation(
        LangChainAgent agent,
        [FromBody] AutomateOperationRequest request)
    {
        try
        {
            var result = await agent.AutomateStockOperationsAsync(
                request.Operation,
                request.UserId ?? "anonymous",
                CancellationToken.None);
            
            return TypedResults.Ok(result);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest();
        }
    }

    /// <summary>
    /// Generate AI insights using LangChain agent
    /// </summary>
    public async Task<Results<Ok<AIInsights>, BadRequest>> GenerateInsights(
        LangChainAgent agent,
        string insightType)
    {
        try
        {
            var insights = await agent.GenerateInsightsAsync(insightType, CancellationToken.None);
            return TypedResults.Ok(insights);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest();
        }
    }

    /// <summary>
    /// Get agent capabilities and available operations
    /// </summary>
    public Results<Ok<AgentCapabilities>> GetAgentCapabilities()
    {
        var capabilities = new AgentCapabilities
        {
            AvailableIntents = Enum.GetValues<QueryIntent>(),
            SupportedOperations = new[]
            {
                "Check stock levels",
                "Get reorder recommendations",
                "Search for items",
                "View stock movements", 
                "Analyze warehouses",
                "Automate reorder process",
                "Send low stock alerts",
                "Manage expiry tracking",
                "Optimize warehouse space",
                "Evaluate suppliers"
            },
            SupportedInsights = new[]
            {
                "demand",
                "cost", 
                "supplier",
                "warehouse",
                "trends"
            }
        };

        return TypedResults.Ok(capabilities);
    }
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
    public QueryIntent[] AvailableIntents { get; set; } = Array.Empty<QueryIntent>();
    public string[] SupportedOperations { get; set; } = Array.Empty<string>();
    public string[] SupportedInsights { get; set; } = Array.Empty<string>();
}
