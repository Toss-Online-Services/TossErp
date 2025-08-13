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
    .WithEndpoint(name: "http-stock", port: 5001, targetPort: 8080);

// Gateway
var gateway = builder.AddProject("gateway", "../../gateway/Gateway.csproj")
    .WithEnvironment("ASPNETCORE_URLS", "http://+:8080")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEndpoint(name: "http-gateway", port: 8080, targetPort: 8080);

// Web (Nuxt frontend)
var web = builder.AddNpmApp("web", "../../TossErp.Web")
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .WithReference(gateway);

builder.Build().Run();


