using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using Shared.Infrastructure.Idempotency;
using Microsoft.Extensions.Http.Resilience;
using System.Net;
using Polly; // For TimeoutRejectedException
using Shared.LLMAdapter; // Resilience metrics

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

// Resilient named HttpClient for OpenAI (only created if OPENAI_API_KEY present)
builder.Services.AddHttpClient("openai", client =>
{
    client.BaseAddress = new Uri("https://api.openai.com/");
    client.Timeout = TimeSpan.FromSeconds(30); // overall client timeout safeguard
})
// Custom resilience pipeline (retry -> circuit breaker -> per-attempt timeout)
// We don't use AddStandardResilienceHandler because we want to fine-tune ShouldHandle for POST semantics (avoid duplicating successful completions)
// Retry only on: 429 (rate limit), 408, >=500, HttpRequestException, TimeoutRejectedException
    .AddResilienceHandler("openai-pipeline", (builderCtx, context) =>
    {
        var loggerFactory = context.ServiceProvider.GetService<ILoggerFactory>();
        var logger = loggerFactory?.CreateLogger("OpenAIHttpResilience");

        builderCtx.AddRetry(new HttpRetryStrategyOptions
        {
            BackoffType = DelayBackoffType.Exponential,
            UseJitter = true,
            MaxRetryAttempts = 3,
            Delay = TimeSpan.FromSeconds(1),
            ShouldHandle = static args =>
            {
                // Pattern match acceptable transient faults
                if (args.Outcome.Exception is TimeoutRejectedException || args.Outcome.Exception is HttpRequestException)
                    return ValueTask.FromResult(true);
                if (args.Outcome.Result is { } resp)
                {
                    var sc = (int)resp.StatusCode;
                    if (sc == 408 || sc == 429 || sc >= 500)
                        return ValueTask.FromResult(true);
                }
                return ValueTask.FromResult(false);
            },
            OnRetry = args =>
            {
                logger?.LogWarning("OpenAI retry attempt {Attempt} delay {Delay} status {Status} exception {Exception}",
                    args.AttemptNumber + 1,
                    args.RetryDelay,
                    args.Outcome.Result?.StatusCode,
                    args.Outcome.Exception?.GetType().Name);
                ResilienceMetrics.OpenAiRetryAttempts.Add(1);
                return default;
            }
        });

        builderCtx.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
        {
            FailureRatio = 0.5,               // open when >=50% failures in sampling window
            SamplingDuration = TimeSpan.FromSeconds(30),
            MinimumThroughput = 20,            // need at least 20 requests in window to evaluate
            BreakDuration = TimeSpan.FromSeconds(5),
            ShouldHandle = static args =>
            {
                if (args.Outcome.Exception is TimeoutRejectedException || args.Outcome.Exception is HttpRequestException)
                    return ValueTask.FromResult(true);
                if (args.Outcome.Result is { } resp)
                {
                    var sc = (int)resp.StatusCode;
                    if (sc == 408 || sc == 429 || sc >= 500)
                        return ValueTask.FromResult(true);
                }
                return ValueTask.FromResult(false);
            },
            OnOpened = _ =>
            {
                logger?.LogError("OpenAI circuit opened (breaking calls for 5s)");
                ResilienceMetrics.OpenAiCircuitOpened.Add(1);
                return default;
            },
            OnClosed = _ =>
            {
                logger?.LogInformation("OpenAI circuit closed (resuming calls)");
                return default;
            },
            OnHalfOpened = _ =>
            {
                logger?.LogInformation("OpenAI circuit half-open (probing)");
                return default;
            }
        });

        // Per-attempt timeout (each HTTP attempt) distinct from HttpClient.Timeout
        builderCtx.AddTimeout(TimeSpan.FromSeconds(10));
    });

// Dynamic LLM adapter factory: prefer OpenAI if env vars present else stub
builder.Services.AddHttpClient(); // generic fallback factory if needed elsewhere
builder.Services.AddTenantContext();
builder.Services.AddSingleton<Shared.LLMAdapter.ILLMAdapter>(sp =>
{
    var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
    var model = Environment.GetEnvironmentVariable("LLM_MODEL") ?? "gpt-4o-mini";
    if (!string.IsNullOrWhiteSpace(apiKey))
    {
        var http = sp.GetRequiredService<IHttpClientFactory>().CreateClient("openai");
        var opts = new Shared.LLMAdapter.OpenAIOptions(apiKey, model);
        var logger = sp.GetService<ILogger<Shared.LLMAdapter.OpenAIAdapter>>();
        return new Shared.LLMAdapter.OpenAIAdapter(http, opts, logger);
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
app.UseMiddleware<AgentManager.ResilienceExceptionMiddleware>();
app.UseIdempotency();
app.UseTenantContext();
app.UseAuthorization();
app.MapControllers();

app.Run();
