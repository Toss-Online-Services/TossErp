using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using Shared.Infrastructure.Idempotency;

var builder = WebApplication.CreateBuilder(args);

// Serilog configuration (console)
builder.Host.UseSerilog((ctx, cfg) =>
{
    cfg.Enrich.FromLogContext()
       .WriteTo.Console();
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// OpenTelemetry (basic console exporter)
builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("AgentManager"))
    .WithTracing(t => t
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddSource("LLMAdapter")
        .AddConsoleExporter())
    .WithMetrics(m => m
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddRuntimeInstrumentation()
        .AddConsoleExporter());

// Dynamic LLM adapter factory: prefer OpenAI if env vars present else stub
builder.Services.AddHttpClient();
builder.Services.AddTenantContext();
builder.Services.AddSingleton<Shared.LLMAdapter.ILLMAdapter>(sp =>
{
    var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
    var model = Environment.GetEnvironmentVariable("LLM_MODEL") ?? "gpt-4o-mini";
    if (!string.IsNullOrWhiteSpace(apiKey))
    {
        var http = sp.GetRequiredService<IHttpClientFactory>().CreateClient("openai");
        var opts = new Shared.LLMAdapter.OpenAIOptions(apiKey, model);
        return new Shared.LLMAdapter.OpenAIAdapter(http, opts);
    }
    return new Shared.LLMAdapter.StubLLMAdapter();
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
