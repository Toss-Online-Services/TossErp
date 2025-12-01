using Toss.Application.Notifications.Commands.CreateNotification;
using Toss.Application.Notifications.Commands.MarkNotificationRead;
using Toss.Application.Notifications.Commands.UpdateNotificationPreference;
using Toss.Application.Notifications.Queries.GetNotifications;
using Toss.Domain.Constants;

namespace Toss.Web.Endpoints;

public class Notifications : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.RequireAuthorization(Policies.RequireStaffOrAbove);

        group.MapGet(string.Empty, GetNotifications)
            .WithName("GetNotifications");

        group.MapPost(string.Empty, CreateNotification)
            .WithName("CreateNotification")
            .RequireAuthorization(Policies.RequireOwnerOrManager); // Only managers can create notifications

        group.MapPut("{id}/read", MarkNotificationRead)
            .WithName("MarkNotificationRead");

        group.MapPut("preferences", UpdateNotificationPreference)
            .WithName("UpdateNotificationPreference");
    }

    public async Task<IResult> GetNotifications(ISender sender, [AsParameters] GetNotificationsQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> CreateNotification(ISender sender, CreateNotificationCommand command)
    {
        var id = await sender.Send(command);
        return Results.Created($"/api/notifications/{id}", new { id });
    }

    public async Task<IResult> MarkNotificationRead(ISender sender, int id)
    {
        var result = await sender.Send(new MarkNotificationReadCommand { NotificationId = id });
        return result ? Results.Ok() : Results.NotFound();
    }

    public async Task<IResult> UpdateNotificationPreference(ISender sender, UpdateNotificationPreferenceCommand command)
    {
        var result = await sender.Send(command);
        return result ? Results.Ok() : Results.BadRequest();
    }
}

