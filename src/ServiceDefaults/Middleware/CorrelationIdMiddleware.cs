using Microsoft.AspNetCore.Http;

namespace eShop.ServiceDefaults.Middleware;

/// <summary>
/// Middleware to handle correlation IDs for request tracing
/// </summary>
public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _logger;
    private const string CorrelationIdHeaderName = "X-Correlation-ID";

    public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = GetOrCreateCorrelationId(context);
        
        // Store in context for use throughout the request
        context.Items["CorrelationId"] = correlationId;
        
        // Add to response headers
        context.Response.Headers[CorrelationIdHeaderName] = correlationId;
        
        // Add to logging context
        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId
        }))
        {
            _logger.LogDebug("Processing request with correlation ID: {CorrelationId}", correlationId);
            await _next(context);
        }
    }

    private string GetOrCreateCorrelationId(HttpContext context)
    {
        // Check if correlation ID is provided in request headers
        if (context.Request.Headers.TryGetValue(CorrelationIdHeaderName, out var correlationId))
        {
            var headerValue = correlationId.FirstOrDefault();
            if (!string.IsNullOrEmpty(headerValue))
            {
                return headerValue;
            }
        }

        // Generate new correlation ID
        return Guid.NewGuid().ToString();
    }
}

/// <summary>
/// Extensions for correlation ID middleware
/// </summary>
public static class CorrelationIdExtensions
{
    /// <summary>
    /// Add correlation ID middleware to the pipeline
    /// </summary>
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
    {
        return app.UseMiddleware<CorrelationIdMiddleware>();
    }

    /// <summary>
    /// Get the current correlation ID from HttpContext
    /// </summary>
    public static string? GetCorrelationId(this HttpContext context)
    {
        return context.Items.TryGetValue("CorrelationId", out var correlationId) 
            ? correlationId?.ToString() 
            : null;
    }
}
