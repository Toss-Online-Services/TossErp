using Toss.Application.Common.Interfaces;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace Toss.Application.Common.Behaviours;

/// <summary>
/// MediatR pre-processor that logs all incoming requests with user context.
/// Executes BEFORE all other pipeline behaviors and the request handler.
/// </summary>
/// <typeparam name="TRequest">The request type being logged.</typeparam>
/// <remarks>
/// Logs the following information for each request:
/// <list type="bullet">
/// <item><description>Request type name</description></item>
/// <item><description>User ID (if authenticated)</description></item>
/// <item><description>Username (if authenticated)</description></item>
/// <item><description>Full request object (serialized)</description></item>
/// </list>
/// This provides comprehensive audit trail and debugging capabilities.
/// </remarks>
public class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest>
    where TRequest : notnull
{
    private readonly ILogger _logger;
    private readonly IUser _user;
    private readonly IIdentityService _identityService;

    public LoggingBehaviour(ILogger<TRequest> logger, IUser user, IIdentityService identityService)
    {
        _logger = logger;
        _user = user;
        _identityService = identityService;
    }

    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = _user.Id ?? string.Empty;
        string? userName = string.Empty;

        if (!string.IsNullOrEmpty(userId))
        {
            userName = await _identityService.GetUserNameAsync(userId);
        }

        _logger.LogInformation("Toss Request: {Name} {@UserId} {@UserName} {@Request}",
            requestName, userId, userName, request);
    }
}
