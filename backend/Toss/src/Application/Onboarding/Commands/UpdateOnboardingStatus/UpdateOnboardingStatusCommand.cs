using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Onboarding;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Onboarding.Commands.UpdateOnboardingStatus;

public record UpdateOnboardingStatusCommand : IRequest<bool>
{
    public string UserId { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public List<string> CompletedSteps { get; init; } = new();
    public int CurrentStep { get; init; }
    public string? OnboardingData { get; init; }
}

public class UpdateOnboardingStatusCommandHandler : IRequestHandler<UpdateOnboardingStatusCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpdateOnboardingStatusCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateOnboardingStatusCommand request, CancellationToken cancellationToken)
    {
        var status = await _context.OnboardingStatuses
            .FirstOrDefaultAsync(
                s => s.UserId == request.UserId && s.Role == request.Role,
                cancellationToken);

        if (status == null)
        {
            // Create new onboarding status
            status = new OnboardingStatus
            {
                UserId = request.UserId,
                Role = request.Role,
                CompletedSteps = request.CompletedSteps,
                CurrentStep = request.CurrentStep,
                OnboardingData = request.OnboardingData,
                IsCompleted = false
            };
            _context.OnboardingStatuses.Add(status);
        }
        else
        {
            // Update existing status
            status.CompletedSteps = request.CompletedSteps;
            status.CurrentStep = request.CurrentStep;
            status.OnboardingData = request.OnboardingData;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

