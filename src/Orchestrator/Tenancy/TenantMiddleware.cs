using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Orchestrator.Tenancy;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;
    private readonly bool _enforce;
    public const string HeaderName = "X-Tenant-Id";

    public TenantMiddleware(RequestDelegate next, IConfiguration config)
    {
        _next = next;
        _enforce = string.Equals(config["TENANT_REQUIRED"], "true", StringComparison.OrdinalIgnoreCase);
    }

    public async Task InvokeAsync(HttpContext ctx, ITenantContext tenantCtx)
    {
        var tenantId = ctx.Request.Headers[HeaderName].FirstOrDefault();
        tenantCtx.Set(tenantId);
        if (_enforce && string.IsNullOrWhiteSpace(tenantId))
        {
            ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
            await ctx.Response.WriteAsJsonAsync(new { error = "missing_tenant", message = $"Header '{HeaderName}' required" });
            return;
        }
        await _next(ctx);
    }
}

public static class TenantMiddlewareExtensions
{
    public static IApplicationBuilder UseTenantContext(this IApplicationBuilder app) => app.UseMiddleware<TenantMiddleware>();
    public static IServiceCollection AddTenantContext(this IServiceCollection services)
    {
        services.AddScoped<ITenantContext, TenantContext>();
        return services;
    }
}
