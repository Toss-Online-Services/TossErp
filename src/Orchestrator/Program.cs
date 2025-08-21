using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using Shared.Infrastructure.Idempotency;
using Orchestrator.Tenancy;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, cfg) =>
{
    cfg.Enrich.FromLogContext().WriteTo.Console();
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTenantContext();

builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("Orchestrator"))
    .WithTracing(t => t
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddSource("TemporalOrchestration")
        .AddConsoleExporter())
    .WithMetrics(m => m
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddRuntimeInstrumentation()
        .AddConsoleExporter());

// Temporal client (lazy) registration
builder.Services.AddSingleton<Func<Task<Temporalio.Client.TemporalClient>>>(sp =>
{
    return async () =>
    {
        var target = Environment.GetEnvironmentVariable("TEMPORAL_TARGET") ?? "localhost:7233";
        var ns = Environment.GetEnvironmentVariable("TEMPORAL_NAMESPACE") ?? "default";
        return await Temporalio.Client.TemporalClient.ConnectAsync(new Temporalio.Client.TemporalClientConnectOptions(target)
        {
            Namespace = ns
        });
    };
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseIdempotency();
app.UseTenantContext();
app.UseAuthorization();
app.MapControllers();

app.Run();
