using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Core;
using TossErp.Application.DTOs;
using TossErp.Domain.AggregatesModel.ProductAggregate;
using TossErp.Domain.AggregatesModel.SaleAggregate;
using TossErp.Domain.AggregatesModel.GroupPurchaseAggregate;

namespace TossErp.Application.Services;

/// <summary>
/// AI Copilot service implementation using Microsoft Semantic Kernel
/// Provides business intelligence and decision support for township enterprises
/// </summary>
public class CopilotService : ICopilotService
{
    private readonly Kernel _kernel;
    private readonly ILogger<CopilotService> _logger;
    private readonly IProductRepository _productRepository;
    private readonly ISaleRepository _saleRepository;
    private readonly IGroupPurchaseRepository _groupPurchaseRepository;

    public CopilotService(
        Kernel kernel,
        ILogger<CopilotService> logger,
        IProductRepository productRepository,
        ISaleRepository saleRepository,
        IGroupPurchaseRepository groupPurchaseRepository)
    {
        _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _productRepository = productRepository;
        _saleRepository = saleRepository;
        _groupPurchaseRepository = groupPurchaseRepository;
    }

    public async Task<CopilotResponseDto> ProcessQueryAsync(CopilotQueryDto query, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Processing AI Copilot query for business {BusinessId}: {Query}", 
                query.BusinessId, query.Query);

            // Create a prompt template for the query
            var prompt = $@"
You are TOSS ERP's AI Business Copilot, designed to help township and rural business owners in South Africa.
Business Context: Business ID {query.BusinessId}, User ID {query.UserId}

User Query: {query.Query}

Please provide:
1. A helpful response to the query
2. Relevant business insights
3. Actionable recommendations
4. Additional suggestions for business improvement

Focus on practical, actionable advice that would benefit a small business owner in a township or rural area.
Consider local market conditions, seasonal factors, and common challenges faced by SMMEs in South Africa.
";

            var result = await _kernel.InvokePromptAsync(prompt, cancellationToken: cancellationToken);
            var response = result.GetValue<string>() ?? "I'm sorry, I couldn't process your query at the moment.";

            return new CopilotResponseDto
            {
                Response = response,
                Type = "QueryResponse",
                Actions = ExtractActions(response),
                Suggestions = ExtractSuggestions(response),
                RequiresAction = response.Contains("action") || response.Contains("recommend")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing AI Copilot query for business {BusinessId}", query.BusinessId);
            return new CopilotResponseDto
            {
                Response = "I'm sorry, I encountered an error while processing your query. Please try again later.",
                Type = "Error",
                RequiresAction = false
            };
        }
    }

    public async Task<CopilotResponseDto> GetBusinessInsightsAsync(Guid businessId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Generating business insights for business {BusinessId}", businessId);

            var prompt = $@"
As TOSS ERP's AI Business Copilot, provide comprehensive business insights for a township/rural business (ID: {businessId}).

Consider:
- Seasonal business patterns in South Africa
- Local market trends
- Common challenges for SMMEs
- Opportunities for growth
- Risk factors to watch

Provide:
1. Key business insights
2. Market opportunities
3. Risk warnings
4. Growth recommendations
5. Operational improvements

Focus on practical, actionable advice for a small business owner.
";

            var result = await _kernel.InvokePromptAsync(prompt, cancellationToken: cancellationToken);
            var response = result.GetValue<string>() ?? "Unable to generate business insights at this time.";

            return new CopilotResponseDto
            {
                Response = response,
                Type = "BusinessInsights",
                Actions = ExtractActions(response),
                Suggestions = ExtractSuggestions(response),
                RequiresAction = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating business insights for business {BusinessId}", businessId);
            return new CopilotResponseDto
            {
                Response = "I'm sorry, I couldn't generate business insights at the moment. Please try again later.",
                Type = "Error",
                RequiresAction = false
            };
        }
    }

    public async Task<CopilotResponseDto> GetInventoryRecommendationsAsync(Guid businessId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Generating inventory recommendations for business {BusinessId}", businessId);

            var prompt = $@"
As TOSS ERP's AI Business Copilot, provide inventory management recommendations for a township/rural business (ID: {businessId}).

Consider:
- Seasonal inventory needs
- Stock optimization strategies
- Reorder point recommendations
- Dead stock management
- Inventory cost optimization
- Supplier relationship management

Provide:
1. Inventory optimization strategies
2. Stock level recommendations
3. Reorder suggestions
4. Cost-saving opportunities
5. Supplier management tips

Focus on practical inventory management for small businesses.
";

            var result = await _kernel.InvokePromptAsync(prompt, cancellationToken: cancellationToken);
            var response = result.GetValue<string>() ?? "Unable to generate inventory recommendations at this time.";

            return new CopilotResponseDto
            {
                Response = response,
                Type = "InventoryRecommendations",
                Actions = ExtractActions(response),
                Suggestions = ExtractSuggestions(response),
                RequiresAction = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating inventory recommendations for business {BusinessId}", businessId);
            return new CopilotResponseDto
            {
                Response = "I'm sorry, I couldn't generate inventory recommendations at the moment. Please try again later.",
                Type = "Error",
                RequiresAction = false
            };
        }
    }

    public async Task<CopilotResponseDto> GetSalesInsightsAsync(Guid businessId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Generating sales insights for business {BusinessId} from {FromDate} to {ToDate}", 
                businessId, fromDate, toDate);

            var prompt = $@"
