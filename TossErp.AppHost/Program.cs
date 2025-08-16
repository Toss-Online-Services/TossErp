using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddLogging(logging => logging.AddConsole());

// Infrastructure Services
var postgres = builder.AddPostgres("stock-postgres")
    .AddDatabase("tosserp_stock");

var redis = builder.AddRedis("stock-redis");

var rabbitmq = builder.AddRabbitMQ("stock-rabbitmq");

// Stock API Service
var stockApi = builder.AddProject<Projects.Stock_API>("stock-api")
    .WithReference(postgres)
    .WithReference(redis)
    .WithReference(rabbitmq)
    .WithEnvironment("OpenAI__ApiKey", builder.Configuration["OpenAI:ApiKey"] ?? "");

// Stock Processor Service
var stockProcessor = builder.AddProject<Projects.Stock_Processor>("stock-processor")
    .WithReference(postgres)
    .WithReference(redis)
    .WithReference(rabbitmq);

// API Gateway
var gateway = builder.AddProject<Projects.Gateway>("gateway")
    .WithReference(stockApi)
    .WithReference(postgres)
    .WithReference(redis)
    .WithReference(rabbitmq);

// Client Applications
var mobileClient = builder.AddProject<Projects.Mobile_Client>("mobile-client")
    .WithReference(gateway);

var webClient = builder.AddProject<Projects.Web_Client>("web-client")
    .WithReference(gateway);

var adminClient = builder.AddProject<Projects.Admin_Client>("admin-client")
    .WithReference(gateway);

// Monitoring (Optional)
if (builder.Configuration.GetValue<bool>("EnableMonitoring", false))
{
    var prometheus = builder.AddContainer("stock-prometheus", "prom/prometheus:latest")
        .WithVolumeMount("./infra/k8s/stock/stock-monitoring.yaml", "/etc/prometheus/prometheus.yml")
        .WithVolumeMount("stock-prometheus-data", "/prometheus")
        .WithCommand("--config.file=/etc/prometheus/prometheus.yml", "--storage.tsdb.path=/prometheus")
        .WithHttpEndpoint(9090, name: "prometheus");

    var grafana = builder.AddContainer("stock-grafana", "grafana/grafana:latest")
        .WithEnvironment("GF_SECURITY_ADMIN_PASSWORD", "admin123")
        .WithVolumeMount("stock-grafana-data", "/var/lib/grafana")
        .WithHttpEndpoint(3001, name: "grafana")
        .WithReference(prometheus);
}

// Build and run the application
var app = builder.Build();

// Configure the application
app.MapDefaultEndpoints();

// Start the application
await app.RunAsync();
