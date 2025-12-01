using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Analytics;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Application.Common.Interfaces.Authentication;
using Toss.Domain.Entities.Analytics;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Infrastructure.Services.Analytics;

public class BusinessEventService : IBusinessEventService
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;
    private readonly IUser _user;

    public BusinessEventService(
        IApplicationDbContext context,
        IBusinessContext businessContext,
        IUser user)
    {
        _context = context;
        _businessContext = businessContext;
        _user = user;
    }

    public async Task EmitEventAsync(
        BusinessEventType eventType,
        string? module = null,
        string? eventData = null,
        string? userId = null,
        CancellationToken cancellationToken = default)
    {
        // Only emit events if we have a business context
        if (!_businessContext.HasBusiness)
        {
            return; // Silently skip if no business context
        }

        var businessEvent = new BusinessEvent
        {
            BusinessId = _businessContext.CurrentBusinessId!.Value,
            EventType = eventType,
            OccurredAt = DateTimeOffset.UtcNow,
            UserId = userId ?? _user.Id,
            EventData = eventData ?? string.Empty,
            Module = module
        };

        _context.BusinessEvents.Add(businessEvent);
        
        // Save changes asynchronously without blocking
        // Use fire-and-forget pattern for analytics events
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            // Silently fail analytics events - don't break the main flow
            // In production, you might want to log this
        }
    }
}

