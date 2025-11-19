using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Onboarding;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Toss.Application.Onboarding.Commands.CompleteOnboarding;

public record CompleteOnboardingCommand : IRequest<bool>
{
    public string UserId { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
}

public class CompleteOnboardingCommandHandler : IRequestHandler<CompleteOnboardingCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public CompleteOnboardingCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(CompleteOnboardingCommand request, CancellationToken cancellationToken)                                                      
    {
        var query = _context.OnboardingStatuses.Where(s => s.UserId == request.UserId);
        
        // If role is provided, filter by it; otherwise get the first one for this user
        if (!string.IsNullOrEmpty(request.Role))
        {
            query = query.Where(s => s.Role == request.Role);
        }

        var status = await query.FirstOrDefaultAsync(cancellationToken);

        if (status == null)
        {
            // Create new onboarding status and mark as completed
            status = new OnboardingStatus
            {
                UserId = request.UserId,
                Role = request.Role,
                IsCompleted = true,
                CompletedSteps = new List<string>(),
                CurrentStep = 0
            };
            _context.OnboardingStatuses.Add(status);
        }
        else
        {
            status.IsCompleted = true;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

