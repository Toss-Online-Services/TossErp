using System.Diagnostics;
using Toss.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace Toss.Application.Common.Behaviours;

/// <summary>
/// MediatR pipeline behavior that monitors request execution time and logs warnings for slow requests.
/// Helps identify performance bottlenecks in the application.
/// </summary>
/// <typeparam name="TRequest">The request type being monitored.</typeparam>
/// <typeparam name="TResponse">The response type returned by the handler.</typeparam>
/// <remarks>
/// Performance monitoring thresholds:
/// <list type="bullet">
/// <item><description>Threshold: 500 milliseconds</description></item>
/// <item><description>Action: Log warning with user context and elapsed time</description></item>
/// </list>
/// Logs include request name, execution time, user ID, username, and full request object
/// for debugging slow operations.
/// </remarks>
public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly Stopwatch _timer;
    private readonly ILogger<TRequest> _logger;
    private readonly IUser _user;
    private readonly IIdentityService _identityService;

    public PerformanceBehaviour(
        ILogger<TRequest> logger,
        IUser user,
        IIdentityService identityService)
    {
        _timer = new Stopwatch();

        _logger = logger;
        _user = user;
        _identityService = identityService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _timer.Start();

        var response = await next();

        _timer.Stop();

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        if (elapsedMilliseconds > 500)
        {
            var requestName = typeof(TRequest).Name;
            var userId = _user.Id ?? string.Empty;
            var userName = string.Empty;

            if (!string.IsNullOrEmpty(userId))
            {
                userName = await _identityService.GetUserNameAsync(userId);
            }

            _logger.LogWarning("Toss Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@UserName} {@Request}",
                requestName, elapsedMilliseconds, userId, userName, request);
        }

        return response;
    }
}
