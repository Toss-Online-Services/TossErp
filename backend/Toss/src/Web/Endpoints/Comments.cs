using Toss.Application.Notifications.Commands.CreateComment;
using Toss.Application.Notifications.Commands.UpdateComment;
using Toss.Application.Notifications.Commands.DeleteComment;
using Toss.Application.Notifications.Queries.GetComments;
using Toss.Domain.Constants;

namespace Toss.Web.Endpoints;

public class Comments : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.RequireAuthorization(Policies.RequireStaffOrAbove);

        group.MapGet(string.Empty, GetComments)
            .WithName("GetComments");

        group.MapPost(string.Empty, CreateComment)
            .WithName("CreateComment");

        group.MapPut("{id}", UpdateComment)
            .WithName("UpdateComment");

        group.MapDelete("{id}", DeleteComment)
            .WithName("DeleteComment");
    }

    public async Task<IResult> GetComments(ISender sender, [AsParameters] GetCommentsQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> CreateComment(ISender sender, CreateCommentCommand command)
    {
        var id = await sender.Send(command);
        return Results.Created($"/api/comments/{id}", new { id });
    }

    public async Task<IResult> UpdateComment(ISender sender, int id, UpdateCommentCommand command)
    {
        var result = await sender.Send(command with { CommentId = id });
        return result ? Results.Ok() : Results.NotFound();
    }

    public async Task<IResult> DeleteComment(ISender sender, int id)
    {
        var result = await sender.Send(new DeleteCommentCommand { CommentId = id });
        return result ? Results.Ok() : Results.NotFound();
    }
}

