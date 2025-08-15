using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// Infra: Postgres
var pg = builder.AddPostgres("postgres")
    .WithDataVolume();
var inventoryDb = pg.AddDatabase("toss-inventory");

// Stock API (Inventory)
var stock = builder.AddProject("stockapi", "../Services/Stock/Stock.API/Stock.API.csproj")
    .WithEnvironment("ASPNETCORE_URLS", "http://+:8080")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithReference(inventoryDb, "ConnectionStrings__TossErpDb")
    .WithEndpoint(port: 5001, targetPort: 8080, name: "http-stock");

// Gateway
var gateway = builder.AddProject("gateway", "../Gateway/Gateway.csproj")
    .WithEnvironment("ASPNETCORE_URLS", "http://+:8080")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEndpoint(port: 8081, targetPort: 8080, name: "http-gateway");

builder.Build().Run();


