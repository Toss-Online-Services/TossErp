using Microsoft.AspNetCore.Http;
using Polly.CircuitBreaker;
using Polly;
using Microsoft.Extensions.Logging;

namespace AgentManager;

/// <summary>
/// Maps resilience framework exceptions to ProblemDetails JSON responses.
/// </summary>
public class ResilienceExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ResilienceExceptionMiddleware> _logger;

    public ResilienceExceptionMiddleware(RequestDelegate next, ILogger<ResilienceExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BrokenCircuitException ex)
        {
            _logger.LogError(ex, "Circuit open blocking outbound call");
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Title = "Service temporarily unavailable",
                Type = "https://problems.tosserp.ai/llm/circuit-open",
                Detail = "The AI provider circuit is open due to recent failures. Please retry shortly.",
                Status = StatusCodes.Status503ServiceUnavailable
            });
        }
        catch (TimeoutRejectedException ex)
        {
            _logger.LogWarning(ex, "Outbound AI request timed out");
            context.Response.StatusCode = StatusCodes.Status504GatewayTimeout;
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Title = "AI upstream timeout",
                Type = "https://problems.tosserp.ai/llm/timeout",
                Detail = "The AI provider did not respond in time.",
                Status = StatusCodes.Status504GatewayTimeout
            });
        }
    }
}
