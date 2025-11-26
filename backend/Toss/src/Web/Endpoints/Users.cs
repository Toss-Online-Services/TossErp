using Toss.Application.Users.Commands.ActivateUser;
using Toss.Application.Users.Commands.DeactivateUser;
using Toss.Application.Users.Commands.UpdateUserRoles;
using Toss.Application.Users.Queries.GetUserById;
using Toss.Application.Users.Queries.GetUsers;
using Toss.Domain.Constants;
using Toss.Infrastructure.Identity;

namespace Toss.Web.Endpoints;

public class Users : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        // Identity API endpoints (register, login, etc.)
        group.MapIdentityApi<ApplicationUser>();

        // User management endpoints (Admin only)
        group.MapGet("list", GetUsers)
            .WithName("GetUsers")
            .RequireAuthorization("RequireAdmin");

        group.MapGet("details/{id}", GetUserById)
            .WithName("GetUserById")
            .RequireAuthorization("RequireAdmin");

        group.MapPut("{id}/roles", UpdateUserRoles)
            .WithName("UpdateUserRoles")
            .RequireAuthorization("RequireAdmin");

        group.MapPost("{id}/activate", ActivateUser)
            .WithName("ActivateUser")
            .RequireAuthorization("RequireAdmin");

        group.MapPost("{id}/deactivate", DeactivateUser)
            .WithName("DeactivateUser")
            .RequireAuthorization("RequireAdmin");
    }

    public async Task<IResult> GetUsers(ISender sender, int? skip, int? take, string? searchTerm, string? role)                                                               
    {
        var query = new GetUsersQuery
        {
            Skip = skip ?? 0,
            Take = take ?? 50,
            SearchTerm = searchTerm,
            Role = role
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

    public async Task<IResult> ActivateUser(ISender sender, string id)
    {
        var result = await sender.Send(new ActivateUserCommand { UserId = id });
        return result ? Results.Ok(new { message = "User activated successfully" })
                     : Results.BadRequest(new { message = "Failed to activate user" });
    }

    public async Task<IResult> DeactivateUser(ISender sender, string id)
    {
        var result = await sender.Send(new DeactivateUserCommand { UserId = id });
        return result ? Results.Ok(new { message = "User deactivated successfully" })
                     : Results.BadRequest(new { message = "Failed to deactivate user" });
    }
}
