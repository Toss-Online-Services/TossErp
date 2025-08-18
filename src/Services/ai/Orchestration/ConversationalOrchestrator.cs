using TossErp.AI.Services;
using TossErp.AI.Agents;

namespace TossErp.AI.Orchestration;

/// <summary>
/// Orchestrates conversational interactions and routes them to appropriate autonomous agents
/// </summary>
public class ConversationalOrchestrator : IConversationalOrchestrator
{
    private readonly IInventoryAgent _inventoryAgent;
    private readonly ISalesAgent _salesAgent;
    private readonly IPurchasingAgent _purchasingAgent;
    private readonly IFinanceAgent _financeAgent;
    private readonly ICustomerServiceAgent _customerServiceAgent;
    private readonly ILogger<ConversationalOrchestrator> _logger;

    public ConversationalOrchestrator(
        IInventoryAgent inventoryAgent,
        ISalesAgent salesAgent,
        IPurchasingAgent purchasingAgent,
        IFinanceAgent financeAgent,
        ICustomerServiceAgent customerServiceAgent,
        ILogger<ConversationalOrchestrator> logger)
    {
        _inventoryAgent = inventoryAgent;
        _salesAgent = salesAgent;
        _purchasingAgent = purchasingAgent;
        _financeAgent = financeAgent;
        _customerServiceAgent = customerServiceAgent;
        _logger = logger;
    }

    public async Task<ConversationResponse> ProcessConversationAsync(ConversationRequest request)
    {
        _logger.LogInformation("Processing conversation for user {UserId}: {Message}", request.UserId, request.Message);

        try
        {
            // Analyze the intent of the conversation
            var intent = await AnalyzeIntentAsync(request.Message, request.UserId);
            
            // Route to appropriate agent
            var response = await RouteToAgentAsync(intent, request.Message);
            
            // Add context from the request
            response.Data["userId"] = request.UserId;
            response.Data["language"] = request.Language;
            response.Data["channel"] = request.Channel;
            
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing conversation for user {UserId}", request.UserId);
            
            return new ConversationResponse
            {
                Message = "I'm sorry, I encountered an error processing your request. Please try again.",
                Actions = new List<AutonomousAction>(),
                Data = new Dictionary<string, object>(),
                RequiresConfirmation = false
            };
        }
    }

    public async Task<ServiceIntent> AnalyzeIntentAsync(string userInput, string userId)
    {
        _logger.LogDebug("Analyzing intent for user input: {UserInput}", userInput);

        // Simple keyword-based intent analysis
        // In a real implementation, this would use NLP/LLM for more sophisticated analysis
        var input = userInput.ToLower();
        
        var intent = new ServiceIntent
        {
            PrimaryService = DeterminePrimaryService(input),
            Action = DetermineAction(input),
            Parameters = ExtractParameters(input),
            IsAutomationRequest = input.Contains("automate") || input.Contains("auto"),
            RequiresApproval = input.Contains("approve") || input.Contains("confirm")
        };

        _logger.LogDebug("Intent analysis result: {Service}.{Action}", intent.PrimaryService, intent.Action);
        
        return intent;
    }

    public async Task<ConversationResponse> RouteToAgentAsync(ServiceIntent intent, string userInput)
    {
        _logger.LogDebug("Routing to agent: {Service}.{Action}", intent.PrimaryService, intent.Action);

        try
        {
            var response = intent.PrimaryService switch
            {
                "inventory" => await HandleInventoryRequest(intent, userInput),
                "sales" => await HandleSalesRequest(intent, userInput),
                "purchasing" => await HandlePurchasingRequest(intent, userInput),
                "finance" => await HandleFinanceRequest(intent, userInput),
                "customer_service" => await HandleCustomerServiceRequest(intent, userInput),
                _ => await HandleGeneralRequest(intent, userInput)
            };

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error routing to agent for service: {Service}", intent.PrimaryService);
            
            return new ConversationResponse
            {
                Message = "I'm sorry, I couldn't process your request. Please try rephrasing it.",
                Actions = new List<AutonomousAction>(),
                Data = new Dictionary<string, object>(),
                RequiresConfirmation = false
            };
        }
    }

    private async Task<ConversationResponse> HandleInventoryRequest(ServiceIntent intent, string userInput)
    {
        var response = new ConversationResponse();
        
        switch (intent.Action)
        {
            case "query":
                var insights = await _inventoryAgent.GetInventoryInsightsAsync("user123");
                response.Message = $"Here's your inventory summary: {insights.LowStockItems.Count} items need reordering, total value: R{insights.TotalInventoryValue:N2}";
                response.Data["insights"] = insights;
                break;
                
            case "automate":
                var result = await _inventoryAgent.MonitorAndReorderAsync("user123");
                response.Message = $"I've automatically checked your inventory. {result.ActionsPerformed.Count} actions performed, saved R{result.MoneySaved:N2}";
                response.Actions.Add(new AutonomousAction
                {
                    Service = "inventory",
                    Action = "monitor_and_reorder",
                    Parameters = intent.Parameters
                });
                break;
                
            default:
                response.Message = "I can help you with inventory management. What would you like to know?";
                break;
        }
        
        return response;
    }

