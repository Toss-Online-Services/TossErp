using System.Reflection;
using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Security;

namespace Toss.Application.Common.Behaviours;

/// <summary>
/// MediatR pipeline behavior that enforces role-based and policy-based authorization.
/// Prevents unauthorized users from executing protected commands and queries.
/// </summary>
/// <typeparam name="TRequest">The request type being authorized.</typeparam>
/// <typeparam name="TResponse">The response type returned by the handler.</typeparam>
/// <remarks>
/// Authorization process:
/// <list type="number">
/// <item><description>Check for [Authorize] attribute on the request class</description></item>
/// <item><description>If attribute present, verify user is authenticated</description></item>
/// <item><description>Validate user has required roles (if Roles specified)</description></item>
/// <item><description>Validate user meets required policies (if Policy specified)</description></item>
/// </list>
/// Throws <see cref="UnauthorizedAccessException"/> if user not authenticated.
/// Throws <see cref="ForbiddenAccessException"/> if user lacks required roles or policies.
/// </remarks>
public class AuthorizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
    where TRequest : notnull
{
    private readonly IUser _user;
    private readonly IIdentityService _identityService;

    public AuthorizationBehaviour(
        IUser user,
        IIdentityService identityService)
    {
        _user = user;
        _identityService = identityService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var authorizeAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>();

        if (authorizeAttributes.Any())
        {
            // Must be authenticated user
            if (_user.Id == null)
            {
                throw new UnauthorizedAccessException();
            }

            // Role-based authorization
            var authorizeAttributesWithRoles = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Roles));

            if (authorizeAttributesWithRoles.Any())
            {
                var authorized = false;

                foreach (var roles in authorizeAttributesWithRoles.Select(a => a.Roles.Split(',')))
                {
                    foreach (var role in roles)
                    {
                        var isInRole = _user.Roles?.Any(x => role == x)??false;
                        if (isInRole)
                        {
                            authorized = true;
                            break;
                        }
                    }
                }

                // Must be a member of at least one role in roles
                if (!authorized)
                {
                    throw new ForbiddenAccessException();
                }
            }

            // Policy-based authorization
            var authorizeAttributesWithPolicies = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Policy));
            if (authorizeAttributesWithPolicies.Any())
            {
                foreach (var policy in authorizeAttributesWithPolicies.Select(a => a.Policy))
                {
                    var authorized = await _identityService.AuthorizeAsync(_user.Id, policy);

                    if (!authorized)
                    {
                        throw new ForbiddenAccessException();
                    }
                }
            }
        }

        // User is authorized / authorization not required
        return await next();
    }
}
