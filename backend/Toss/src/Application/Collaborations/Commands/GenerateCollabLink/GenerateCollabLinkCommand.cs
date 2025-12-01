using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.Collaborations;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Toss.Application.Collaborations.Commands.GenerateCollabLink;

public record GenerateCollabLinkCommand : IRequest<string>
{
    public string LinkedType { get; init; } = string.Empty;
    public int LinkedId { get; init; }
    public CollabLinkPurpose Purpose { get; init; }
    public DateTimeOffset? ExpiresAt { get; init; }
}

public class GenerateCollabLinkCommandHandler : IRequestHandler<GenerateCollabLinkCommand, string>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public GenerateCollabLinkCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<string> Handle(GenerateCollabLinkCommand request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        if (string.IsNullOrWhiteSpace(request.LinkedType))
        {
            throw new ValidationException("Linked type is required.");
        }

        // Generate a unique link code
        string linkCode;
        bool isUnique = false;
        int attempts = 0;
        const int maxAttempts = 10;

        do
        {
            linkCode = GenerateSecureLinkCode();
            var exists = await _context.CollabLinks
                .AnyAsync(l => l.LinkCode == linkCode, cancellationToken);
            
            if (!exists)
            {
                isUnique = true;
            }
            attempts++;
        } while (!isUnique && attempts < maxAttempts);

        if (!isUnique)
        {
            throw new ValidationException("Failed to generate unique link code after multiple attempts.");
        }

        var link = new CollabLink
        {
            BusinessId = _businessContext.CurrentBusinessId!.Value,
            LinkCode = linkCode,
            LinkedType = request.LinkedType,
            LinkedId = request.LinkedId,
            Purpose = request.Purpose,
            IsActive = true,
            ExpiresAt = request.ExpiresAt
        };

        _context.CollabLinks.Add(link);
        await _context.SaveChangesAsync(cancellationToken);

        return linkCode;
    }

    private static string GenerateSecureLinkCode()
    {
        // Generate a secure random token (32 bytes = 64 hex characters)
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[32];
        rng.GetBytes(bytes);
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}

