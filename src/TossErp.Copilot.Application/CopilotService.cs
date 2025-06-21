using Microsoft.SemanticKernel;
using TossErp.Copilot.Domain;
using Microsoft.Extensions.Logging;

namespace TossErp.Copilot.Application;

/// <summary>
/// AI Copilot service implementation using Microsoft Semantic Kernel
/// </summary>
public class CopilotService : ICopilotService
{
    private readonly Kernel _kernel;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<CopilotService> _logger;

    public CopilotService(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<CopilotService>();
        
        var builder = Kernel.CreateBuilder();

        // Register all Copilot skills as plugins
        builder.Plugins.AddFromObject(new Skills.InventorySkill(_loggerFactory.CreateLogger<Skills.InventorySkill>()), "InventorySkill");
        builder.Plugins.AddFromObject(new Skills.POSSkill(_loggerFactory.CreateLogger<Skills.POSSkill>()), "POSSkill");
        builder.Plugins.AddFromObject(new Skills.VendorSkill(_loggerFactory.CreateLogger<Skills.VendorSkill>()), "VendorSkill");
        builder.Plugins.AddFromObject(new Skills.GroupBuySkill(_loggerFactory.CreateLogger<Skills.GroupBuySkill>()), "GroupBuySkill");
        builder.Plugins.AddFromObject(new Skills.TransportSkill(_loggerFactory.CreateLogger<Skills.TransportSkill>()), "TransportSkill");
        builder.Plugins.AddFromObject(new Skills.CustomerSkill(_loggerFactory.CreateLogger<Skills.CustomerSkill>()), "CustomerSkill");

        _kernel = builder.Build();
        
        _logger.LogInformation("CopilotService initialized with all skills registered");
    }

    /// <summary>
    /// Get AI Copilot response using Semantic Kernel
    /// </summary>
    public async Task<string> GetCopilotResponseAsync(string prompt, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Processing copilot prompt: {Prompt}", prompt);

            // Create a simple function for now
            var function = _kernel.CreateFunctionFromPrompt(@"
You are an AI business assistant for TossErp, a township and rural enterprise ERP system. 
You help with business operations including:

1. **Inventory Management**: Stock levels, reorder recommendations, stock movements
2. **POS & Sales**: Sales reports, performance analysis, daily summaries
3. **Vendor Management**: Vendor comparison, price analysis, supplier performance
4. **Group Buying**: Group purchases, cooperative buying, cost savings
5. **Transport & Logistics**: Delivery estimates, driver assignment, shipment tracking
6. **Customer Management**: Customer analysis, loyalty insights, communication

Provide helpful, professional, and actionable business advice.

User request: {{$prompt}}

Respond with practical business insights and recommendations.");

            var result = await _kernel.InvokeAsync(function, new() { ["prompt"] = prompt }, cancellationToken);
            
            var response = result.GetValue<string>() ?? "I'm sorry, I couldn't process your request. Please try again.";
            
            _logger.LogInformation("Generated copilot response for prompt: {Prompt}", prompt);
            
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating copilot response for prompt: {Prompt}", prompt);
            return "I'm sorry, I encountered an error while processing your request. Please try again or contact support.";
        }
    }

    /// <summary>
    /// Get specific business insights using targeted skills
    /// </summary>
    public async Task<string> GetBusinessInsightsAsync(string insightType, string parameters = "")
    {
        try
        {
            _logger.LogInformation("Getting business insights for type: {InsightType}", insightType);

            var function = _kernel.CreateFunctionFromPrompt(@"
You are a business intelligence assistant. Based on the insight type requested, provide detailed analysis and recommendations.

Insight Type: {{$insightType}}
Parameters: {{$parameters}}

Provide comprehensive business insights and actionable recommendations.");

            var result = await _kernel.InvokeAsync(function, new() 
            { 
                ["insightType"] = insightType,
                ["parameters"] = parameters
            });
            
            var response = result.GetValue<string>() ?? "I'm sorry, I couldn't generate the requested insights.";
            
            _logger.LogInformation("Generated business insights for type: {InsightType}", insightType);
            
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating business insights for type: {InsightType}", insightType);
            return "I'm sorry, I encountered an error while generating business insights. Please try again.";
        }
    }
} 
