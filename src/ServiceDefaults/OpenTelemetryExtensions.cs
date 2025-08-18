using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace eShop.ServiceDefaults;

/// <summary>
/// Enhanced OpenTelemetry configuration for TOSS ERP
/// </summary>
public static class OpenTelemetryExtensions
{
    /// <summary>
    /// Add enhanced OpenTelemetry configuration
    /// </summary>
    public static IHostApplicationBuilder AddEnhancedOpenTelemetry(this IHostApplicationBuilder builder)
    {
        // Create application-specific activity source and meter
        var serviceName = builder.Environment.ApplicationName;
        var serviceVersion = "1.0.0"; // Could be read from assembly or configuration

        // Register activity source and meter as singletons
        builder.Services.AddSingleton(_ => new ActivitySource(serviceName));
        builder.Services.AddSingleton(_ => new Meter(serviceName, serviceVersion));

        // Configure resource attributes
        var resourceBuilder = ResourceBuilder.CreateDefault()
            .AddService(serviceName, serviceVersion)
            .AddAttributes(new Dictionary<string, object>
            {
                ["service.environment"] = builder.Environment.EnvironmentName,
                ["service.instance.id"] = Environment.MachineName,
                ["deployment.environment"] = builder.Environment.EnvironmentName
            });

        // Add OpenTelemetry
        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName, serviceVersion))
            .WithTracing(tracing =>
            {
                tracing
                    .SetResourceBuilder(resourceBuilder)
                    .AddSource(serviceName)
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.RecordException = true;
                        options.EnrichWithHttpRequest = EnrichWithHttpRequest;
                        options.EnrichWithHttpResponse = EnrichWithHttpResponse;
                        options.Filter = FilterHttpRequests;
                    })
                    .AddHttpClientInstrumentation(options =>
                    {
                        options.RecordException = true;
                        options.EnrichWithHttpRequestMessage = EnrichWithHttpRequestMessage;
                        options.EnrichWithHttpResponseMessage = EnrichWithHttpResponseMessage;
                    })
                    .AddGrpcClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation(options =>
                    {
                        options.SetDbStatementForText = true;
                        options.SetDbStatementForStoredProcedure = true;
                        options.EnrichWithIDbCommand = EnrichWithDbCommand;
                    })
                    .AddRedisInstrumentation()
                    .AddSqlClientInstrumentation(options =>
                    {
                        options.SetDbStatementForText = true;
                        options.SetDbStatementForStoredProcedure = true;
                        options.RecordException = true;
                    });

                // Add development sampling
                if (builder.Environment.IsDevelopment())
                {
                    tracing.SetSampler(new AlwaysOnSampler());
                }
                else
                {
                    // Use trace-based sampling in production
                    tracing.SetSampler(new TraceIdRatioBasedSampler(0.1)); // 10% sampling
                }
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .SetResourceBuilder(resourceBuilder)
                    .AddMeter(serviceName)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddProcessInstrumentation()
                    .AddView("http.server.request.duration", new ExplicitBucketHistogramConfiguration
                    {
                        Boundaries = new double[] { 0, 0.005, 0.01, 0.025, 0.05, 0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10 }
                    });
            });

        // Add exporters
        builder.AddOpenTelemetryExporters();

        return builder;
    }

    /// <summary>
    /// Add OpenTelemetry middleware for tracing
    /// </summary>
    public static IApplicationBuilder UseEnhancedOpenTelemetry(this IApplicationBuilder app)
    {
        // Custom middleware to add additional trace enrichment
        app.Use(async (context, next) =>
        {
            var activity = Activity.Current;
            if (activity != null)
            {
                // Add correlation ID
                if (context.Items.TryGetValue("CorrelationId", out var correlationId))
                {
                    activity.SetTag("correlation.id", correlationId?.ToString());
                }

                // Add tenant ID
                if (context.Items.TryGetValue("TenantId", out var tenantId))
                {
                    activity.SetTag("tenant.id", tenantId?.ToString());
                }

                // Add user information
                if (context.User.Identity?.IsAuthenticated == true)
                {
                    activity.SetTag("user.id", context.User.FindFirst("sub")?.Value ?? context.User.FindFirst("id")?.Value);
                    activity.SetTag("user.email", context.User.FindFirst("email")?.Value);
                }
            }

            await next();
        });

        return app;
    }

    #region Private Helper Methods

    private static void EnrichWithHttpRequest(Activity activity, Microsoft.AspNetCore.Http.HttpRequest request)
    {
        activity.SetTag("http.request.header.user_agent", request.Headers["User-Agent"].FirstOrDefault());
        activity.SetTag("http.request.header.x_forwarded_for", request.Headers["X-Forwarded-For"].FirstOrDefault());
        activity.SetTag("http.request.content_length", request.ContentLength?.ToString());
    }

    private static void EnrichWithHttpResponse(Activity activity, Microsoft.AspNetCore.Http.HttpResponse response)
    {
        activity.SetTag("http.response.content_length", response.ContentLength?.ToString());
        activity.SetTag("http.response.content_type", response.ContentType);
    }

    private static void EnrichWithHttpRequestMessage(Activity activity, HttpRequestMessage request)
    {
        activity.SetTag("http.client.request.content_length", request.Content?.Headers?.ContentLength?.ToString());
        activity.SetTag("http.client.request.content_type", request.Content?.Headers?.ContentType?.ToString());
    }

    private static void EnrichWithHttpResponseMessage(Activity activity, HttpResponseMessage response)
    {
        activity.SetTag("http.client.response.content_length", response.Content?.Headers?.ContentLength?.ToString());
        activity.SetTag("http.client.response.content_type", response.Content?.Headers?.ContentType?.ToString());
    }

    private static void EnrichWithDbCommand(Activity activity, System.Data.IDbCommand command)
    {
        activity.SetTag("db.connection.string", SanitizeConnectionString(command.Connection?.ConnectionString));
        activity.SetTag("db.command.timeout", command.CommandTimeout.ToString());
    }

    private static bool FilterHttpRequests(Microsoft.AspNetCore.Http.HttpContext context)
    {
        // Don't trace health check endpoints
        var path = context.Request.Path.Value?.ToLowerInvariant();
        return !path?.Contains("/health") == true && !path?.Contains("/metrics") == true;
    }

    private static string? SanitizeConnectionString(string? connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            return null;

        // Remove sensitive information from connection string
        var parts = connectionString.Split(';');
        var sanitized = parts.Where(part =>
            !part.Trim().StartsWith("Password", StringComparison.OrdinalIgnoreCase) &&
            !part.Trim().StartsWith("Pwd", StringComparison.OrdinalIgnoreCase) &&
            !part.Trim().StartsWith("User ID", StringComparison.OrdinalIgnoreCase) &&
            !part.Trim().StartsWith("UID", StringComparison.OrdinalIgnoreCase));

        return string.Join(";", sanitized);
    }

    #endregion
}