As TOSS ERP's AI Business Copilot, analyze sales performance for a township/rural business (ID: {businessId}) 
for the period from {fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}.

Consider:
- Sales trends and patterns
- Peak and off-peak periods
- Product performance
- Customer behavior patterns
- Seasonal factors
- Market conditions

Provide:
1. Sales performance analysis
2. Trend identification
3. Growth opportunities
4. Risk factors
5. Actionable recommendations

Focus on practical insights for improving sales performance.
";

            var result = await _kernel.InvokePromptAsync(prompt, cancellationToken: cancellationToken);
            var response = result.GetValue<string>() ?? "Unable to generate sales insights at this time.";

            return new CopilotResponseDto
            {
                Response = response,
                Type = "SalesInsights",
                Actions = ExtractActions(response),
                Suggestions = ExtractSuggestions(response),
                RequiresAction = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating sales insights for business {BusinessId}", businessId);
            return new CopilotResponseDto
            {
                Response = "I'm sorry, I couldn't generate sales insights at the moment. Please try again later.",
                Type = "Error",
                RequiresAction = false
            };
        }
    }

    public async Task<CopilotResponseDto> GetFinancialRecommendationsAsync(Guid businessId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Generating financial recommendations for business {BusinessId}", businessId);

            var prompt = $@"
As TOSS ERP's AI Business Copilot, provide financial management recommendations for a township/rural business (ID: {businessId}).

Consider:
- Cash flow management
- Cost control strategies
- Pricing optimization
- Profit margin improvement
- Financial planning
- Risk management
- Investment opportunities

Provide:
1. Financial health assessment
2. Cash flow recommendations
3. Cost optimization strategies
4. Pricing advice
5. Investment suggestions
6. Risk mitigation strategies

Focus on practical financial management for small businesses.
";

            var result = await _kernel.InvokePromptAsync(prompt, cancellationToken: cancellationToken);
            var response = result.GetValue<string>() ?? "Unable to generate financial recommendations at this time.";

            return new CopilotResponseDto
            {
                Response = response,
                Type = "FinancialRecommendations",
                Actions = ExtractActions(response),
                Suggestions = ExtractSuggestions(response),
                RequiresAction = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating financial recommendations for business {BusinessId}", businessId);
            return new CopilotResponseDto
            {
                Response = "I'm sorry, I couldn't generate financial recommendations at the moment. Please try again later.",
                Type = "Error",
                RequiresAction = false
            };
        }
    }

    private static List<string> ExtractActions(string response)
    {
        var actions = new List<string>();
        var lines = response.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();
            if (trimmedLine.StartsWith("-") || trimmedLine.StartsWith("•") || trimmedLine.StartsWith("*"))
            {
                actions.Add(trimmedLine.Substring(1).Trim());
            }
        }
        
        return actions.Take(5).ToList(); // Limit to 5 actions
    }

    private static List<string> ExtractSuggestions(string response)
    {
        var suggestions = new List<string>();
        var lines = response.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();
            if (trimmedLine.Contains("suggest") || trimmedLine.Contains("recommend") || trimmedLine.Contains("consider"))
            {
                suggestions.Add(trimmedLine);
            }
        }
        
        return suggestions.Take(3).ToList(); // Limit to 3 suggestions
    }
} 
