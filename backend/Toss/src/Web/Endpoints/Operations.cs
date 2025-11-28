using MediatR;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Application.Operations.Queries.GetTodayView;
using Toss.Domain.Constants;

namespace Toss.Web.Endpoints;

public sealed class Operations : EndpointGroupBase
{
    public override string? GroupName => "operations";

    public override void Map(RouteGroupBuilder group)
    {
        group.RequireAuthorization(Policies.RequireStaffOrAbove);
        group.MapGet("today", GetTodayView)
            .WithName("GetOperationsToday")
            .Produces<OperationsTodayDto>();
    }

    private static async Task<IResult> GetTodayView(
        ISender sender,
        IBusinessContext businessContext,
        CancellationToken cancellationToken = default)
    {
        if (!businessContext.HasBusiness || businessContext.CurrentBusinessId is null)
        {
            return Results.BadRequest(new { message = "Active business context required." });
        }

        var dto = await sender.Send(
            new GetOperationsTodayQuery(businessContext.CurrentBusinessId.Value),
            cancellationToken);

        return Results.Ok(dto);
    }
}

