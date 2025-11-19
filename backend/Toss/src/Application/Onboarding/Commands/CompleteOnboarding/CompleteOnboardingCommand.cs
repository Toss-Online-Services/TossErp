using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Onboarding;
using Microsoft.EntityFrameworkCore;

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
        var status = await _context.OnboardingStatuses
            .FirstOrDefaultAsync(
                s => s.UserId == request.UserId && s.Role == request.Role,
                cancellationToken);

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

