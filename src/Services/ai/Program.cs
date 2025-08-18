using Microsoft.OpenApi.Models;
using TossErp.AI.Services;
using TossErp.AI.Orchestration;
using TossErp.AI.Agents;
using TossErp.AI.Automation;
using TossErp.AI.BackgroundServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "TOSS AI Service-as-a-Software API", 
        Version = "v1",
        Description = "AI-powered autonomous business services for SMMEs"
    });
});

// Add AI Services
builder.Services.AddScoped<IConversationalOrchestrator, ConversationalOrchestrator>();
builder.Services.AddScoped<IAutonomousAgentManager, AutonomousAgentManager>();
builder.Services.AddScoped<IServiceAutomationEngine, ServiceAutomationEngine>();
builder.Services.AddScoped<IBusinessOutcomeTracker, BusinessOutcomeTracker>();

// Add specialized agents
builder.Services.AddScoped<IInventoryAgent, InventoryAgent>();
builder.Services.AddScoped<ISalesAgent, SalesAgent>();
builder.Services.AddScoped<IPurchasingAgent, PurchasingAgent>();
builder.Services.AddScoped<IFinanceAgent, FinanceAgent>();
builder.Services.AddScoped<ICustomerServiceAgent, CustomerServiceAgent>();

// Add background services
builder.Services.AddHostedService<ProactiveServiceMonitor>();
builder.Services.AddHostedService<AutomationScheduler>();

// Add HTTP client for external integrations
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Service-as-a-Software endpoints
app.MapPost("/api/ai/conversation", async (ConversationRequest request, IConversationalOrchestrator orchestrator) =>
{
    var response = await orchestrator.ProcessConversationAsync(request);
    return Results.Ok(response);
});

app.MapPost("/api/ai/automate", async (AutomationRequest request, IServiceAutomationEngine engine) =>
{
    var result = await engine.ExecuteAutomationAsync(request);
    return Results.Ok(result);
});

app.MapGet("/api/ai/services/status", async (IAutonomousAgentManager manager) =>
{
    var status = await manager.GetServiceStatusAsync();
    return Results.Ok(status);
});

app.MapGet("/api/ai/outcomes", async (IBusinessOutcomeTracker tracker) =>
{
    var outcomes = await tracker.GetBusinessOutcomesAsync();
    return Results.Ok(outcomes);
});

app.MapGet("/", () => "TOSS AI Service-as-a-Software Platform Running");

app.Run();