    private async Task<ConversationResponse> HandleSalesRequest(ServiceIntent intent, string userInput)
    {
        var response = new ConversationResponse();
        
        switch (intent.Action)
        {
            case "query":
                var insights = await _salesAgent.GetSalesInsightsAsync("user123");
                response.Message = $"Sales summary: R{insights.TotalSales:N2} total sales, {insights.TotalTransactions} transactions";
                response.Data["insights"] = insights;
                break;
                
            case "automate":
                var result = await _salesAgent.ManageCustomerRelationshipsAsync("user123");
                response.Message = $"I've managed customer relationships. {result.ActionsPerformed.Count} actions performed";
                response.Actions.Add(new AutonomousAction
                {
                    Service = "sales",
                    Action = "manage_customer_relationships",
                    Parameters = intent.Parameters
                });
                break;
                
            default:
                response.Message = "I can help you with sales and customer management. What would you like to know?";
                break;
        }
        
        return response;
    }

    private async Task<ConversationResponse> HandlePurchasingRequest(ServiceIntent intent, string userInput)
    {
        var response = new ConversationResponse();
        
        switch (intent.Action)
        {
            case "query":
                var insights = await _purchasingAgent.GetPurchaseInsightsAsync("user123");
                response.Message = $"Purchase summary: R{insights.TotalPurchases:N2} total purchases, {insights.TotalOrders} orders";
                response.Data["insights"] = insights;
                break;
                
            case "automate":
                var result = await _purchasingAgent.ManageSuppliersAsync("user123");
                response.Message = $"I've managed supplier relationships. {result.ActionsPerformed.Count} actions performed, saved R{result.MoneySaved:N2}";
                response.Actions.Add(new AutonomousAction
                {
                    Service = "purchasing",
                    Action = "manage_suppliers",
                    Parameters = intent.Parameters
                });
                break;
                
            default:
                response.Message = "I can help you with purchasing and supplier management. What would you like to know?";
                break;
        }
        
        return response;
    }

    private async Task<ConversationResponse> HandleFinanceRequest(ServiceIntent intent, string userInput)
    {
        var response = new ConversationResponse();
        
        switch (intent.Action)
        {
            case "query":
                var insights = await _financeAgent.GetFinancialInsightsAsync("user123");
                response.Message = $"Financial summary: R{insights.TotalRevenue:N2} revenue, R{insights.NetProfit:N2} net profit";
                response.Data["insights"] = insights;
                break;
                
            case "automate":
                var result = await _financeAgent.MonitorCashFlowAsync("user123");
                response.Message = $"I've monitored your cash flow. {result.ActionsPerformed.Count} actions performed";
                response.Actions.Add(new AutonomousAction
                {
                    Service = "finance",
                    Action = "monitor_cash_flow",
                    Parameters = intent.Parameters
                });
                break;
                
            default:
                response.Message = "I can help you with financial management and reporting. What would you like to know?";
                break;
        }
        
        return response;
    }

    private async Task<ConversationResponse> HandleCustomerServiceRequest(ServiceIntent intent, string userInput)
    {
        var response = new ConversationResponse();
        
        switch (intent.Action)
        {
            case "query":
                var insights = await _customerServiceAgent.GetCustomerServiceInsightsAsync("user123");
                response.Message = $"Customer service summary: {insights.TotalInquiries} inquiries, {insights.ResolutionRate:P0} resolution rate";
                response.Data["insights"] = insights;
                break;
                
            case "automate":
                var result = await _customerServiceAgent.ManageCustomerRelationshipsAsync("user123");
                response.Message = $"I've managed customer relationships. {result.ActionsTaken.Count} actions taken";
                response.Actions.Add(new AutonomousAction
                {
                    Service = "customer_service",
                    Action = "manage_customer_relationships",
                    Parameters = intent.Parameters
                });
                break;
                
            default:
                response.Message = "I can help you with customer service and support. What would you like to know?";
                break;
        }
        
        return response;
    }

    private async Task<ConversationResponse> HandleGeneralRequest(ServiceIntent intent, string userInput)
    {
        return new ConversationResponse
        {
            Message = "I can help you with inventory, sales, purchasing, finance, and customer service. What would you like to know?",
            Actions = new List<AutonomousAction>(),
            Data = new Dictionary<string, object>(),
            RequiresConfirmation = false
        };
    }

    private string DeterminePrimaryService(string input)
    {
        if (input.Contains("stock") || input.Contains("inventory") || input.Contains("item"))
            return "inventory";
        if (input.Contains("sale") || input.Contains("customer") || input.Contains("invoice"))
            return "sales";
        if (input.Contains("purchase") || input.Contains("supplier") || input.Contains("order"))
            return "purchasing";
        if (input.Contains("finance") || input.Contains("money") || input.Contains("cash") || input.Contains("report"))
            return "finance";
        if (input.Contains("support") || input.Contains("help") || input.Contains("service"))
            return "customer_service";
        
        return "general";
    }

    private string DetermineAction(string input)
    {
        if (input.Contains("show") || input.Contains("what") || input.Contains("how") || input.Contains("summary"))
            return "query";
        if (input.Contains("automate") || input.Contains("auto") || input.Contains("monitor"))
            return "automate";
        if (input.Contains("create") || input.Contains("add") || input.Contains("new"))
            return "create";
        if (input.Contains("update") || input.Contains("change") || input.Contains("modify"))
            return "update";
        
        return "query";
    }

    private Dictionary<string, object> ExtractParameters(string input)
    {
        var parameters = new Dictionary<string, object>();
        
        // Extract simple parameters based on keywords
        if (input.Contains("today"))
            parameters["date"] = DateTime.Today;
        if (input.Contains("week"))
            parameters["period"] = "weekly";
        if (input.Contains("month"))
            parameters["period"] = "monthly";
        
        return parameters;
    }
}

