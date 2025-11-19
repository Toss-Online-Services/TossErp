using Toss.Application.Onboarding.Commands.CompleteOnboarding;
using Toss.Application.Onboarding.Commands.UpdateOnboardingStatus;
using Toss.Application.Onboarding.Queries.GetOnboardingStatus;
using System.Security.Claims;

namespace Toss.Web.Endpoints;

public class Onboarding : EndpointGroupBase
{
    public override string? GroupName => "onboarding";

    public override void Map(RouteGroupBuilder group)
    {
        group.MapGet("{userId}", GetOnboardingStatus)
            .WithName("GetOnboardingStatus")
            .RequireAuthorization();

        group.MapPut("{userId}", UpdateOnboardingStatus)
            .WithName("UpdateOnboardingStatus")
            .RequireAuthorization();

        group.MapPost("{userId}/complete", CompleteOnboarding)
            .WithName("CompleteOnboarding")
            .RequireAuthorization();
    }

    public async Task<IResult> GetOnboardingStatus(
        ISender sender,
        string userId,
        string? role)
    {
        var query = new GetOnboardingStatusQuery
        {
            UserId = userId,
            Role = role ?? string.Empty
        };
        var result = await sender.Send(query);
        return result != null ? Results.Ok(result) : Results.NotFound();        
    }

    public async Task<IResult> UpdateOnboardingStatus(
        ISender sender,
        string userId,
        UpdateOnboardingStatusCommand command)
    {
        var result = await sender.Send(command with { UserId = userId, Role = command.Role });
        return result ? Results.Ok(new { message = "Onboarding status updated successfully" })
                     : Results.BadRequest(new { message = "Failed to update onboarding status" });
    }

    public async Task<IResult> CompleteOnboarding(
        ISender sender,
        HttpContext httpContext,
        string userId,
        string? role)
    {
        // If role not provided, try to get it from the user's claims
        if (string.IsNullOrEmpty(role))
        {
            var user = httpContext.User;
            var roleClaim = user.FindFirst(ClaimTypes.Role);
            role = roleClaim?.Value ?? string.Empty;
        }

        var result = await sender.Send(new CompleteOnboardingCommand 
        { 
            UserId = userId,
            Role = role
        });
        return result ? Results.Ok(new { message = "Onboarding completed successfully" })                                                                       
                     : Results.BadRequest(new { message = "Failed to complete onboarding" });                                                                   
    }
}

