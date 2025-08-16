using Stock.Processor;
using TossErp.Stock.Infrastructure;
using TossErp.Stock.Application;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

var builder = Host.CreateApplicationBuilder(args);

// Add configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();

// Add application and infrastructure services
builder.Services.AddApplicationServices(builder);
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add background services
builder.Services.AddHostedService<StockProcessorWorker>();
builder.Services.AddHostedService<LowStockAlertWorker>();
builder.Services.AddHostedService<StockReconciliationWorker>();

var host = builder.Build();
host.Run();
