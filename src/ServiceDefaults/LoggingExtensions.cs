using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Serilog.Formatting.Compact;

namespace eShop.ServiceDefaults;

/// <summary>
/// Extensions for structured logging configuration
/// </summary>
public static class LoggingExtensions
{
    /// <summary>
    /// Add structured logging with Serilog
    /// </summary>
    public static IHostApplicationBuilder AddStructuredLogging(this IHostApplicationBuilder builder)
    {
        // Configure Serilog
        Log.Logger = CreateSerilogLogger(builder.Environment, builder.Configuration);

        // Clear default providers and add Serilog
        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(Log.Logger);

        return builder;
    }

    /// <summary>
    /// Use structured logging middleware
    /// </summary>
    public static IApplicationBuilder UseStructuredLogging(this IApplicationBuilder app)
    {
        // Add Serilog request logging
        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
            options.GetLevel = GetLogLevel;
            options.EnrichDiagnosticContext = EnrichFromRequest;
        });

        return app;
    }

    private static ILogger CreateSerilogLogger(IHostEnvironment environment, Microsoft.Extensions.Configuration.IConfiguration configuration)
    {
        var loggerConfig = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .MinimumLevel.Override("MassTransit", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Environment", environment.EnvironmentName)
            .Enrich.WithProperty("Application", environment.ApplicationName);

        // Console logging
        if (environment.IsDevelopment())
        {
            loggerConfig.WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}");
        }
        else
        {
            // Use compact JSON format for production
            loggerConfig.WriteTo.Console(new CompactJsonFormatter());
        }

        // File logging for all environments
        loggerConfig.WriteTo.File(
            new CompactJsonFormatter(),
            path: "logs/toss-erp-.json",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 30,
            fileSizeLimitBytes: 100_000_000,
            rollOnFileSizeLimit: true);

        // Add structured logging to external systems if configured
        var seqUrl = configuration["Logging:Seq:Url"];
        if (!string.IsNullOrEmpty(seqUrl))
        {
            loggerConfig.WriteTo.Seq(seqUrl);
        }

        var elasticUrl = configuration["Logging:Elasticsearch:Url"];
        if (!string.IsNullOrEmpty(elasticUrl))
        {
            loggerConfig.WriteTo.Elasticsearch(elasticUrl);
        }

        return loggerConfig.CreateLogger();
    }

    private static LogEventLevel GetLogLevel(Microsoft.AspNetCore.Http.HttpContext ctx, double _, Exception? ex) =>
        ex != null
            ? LogEventLevel.Error
            : ctx.Response.StatusCode > 499
                ? LogEventLevel.Error
                : ctx.Response.StatusCode > 399
                    ? LogEventLevel.Warning
                    : LogEventLevel.Information;

    private static void EnrichFromRequest(Serilog.Context.IDiagnosticContext diagnosticContext, Microsoft.AspNetCore.Http.HttpContext httpContext)
    {
        var request = httpContext.Request;

        // Add correlation ID
        if (httpContext.Items.TryGetValue("CorrelationId", out var correlationId))
        {
            diagnosticContext.Set("CorrelationId", correlationId);
        }

        // Add tenant ID
        if (httpContext.Items.TryGetValue("TenantId", out var tenantId))
        {
            diagnosticContext.Set("TenantId", tenantId);
        }

        // Add user information
        if (httpContext.User.Identity?.IsAuthenticated == true)
        {
            diagnosticContext.Set("UserId", httpContext.User.FindFirst("sub")?.Value ?? httpContext.User.FindFirst("id")?.Value);
            diagnosticContext.Set("UserEmail", httpContext.User.FindFirst("email")?.Value);
        }

        // Add request information
        diagnosticContext.Set("RequestHost", request.Host.Value);
        diagnosticContext.Set("RequestScheme", request.Scheme);
        diagnosticContext.Set("UserAgent", request.Headers["User-Agent"].FirstOrDefault());
        diagnosticContext.Set("ClientIP", GetClientIP(httpContext));

        // Add response information
        diagnosticContext.Set("ResponseContentType", httpContext.Response.ContentType);
        diagnosticContext.Set("ResponseContentLength", httpContext.Response.ContentLength);
    }

    private static string? GetClientIP(Microsoft.AspNetCore.Http.HttpContext context)
    {
        // Check for forwarded IP first (from load balancer/proxy)
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        var realIP = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIP))
        {
            return realIP;
        }

        return context.Connection.RemoteIpAddress?.ToString();
    }
}
