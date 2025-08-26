using Aspire.Hosting;
using TossErp.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

// Add forwarded headers for production deployments
builder.AddForwardedHeaders();

// Infrastructure
var redis = builder.AddTossRedis("redis");

var rabbitMq = builder.AddTossRabbitMQ("eventbus");

var postgres = builder.AddTossPostgreSQL("postgres");

// Databases
var tossDb = postgres.AddDatabase("toss-erp");
var identityDb = postgres.AddDatabase("identitydb");
var crmDb = postgres.AddDatabase("crmdb");
var hrDb = postgres.AddDatabase("hrdb");
var financialDb = postgres.AddDatabase("financialdb");
var logisticsDb = postgres.AddDatabase("logisticsdb");

var launchProfileName = ShouldUseHttpForEndpoints() ? "http" : "https";

// Core Services
var identityApi = builder.AddProject("identity-api", "../Services/identity/Identity.API.csproj")
    .WithExternalHttpEndpoints()
    .WithReference(identityDb)
    .WithReference(rabbitMq).WaitFor(rabbitMq)
    .WithEndpoint(port: 5000, name: "http-identity");

var identityEndpoint = identityApi.GetEndpoint("http-identity");

// Business Services
var setupApi = builder.AddProject("setup-api", "../Services/setup/Setup.API/Setup.API.csproj")
    .WithReference(tossDb)
    .WithReference(rabbitMq).WaitFor(rabbitMq)
    .WithEnvironment("Identity__Url", identityEndpoint)
    .WithEndpoint(port: 5010, name: "http-setup");

// Temporarily disabled due to domain model conflicts
// var crmApi = builder.AddProject("crm-api", "../Services/crm/Crm.API/Crm.API.csproj")
//     .WithReference(crmDb)
//     .WithReference(rabbitMq).WaitFor(rabbitMq)
//     .WithReference(redis)
//     .WithEnvironment("Identity__Url", identityEndpoint)
//     .WithEndpoint(port: 5060, name: "http-crm");

var hrApi = builder.AddProject("hr-api", "../Services/hr/HR.API/HR.API.csproj")
    .WithReference(hrDb)
    .WithReference(rabbitMq).WaitFor(rabbitMq)
    .WithEnvironment("Identity__Url", identityEndpoint)
    .WithEndpoint(port: 5090, name: "http-hr");

var salesApi = builder.AddProject("sales-api", "../Services/sales/Sales.API.csproj")
    .WithReference(tossDb)
    .WithReference(rabbitMq).WaitFor(rabbitMq)
    .WithEnvironment("Identity__Url", identityEndpoint)
    .WithEndpoint(port: 5050, name: "http-sales");

var stockApi = builder.AddProject("stock-api", "../Services/Stock/Stock.API/Stock.API.csproj")
    .WithReference(tossDb)
    .WithReference(rabbitMq).WaitFor(rabbitMq)
    .WithEnvironment("Identity__Url", identityEndpoint)
    .WithEndpoint(port: 5001, name: "http-stock");

var aiApi = builder.AddProject("ai-api", "../Services/ai/Ai.API.csproj")
    .WithReference(tossDb)
    .WithReference(rabbitMq).WaitFor(rabbitMq)
    .WithEnvironment("Identity__Url", identityEndpoint)
    .WithEndpoint(port: 5100, name: "http-ai");

var logisticsApi = builder.AddProject("logistics-api", "../Services/logistics/Logistics.API/Logistics.API.csproj")
    .WithReference(logisticsDb)
    .WithReference(rabbitMq).WaitFor(rabbitMq)
    .WithEnvironment("Identity__Url", identityEndpoint)
    .WithEndpoint(port: 5110, name: "http-logistics");

var financialApi = builder.AddProject("financial-api", "../Services/financial/Financial.API/Financial.API.csproj")
    .WithReference(financialDb)
    .WithReference(rabbitMq).WaitFor(rabbitMq)
    .WithEnvironment("Identity__Url", identityEndpoint)
    .WithEndpoint(port: 5080, name: "http-financial");

var collaborationApi = builder.AddProject("collaboration-api", "../Services/collaboration/Collaboration.API.csproj")
    .WithReference(tossDb)
    .WithReference(rabbitMq).WaitFor(rabbitMq)
    .WithEnvironment("Identity__Url", identityEndpoint)
    .WithEndpoint(port: 5070, name: "http-collaboration");

