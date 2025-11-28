namespace Toss.Application.Common.Behaviours;

/// <summary>
/// MediatR pipeline behavior that validates requests using FluentValidation.
/// Executes all registered validators and throws ValidationException if any fail.
/// </summary>
/// <typeparam name="TRequest">The request type to validate.</typeparam>
/// <typeparam name="TResponse">The response type returned by the handler.</typeparam>
/// <remarks>
/// This behavior runs BEFORE the request handler executes. If validation fails,
/// the handler is never called. Validation failures are collected and thrown as
/// a single ValidationException containing all error details.
/// </remarks>
public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var validationResults = await Task.WhenAll(
                _validators.Select(v =>
                    v.ValidateAsync(new ValidationContext<TRequest>(request), cancellationToken)));

            var failures = validationResults
                .Where(r => r.Errors.Any())
                .SelectMany(r => r.Errors)
                .ToList();

            if (failures.Count != 0)
                throw new ValidationException(failures);
        }

        return await next();
    }
}
