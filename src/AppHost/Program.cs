using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// Infra: Postgres
var pg = builder.AddPostgres("postgres")
    .WithDataVolume();
var tossDb = pg.AddDatabase("toss-erp");

// Setup API
var setup = builder.AddProject("setupapi", "../Services/setup/Setup.API/Setup.API.csproj")
    .WithEnvironment("ASPNETCORE_URLS", "http://+:8080")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithReference(tossDb, "ConnectionStrings__TossErpDb")
    .WithEndpoint(port: 5010, targetPort: 8080, name: "http-setup");

// Assets API
var assets = builder.AddProject("assetsapi", "../Services/assets/Assets.API/Assets.API.csproj")
    .WithEnvironment("ASPNETCORE_URLS", "http://+:8080")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithReference(tossDb, "ConnectionStrings__TossErpDb")
    .WithEndpoint(port: 5020, targetPort: 8080, name: "http-assets");

// Accounts API
var accounts = builder.AddProject("accountsapi", "../Services/accounts/Accounts.API/Accounts.API.csproj")
    .WithEnvironment("ASPNETCORE_URLS", "http://+:8080")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithReference(tossDb, "ConnectionStrings__TossErpDb")
    .WithEndpoint(port: 5030, targetPort: 8080, name: "http-accounts");

// Projects API
var projects = builder.AddProject("projectsapi", "../Services/projects/Projects.API/Projects.API.csproj")
    .WithEnvironment("ASPNETCORE_URLS", "http://+:8080")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithReference(tossDb, "ConnectionStrings__TossErpDb")
    .WithEndpoint(port: 5040, targetPort: 8080, name: "http-projects");

// Sales API
var sales = builder.AddProject("salesapi", "../Services/sales/Sales.API/Sales.API.csproj")
    .WithEnvironment("ASPNETCORE_URLS", "http://+:8080")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithReference(tossDb, "ConnectionStrings__TossErpDb")
    .WithEndpoint(port: 5050, targetPort: 8080, name: "http-sales");

// CRM API
var crm = builder.AddProject("crmapi", "../Services/crm/Crm.API.csproj")
    .WithEnvironment("ASPNETCORE_URLS", "http://+:8080")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithReference(tossDb, "ConnectionStrings__TossErpDb")
    .WithEndpoint(port: 5060, targetPort: 8080, name: "http-crm");

// Collaboration API
var collaboration = builder.AddProject("collaborationapi", "../Services/collaboration/Collaboration.API.csproj")
    .WithEnvironment("ASPNETCORE_URLS", "http://+:8080")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithReference(tossDb, "ConnectionStrings__TossErpDb")
    .WithEndpoint(port: 5070, targetPort: 8080, name: "http-collaboration");

// Financial Services API
var financial = builder.AddProject("financialapi", "../Services/financial/Financial.API/Financial.API.csproj")
    .WithEnvironment("ASPNETCORE_URLS", "http://+:8080")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithReference(tossDb, "ConnectionStrings__TossErpDb")
    .WithEndpoint(port: 5080, targetPort: 8080, name: "http-financial");

// Stock API (Inventory)
var stock = builder.AddProject("stockapi", "../Services/Stock/Stock.API/Stock.API.csproj")
    .WithEnvironment("ASPNETCORE_URLS", "http://+:8080")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithReference(tossDb, "ConnectionStrings__TossErpDb")
    .WithEndpoint(port: 5001, targetPort: 8080, name: "http-stock");

// Gateway
var gateway = builder.AddProject("gateway", "../Gateway/Gateway.csproj")
    .WithEnvironment("ASPNETCORE_URLS", "http://+:8080")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEndpoint(port: 8081, targetPort: 8080, name: "http-gateway");

builder.Build().Run();


