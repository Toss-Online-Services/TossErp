using Microsoft.Extensions.Logging;

namespace Toss.Application.Common.Behaviours;

/// <summary>
/// MediatR pipeline behavior that catches and logs all unhandled exceptions.
/// Provides a safety net for unexpected errors in the request pipeline.
/// </summary>
/// <typeparam name="TRequest">The request type being protected.</typeparam>
/// <typeparam name="TResponse">The response type returned by the handler.</typeparam>
/// <remarks>
/// This behavior wraps the entire pipeline execution in try-catch.
/// When an exception occurs:
/// <list type="number">
/// <item><description>Exception is logged with full context (request name and details)</description></item>
/// <item><description>Exception is re-thrown to maintain proper error flow</description></item>
/// </list>
/// Executes early in the pipeline to catch errors from all subsequent behaviors and handlers.
/// </remarks>
public class UnhandledExceptionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<TRequest> _logger;

    public UnhandledExceptionBehaviour(ILogger<TRequest> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            var requestName = typeof(TRequest).Name;

            _logger.LogError(ex, "Toss Request: Unhandled Exception for Request {Name} {@Request}", requestName, request);

            throw;
        }
    }
}