/// <summary>
/// Custom metrics for TOSS ERP
/// </summary>
public static class TossMetrics
{
    private static readonly Meter Meter = new("TossErp.Custom", "1.0.0");

    // Business metrics
    public static readonly Counter<int> SalesCounter = Meter.CreateCounter<int>("toss.sales.count", "count", "Number of sales transactions");
    public static readonly Histogram<double> SalesAmountHistogram = Meter.CreateHistogram<double>("toss.sales.amount", "currency", "Sales transaction amounts");
    public static readonly Counter<int> StockAdjustmentsCounter = Meter.CreateCounter<int>("toss.stock.adjustments.count", "count", "Number of stock adjustments");
    public static readonly Gauge<int> LowStockItemsGauge = Meter.CreateGauge<int>("toss.stock.low_items", "count", "Number of items with low stock");

    // Technical metrics
    public static readonly Counter<int> EventsPublishedCounter = Meter.CreateCounter<int>("toss.events.published.count", "count", "Number of events published");
    public static readonly Counter<int> EventsConsumedCounter = Meter.CreateCounter<int>("toss.events.consumed.count", "count", "Number of events consumed");
    public static readonly Histogram<double> DatabaseQueryDuration = Meter.CreateHistogram<double>("toss.database.query.duration", "ms", "Database query execution time");
    public static readonly Counter<int> CacheHitsCounter = Meter.CreateCounter<int>("toss.cache.hits.count", "count", "Number of cache hits");
    public static readonly Counter<int> CacheMissesCounter = Meter.CreateCounter<int>("toss.cache.misses.count", "count", "Number of cache misses");
}
