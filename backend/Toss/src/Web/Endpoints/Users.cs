using Toss.Application.Users.Commands.UpdateUserRoles;
using Toss.Application.Users.Queries.GetUserById;
using Toss.Application.Users.Queries.GetUsers;
using Toss.Infrastructure.Identity;

namespace Toss.Web.Endpoints;

public class Users : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        // Identity API endpoints (register, login, etc.)
        group.MapIdentityApi<ApplicationUser>();

        // User management endpoints
        group.MapGet("list", GetUsers)
            .WithName("GetUsers")
            .RequireAuthorization();

        group.MapGet("details/{id}", GetUserById)
            .WithName("GetUserById")
            .RequireAuthorization();

        group.MapPut("{id}/roles", UpdateUserRoles)
            .WithName("UpdateUserRoles")
            .RequireAuthorization();
    }

    public async Task<IResult> GetUsers(ISender sender, int? skip, int? take, string? searchTerm)
    {
        var query = new GetUsersQuery
        {
            Skip = skip ?? 0,
            Take = take ?? 50,
            SearchTerm = searchTerm
        };
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> GetUserById(ISender sender, string id)
    {
        var result = await sender.Send(new GetUserByIdQuery { Id = id });
        return Results.Ok(result);
    }

    public async Task<IResult> UpdateUserRoles(ISender sender, string id, UpdateUserRolesCommand command)
    {
        var result = await sender.Send(command with { UserId = id });
        return result ? Results.Ok(new { message = "Roles updated successfully" })
                     : Results.BadRequest(new { message = "Failed to update roles" });
    }
}
