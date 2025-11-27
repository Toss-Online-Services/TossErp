using Toss.Application.Businesses.Members.Commands.RemoveBusinessMember;
using Toss.Application.Businesses.Members.Commands.UpsertBusinessMember;
using Toss.Application.Businesses.Members.Queries.GetBusinessMembers;
using Toss.Application.Common.Models;
using Toss.Domain.Constants;

namespace Toss.Web.Endpoints;

public class BusinessMembers : EndpointGroupBase
{
    public override string? GroupName => "businesses";

    public override void Map(RouteGroupBuilder group)
    {
        var membersGroup = group.MapGroup("{businessId:int}/members")
            .RequireAuthorization(Policies.RequireOwnerOrManager);

        membersGroup.MapGet("", GetMembers)
            .WithName("GetBusinessMembers");

        membersGroup.MapPut("", UpsertMember)
            .WithName("UpsertBusinessMember");

        membersGroup.MapDelete("{userId}", RemoveMember)
            .WithName("RemoveBusinessMember");
    }

    public async Task<IResult> GetMembers(ISender sender, int businessId)
    {
        var members = await sender.Send(new GetBusinessMembersQuery(businessId));
        return Results.Ok(members);
    }

    public async Task<IResult> UpsertMember(
        ISender sender,
        int businessId,
        BusinessMemberUpsertRequest request)
    {
        var command = new UpsertBusinessMemberCommand
        {
            BusinessId = businessId,
            UserId = request.UserId,
            Role = request.Role,
            IsDefault = request.IsDefault
        };

        var result = await sender.Send(command);
        return FromResult(result);
    }

    public async Task<IResult> RemoveMember(
        ISender sender,
        int businessId,
        string userId)
    {
        var result = await sender.Send(new RemoveBusinessMemberCommand
        {
            BusinessId = businessId,
            UserId = userId
        });

        return FromResult(result);
    }

    private static IResult FromResult(Result result)
    {
        return result.Succeeded
            ? Results.Ok(new { message = "Success" })
            : Results.BadRequest(new { errors = result.Errors });
    }
}

public record BusinessMemberUpsertRequest(string UserId, string Role, bool IsDefault);

