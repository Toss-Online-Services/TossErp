using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Toss.Application.Audit.Commands.LogAuditEvent;

namespace Toss.Web.Endpoints;

public class Audit : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.MapPost("log", LogAuditEvent)
            .WithName("LogAuditEvent")
            .AllowAnonymous(); // Allow anonymous for logging failed auth attempts
    }

    /// <summary>
    /// Logs an audit event with automatic extraction of user context from HttpContext.
    /// Based on Identity.API patterns for security event logging.
    /// </summary>
    public async Task<IResult> LogAuditEvent(
        ISender sender,
        LogAuditEventCommand command,
        HttpContext httpContext)
    {
        try
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(command.Action))
            {
                return Results.BadRequest(new
                {
                    error = "ValidationError",
                    message = "Action is required",
                    field = "action"
                });
            }

            if (string.IsNullOrWhiteSpace(command.Severity))
            {
                return Results.BadRequest(new
                {
                    error = "ValidationError",
                    message = "Severity is required",
                    field = "severity"
                });
            }

            // Validate severity value
            var validSeverities = new[] { "info", "warning", "error", "critical" };
            if (!validSeverities.Contains(command.Severity.ToLowerInvariant()))
            {
                return Results.BadRequest(new
                {
                    error = "ValidationError",
                    message = $"Severity must be one of: {string.Join(", ", validSeverities)}",
                    field = "severity"
                });
            }

            // Automatically extract user context from HttpContext (similar to Identity.API event handling)
            var enhancedCommand = command with
            {
                // Extract IP address (handles proxies/load balancers)
                IpAddress = command.IpAddress ?? GetClientIpAddress(httpContext),
                
                // Extract User-Agent if not provided
                UserAgent = command.UserAgent ?? httpContext.Request.Headers["User-Agent"].ToString(),
                
                // Extract user information from claims if authenticated
                UserId = command.UserId ?? (httpContext.User.Identity?.IsAuthenticated == true
                    ? httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
                    : null),
                
                UserEmail = command.UserEmail ?? (httpContext.User.Identity?.IsAuthenticated == true
                    ? httpContext.User.FindFirstValue(ClaimTypes.Email)
                    : null)
            };

            // Send command to handler
            var auditId = await sender.Send(enhancedCommand);

            // Return success response with audit ID
            return Results.Ok(new
            {
                id = auditId,
                message = "Audit event logged successfully",
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception)
        {
            // Log the error but don't expose internal details to client
            // In production, this would be logged to application logger
            return Results.Problem(
                title: "Failed to log audit event",
                detail: "An error occurred while processing the audit event",
                statusCode: StatusCodes.Status500InternalServerError
            );
        }
    }

    /// <summary>
    /// Extracts the client IP address from HttpContext, handling proxies and load balancers.
    /// Based on Identity.API security patterns.
    /// </summary>
    private static string? GetClientIpAddress(HttpContext httpContext)
    {
        // Check for forwarded IP (from proxies/load balancers)
        var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            // X-Forwarded-For can contain multiple IPs, take the first one
            var ips = forwardedFor.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (ips.Length > 0)
            {
                return ips[0];
            }
        }

        // Check for real IP header
        var realIp = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp))
        {
            return realIp;
        }

        // Fall back to connection remote IP
        return httpContext.Connection.RemoteIpAddress?.ToString();
    }
}