var assetsApi = builder.AddProject("assets-api", "../Services/assets/Assets.API/Assets.API.csproj")
    .WithReference(tossDb)
    .WithReference(rabbitMq).WaitFor(rabbitMq)
    .WithEnvironment("Identity__Url", identityEndpoint)
    .WithEndpoint(port: 5020, name: "http-assets");

var accountsApi = builder.AddProject("accounts-api", "../Services/accounts/Accounts.API/Accounts.API.csproj")
    .WithReference(tossDb)
    .WithReference(rabbitMq).WaitFor(rabbitMq)
    .WithEnvironment("Identity__Url", identityEndpoint)
    .WithEndpoint(port: 5030, name: "http-accounts");

var projectsApi = builder.AddProject("projects-api", "../Services/projects/Projects.API/Projects.API.csproj")
    .WithReference(tossDb)
    .WithReference(rabbitMq).WaitFor(rabbitMq)
    .WithEnvironment("Identity__Url", identityEndpoint)
    .WithEndpoint(port: 5041, name: "http-projects");

// Background Processors
var stockProcessor = builder.AddProject("stock-processor", "../Services/Stock/Stock.Processor/Stock.Processor.csproj")
    .WithReference(rabbitMq).WaitFor(rabbitMq)
    .WithReference(tossDb)
    .WaitFor(stockApi);

// Gateway
var gateway = builder.AddProject("gateway", "../Gateway/Gateway.csproj")
    .WithExternalHttpEndpoints()
    .WithReference(identityApi)
    // .WithReference(crmApi)  // Temporarily disabled
    .WithReference(hrApi)
    .WithReference(salesApi)
    .WithReference(stockApi)
    .WithReference(aiApi)
    .WithReference(logisticsApi)
    .WithReference(financialApi)
    .WithReference(collaborationApi)
    .WithReference(assetsApi)
    .WithReference(accountsApi)
    .WithReference(projectsApi)
    .WithEndpoint(port: 8081, name: "http-gateway");

// AI Integration (optional) - Temporarily disabled due to CRM dependency
bool useOpenAI = false;
if (useOpenAI)
{
    // builder.AddOpenAI(aiApi, crmApi);  // Disabled - requires CRM
}

bool useOllama = false;
if (useOllama)
{
    // builder.AddOllama(aiApi, crmApi);  // Disabled - requires CRM
}

// Wire up callback URLs and cross-references
gateway.WithEnvironment("CallBackUrl", gateway.GetEndpoint("http-gateway"));

// Identity has references to all APIs for callback URLs
identityApi// .WithEnvironment("CrmApiClient", crmApi.GetEndpoint("http"))  // CRM temporarily disabled
           .WithEnvironment("HrApiClient", hrApi.GetEndpoint("http"))
           .WithEnvironment("SalesApiClient", salesApi.GetEndpoint("http"))
           .WithEnvironment("StockApiClient", stockApi.GetEndpoint("http"))
           .WithEnvironment("AiApiClient", aiApi.GetEndpoint("http"))
           .WithEnvironment("LogisticsApiClient", logisticsApi.GetEndpoint("http"))
           .WithEnvironment("FinancialApiClient", financialApi.GetEndpoint("http"))
           .WithEnvironment("CollaborationApiClient", collaborationApi.GetEndpoint("http"))
           .WithEnvironment("AssetsApiClient", assetsApi.GetEndpoint("http"))
           .WithEnvironment("AccountsApiClient", accountsApi.GetEndpoint("http"))
           .WithEnvironment("ProjectsApiClient", projectsApi.GetEndpoint("http"))
           .WithEnvironment("GatewayClient", gateway.GetEndpoint("http-gateway"));

builder.Build().Run();

// For test use only.
// Looks for an environment variable that forces the use of HTTP for all the endpoints. We
// are doing this for ease of running tests in CI.
static bool ShouldUseHttpForEndpoints()
{
    const string EnvVarName = "TOSSERP_USE_HTTP_ENDPOINTS";
    var envValue = Environment.GetEnvironmentVariable(EnvVarName);

    // Attempt to parse the environment variable value; return true if it's exactly "1".
    return int.TryParse(envValue, out int result) && result == 1;
}


