using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Constants;
using Toss.Infrastructure.Data;

namespace Toss.Infrastructure.Services.Tenancy;

public class BusinessContextMiddleware
{
    private const string BusinessIdHeader = "X-Business-Id";
    private const string BusinessCodeHeader = "X-Business-Code";
    private readonly RequestDelegate _next;
    private readonly ILogger<BusinessContextMiddleware> _logger;

    public BusinessContextMiddleware(RequestDelegate next, ILogger<BusinessContextMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(
        HttpContext context,
        IUser currentUser,
        ApplicationDbContext dbContext,
        IBusinessContext businessContext)
    {
        if (context.User?.Identity?.IsAuthenticated != true || string.IsNullOrWhiteSpace(currentUser.Id))
        {
            await _next(context);
            return;
        }

        if (businessContext.HasBusiness)
        {
            await _next(context);
            return;
        }

        var requestedCode = context.Request.Headers[BusinessCodeHeader].FirstOrDefault();
        var requestedIdHeader = context.Request.Headers[BusinessIdHeader].FirstOrDefault();
        int? requestedId = null;

        if (!string.IsNullOrWhiteSpace(requestedIdHeader) && int.TryParse(requestedIdHeader, out var parsedId))
        {
            requestedId = parsedId;
        }

        var query = dbContext.UserBusinesses
            .Include(ub => ub.Business)
            .AsNoTracking()
            .Where(ub => ub.UserId == currentUser.Id);

        if (!string.IsNullOrWhiteSpace(requestedCode))
        {
            query = query.Where(ub => ub.Business.Code == requestedCode);
        }
        else if (requestedId.HasValue)
        {
            query = query.Where(ub => ub.BusinessId == requestedId.Value);
        }

        var membership = await query
            .OrderByDescending(ub => ub.IsDefault)
            .ThenBy(ub => ub.BusinessId)
            .FirstOrDefaultAsync();

        if (membership is null)
        {
            if (!string.IsNullOrWhiteSpace(requestedCode) || requestedId.HasValue)
            {
                _logger.LogWarning("User {UserId} attempted to access unauthorized business {BusinessIdentifier}.", currentUser.Id, requestedCode ?? requestedId?.ToString());
                await WriteForbiddenAsync(context, "You do not have access to the requested business.");
                return;
            }

            if (context.User.IsInRole(Roles.Administrator))
            {
                await _next(context);
                return;
            }

            _logger.LogWarning("User {UserId} is not associated with any business.", currentUser.Id);
            await WriteForbiddenAsync(context, "No business membership found for your account.");
            return;
        }

        businessContext.SetBusiness(
            membership.BusinessId,
            membership.Business.Code,
            membership.Business.Name,
            membership.Role);

        context.Response.Headers[BusinessIdHeader] = membership.BusinessId.ToString();
        context.Response.Headers[BusinessCodeHeader] = membership.Business.Code;

        await _next(context);
    }

    private static async Task WriteForbiddenAsync(HttpContext context, string message)
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        await context.Response.WriteAsJsonAsync(new { message });
    }
}

