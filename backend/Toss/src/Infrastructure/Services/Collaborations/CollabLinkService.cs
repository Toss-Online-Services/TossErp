using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Collaborations;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Infrastructure.Services.Collaborations;

public class CollabLinkService : ICollabLinkService
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public CollabLinkService(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<bool> ValidateLinkAsync(
        string linkCode,
        CollabLinkPurpose purpose,
        string linkedType,
        int linkedId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(linkCode))
        {
            return false;
        }

        var link = await _context.CollabLinks
            .FirstOrDefaultAsync(l => l.LinkCode == linkCode, cancellationToken);

        if (link == null)
        {
            return false;
        }

        // Check if link is active
        if (!link.IsActive)
        {
            return false;
        }

        // Check if link has expired
        if (link.ExpiresAt.HasValue && link.ExpiresAt.Value < DateTimeOffset.UtcNow)
        {
            return false;
        }

        // Check if purpose matches
        if (link.Purpose != purpose)
        {
            return false;
        }

        // Check if linked entity matches
        if (link.LinkedType != linkedType || link.LinkedId != linkedId)
        {
            return false;
        }

        return true;
    }

    public async Task<CollabLinkInfo?> GetLinkInfoAsync(string linkCode, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(linkCode))
        {
            return null;
        }

        var link = await _context.CollabLinks
            .FirstOrDefaultAsync(l => l.LinkCode == linkCode, cancellationToken);

        if (link == null)
        {
            return null;
        }

        return new CollabLinkInfo
        {
            BusinessId = link.BusinessId,
            LinkedType = link.LinkedType,
            LinkedId = link.LinkedId,
            Purpose = link.Purpose,
            IsActive = link.IsActive,
            ExpiresAt = link.ExpiresAt
        };
    }
}

