using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Onboarding;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Toss.Application.Onboarding.Queries.GetOnboardingStatus;

public record GetOnboardingStatusQuery : IRequest<OnboardingStatusDto?>
{
    public string UserId { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
}

public record OnboardingStatusDto
{
    public int Id { get; init; }
    public string UserId { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public bool IsCompleted { get; init; }
    public List<string> CompletedSteps { get; init; } = new();
    public int CurrentStep { get; init; }
    public string? OnboardingData { get; init; }
}

public class GetOnboardingStatusQueryHandler : IRequestHandler<GetOnboardingStatusQuery, OnboardingStatusDto?>
{
    private readonly IApplicationDbContext _context;

    public GetOnboardingStatusQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<OnboardingStatusDto?> Handle(GetOnboardingStatusQuery request, CancellationToken cancellationToken)                                       
    {
        var query = _context.OnboardingStatuses.Where(s => s.UserId == request.UserId);
        
        // If role is provided, filter by it; otherwise get the first one for this user
        if (!string.IsNullOrEmpty(request.Role))
        {
            query = query.Where(s => s.Role == request.Role);
        }

        var status = await query.FirstOrDefaultAsync(cancellationToken);

        if (status == null)
            return null;

        return new OnboardingStatusDto
        {
            Id = status.Id,
            UserId = status.UserId,
            Role = status.Role,
            IsCompleted = status.IsCompleted,
            CompletedSteps = status.CompletedSteps,
            CurrentStep = status.CurrentStep,
            OnboardingData = status.OnboardingData
        };
    }
}

