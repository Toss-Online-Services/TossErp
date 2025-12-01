using Toss.Application.Common.Interfaces.Analytics;
using Toss.Domain.Enums;

namespace Toss.Web.Infrastructure;

/// <summary>
/// Middleware to track module usage for analytics
/// </summary>
public class AnalyticsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AnalyticsMiddleware> _logger;

    public AnalyticsMiddleware(RequestDelegate next, ILogger<AnalyticsMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IBusinessEventService? eventService)
    {
        // Only track authenticated requests with business context
        if (context.User?.Identity?.IsAuthenticated == true && eventService != null)
        {
            var path = context.Request.Path.Value ?? string.Empty;
            var module = DetermineModule(path);

            if (!string.IsNullOrEmpty(module))
            {
                // Fire and forget - don't block the request
                _ = Task.Run(async () =>
                {
                    try
                    {
                        var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                        await eventService.EmitEventAsync(
                            BusinessEventType.ModuleUsage,
                            module: module,
                            userId: userId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to track module usage for {Module}", module);
                    }
                });
            }
        }

        await _next(context);
    }

    private static string? DetermineModule(string path)
    {
        if (path.StartsWith("/api/", StringComparison.OrdinalIgnoreCase))
        {
            var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length >= 2)
            {
                var module = segments[1].ToLowerInvariant();
                // Map API routes to module names
                return module switch
                {
                    "pos" or "sales" => "POS",
                    "stock" or "inventory" => "Stock",
                    "crm" or "customers" => "CRM",
                    "procurement" or "purchases" => "Procurement",
                    "accounting" or "money" => "Accounting",
                    "hr" or "payroll" => "HR",
                    "support" or "tickets" => "Support",
                    "projects" or "jobs" => "Projects",
                    "tasks" => "Tasks",
                    "quality" => "Quality",
                    "assets" => "Assets",
                    "manufacturing" => "Manufacturing",
                    _ => null
                };
            }
        }
        return null;
    }
}

