var builder = DistributedApplication.CreateBuilder(args);

var databaseName = "TossDb";

var postgres = builder
    .AddPostgres("postgres")
    // Set the name of the default database to auto-create on container startup.
    .WithEnvironment("POSTGRES_DB", databaseName)
    .WithPgAdmin();

var database = postgres.AddDatabase(databaseName);

// Use AddProject with explicit configuration to avoid IDE execution mode issues
var web = builder.AddProject<Projects.Web>("web")
    .WithReference(database)
    .WaitFor(database)
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development");

builder.Build().Run();
