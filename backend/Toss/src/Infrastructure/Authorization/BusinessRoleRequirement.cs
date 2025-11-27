using Microsoft.AspNetCore.Authorization;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Constants;

namespace Toss.Infrastructure.Authorization;

public sealed class BusinessRoleRequirement : IAuthorizationRequirement
{
    public BusinessRoleRequirement(IEnumerable<string> roles)
    {
        AllowedRoles = roles?.ToArray() ?? Array.Empty<string>();
    }

    public IReadOnlyCollection<string> AllowedRoles { get; }
}

public sealed class BusinessRoleAuthorizationHandler : AuthorizationHandler<BusinessRoleRequirement>
{
    private readonly IBusinessContext _businessContext;

    public BusinessRoleAuthorizationHandler(IBusinessContext businessContext)
    {
        _businessContext = businessContext;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        BusinessRoleRequirement requirement)
    {
        if (context.User.IsInRole(Roles.Administrator))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        if (_businessContext.HasBusiness &&
            !string.IsNullOrWhiteSpace(_businessContext.CurrentBusinessRole) &&
            requirement.AllowedRoles.Contains(_businessContext.CurrentBusinessRole))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

