using System.Collections.Concurrent;

namespace TossErp.API.Middleware;

/// <summary>
/// Rate limiting middleware to prevent API abuse
/// </summary>
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private static readonly ConcurrentDictionary<string, ClientRateLimit> _clients = new();

    public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Get client identifier (IP or API key)
        var clientId = GetClientIdentifier(context);
        
        // Get or create rate limit tracker
        var rateLimit = _clients.GetOrAdd(clientId, _ => new ClientRateLimit());
        
        // Check if rate limit exceeded
        if (rateLimit.IsLimitExceeded())
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.Response.Headers.Add("Retry-After", "60");
            
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Rate limit exceeded",
                message = "Too many requests. Please try again later.",
                retryAfter = 60
            });
            
            _logger.LogWarning("Rate limit exceeded for client {ClientId}", clientId);
            return;
        }

        // Record request
        rateLimit.RecordRequest();

        await _next(context);
    }

    private string GetClientIdentifier(HttpContext context)
    {
        // Try to get user ID from claims
        var userId = context.User?.FindFirst("sub")?.Value;
        if (!string.IsNullOrEmpty(userId))
            return $"user:{userId}";

        // Fallback to IP address
        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return $"ip:{ipAddress}";
    }
}

/// <summary>
/// Tracks rate limit for a single client
/// </summary>
public class ClientRateLimit
{
    private readonly Queue<DateTime> _requests = new();
    private readonly int _maxRequests = 100; // per minute
    private readonly TimeSpan _timeWindow = TimeSpan.FromMinutes(1);

    public void RecordRequest()
    {
        lock (_requests)
        {
            _requests.Enqueue(DateTime.UtcNow);
            CleanupOldRequests();
        }
    }

    public bool IsLimitExceeded()
    {
        lock (_requests)
        {
            CleanupOldRequests();
            return _requests.Count >= _maxRequests;
        }
    }

    private void CleanupOldRequests()
    {
        var cutoff = DateTime.UtcNow - _timeWindow;
        while (_requests.Count > 0 && _requests.Peek() < cutoff)
        {
            _requests.Dequeue();
        }
    }
}

