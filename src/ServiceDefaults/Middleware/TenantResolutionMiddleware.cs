using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace eShop.ServiceDefaults.Middleware;

/// <summary>
/// Middleware to resolve tenant context from various sources
/// </summary>
public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantResolutionMiddleware> _logger;

    public TenantResolutionMiddleware(RequestDelegate next, ILogger<TenantResolutionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var tenantId = ResolveTenantId(context);
        
        if (!string.IsNullOrEmpty(tenantId))
        {
            context.Items["TenantId"] = tenantId;
            context.Items["HasTenant"] = true;
            
            // Add tenant header for downstream services
            context.Response.Headers["X-Tenant-Id"] = tenantId;
            
            _logger.LogDebug("Resolved tenant: {TenantId}", tenantId);
        }
        else
        {
            context.Items["HasTenant"] = false;
            _logger.LogDebug("No tenant resolved for request");
        }

        await _next(context);
    }

    private string? ResolveTenantId(HttpContext context)
    {
        // Priority order for tenant resolution:
        // 1. JWT claim (for authenticated requests)
        // 2. X-Tenant-Id header
        // 3. Query parameter
        // 4. Subdomain (future enhancement)

        // 1. Check JWT token for tenant claim
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var tenantClaim = context.User.FindFirst("tenant_id")?.Value;
            if (!string.IsNullOrEmpty(tenantClaim))
            {
                return tenantClaim;
            }
        }

        // 2. Check X-Tenant-Id header
        if (context.Request.Headers.TryGetValue("X-Tenant-Id", out var tenantHeader))
        {
            var headerValue = tenantHeader.FirstOrDefault();
            if (!string.IsNullOrEmpty(headerValue))
            {
                return headerValue;
            }
        }

        // 3. Check query parameter
        if (context.Request.Query.TryGetValue("tenantId", out var tenantQuery))
        {
            var queryValue = tenantQuery.FirstOrDefault();
            if (!string.IsNullOrEmpty(queryValue))
            {
                return queryValue;
            }
        }

        // 4. Future: Subdomain resolution
        // var host = context.Request.Host.Host;
        // if (host.Contains('.'))
        // {
        //     var subdomain = host.Split('.')[0];
        //     if (!IsReservedSubdomain(subdomain))
        //     {
        //         return subdomain;
        //     }
        // }

        // Default tenant for development
        if (IsDevelopmentEnvironment(context))
        {
            return "dev-tenant";
        }

        return null;
    }

    private static bool IsDevelopmentEnvironment(HttpContext context)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        return string.Equals(environment, "Development", StringComparison.OrdinalIgnoreCase);
    }
}

/// <summary>
/// Extensions for registering tenant resolution middleware
/// </summary>
public static class TenantResolutionExtensions
{
    /// <summary>
    /// Add tenant resolution middleware to the pipeline
    /// </summary>
    public static IApplicationBuilder UseTenantResolution(this IApplicationBuilder app)
    {
        return app.UseMiddleware<TenantResolutionMiddleware>();
    }

    /// <summary>
    /// Get the current tenant ID from HttpContext
    /// </summary>
    public static string? GetTenantId(this HttpContext context)
    {
        return context.Items.TryGetValue("TenantId", out var tenantId) 
            ? tenantId?.ToString() 
            : null;
    }

    /// <summary>
    /// Check if the current request has a tenant context
    /// </summary>
    public static bool HasTenant(this HttpContext context)
    {
        return context.Items.TryGetValue("HasTenant", out var hasTenant) 
            && hasTenant is bool hasValue 
            && hasValue;
    }

    /// <summary>
    /// Require tenant context for the current request
    /// </summary>
    public static void RequireTenant(this HttpContext context)
    {
        if (!context.HasTenant())
        {
            throw new UnauthorizedAccessException("Tenant context is required for this operation");
        }
    }
}
