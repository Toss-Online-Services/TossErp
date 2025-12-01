using Toss.Application.Support.Commands.CreateTicket;
using Toss.Application.Support.Commands.UpdateTicket;
using Toss.Application.Support.Commands.ChangeTicketStatus;
using Toss.Application.Support.Commands.AddTicketNote;
using Toss.Application.Support.Queries.GetTickets;
using Toss.Application.Support.Queries.GetTicketById;
using Toss.Domain.Constants;

namespace Toss.Web.Endpoints;

public class Support : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.RequireAuthorization(Policies.RequireOwnerOrManager);

        // Tickets
        group.MapGet("tickets", GetTickets)
            .WithName("GetTickets");

        group.MapGet("tickets/{id}", GetTicketById)
            .WithName("GetTicketById");

        group.MapPost("tickets", CreateTicket)
            .WithName("CreateTicket");

        group.MapPut("tickets/{id}", UpdateTicket)
            .WithName("UpdateTicket");

        group.MapPatch("tickets/{id}/status", ChangeTicketStatus)
            .WithName("ChangeTicketStatus");

        // Ticket Notes
        group.MapPost("tickets/{id}/notes", AddTicketNote)
            .WithName("AddTicketNote");
    }

    public async Task<IResult> GetTickets(ISender sender, [AsParameters] GetTicketsQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> GetTicketById(ISender sender, int id)
    {
        var result = await sender.Send(new GetTicketByIdQuery { Id = id });
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    public async Task<IResult> CreateTicket(ISender sender, CreateTicketCommand command)
    {
        var id = await sender.Send(command);
        return Results.Created($"/api/support/tickets/{id}", new { id });
    }

    public async Task<IResult> UpdateTicket(ISender sender, int id, UpdateTicketCommand command)
    {
        var result = await sender.Send(command with { Id = id });
        return result ? Results.Ok() : Results.NotFound();
    }

    public async Task<IResult> ChangeTicketStatus(ISender sender, int id, ChangeTicketStatusCommand command)
    {
        var result = await sender.Send(command with { TicketId = id });
        return result ? Results.Ok() : Results.NotFound();
    }

    public async Task<IResult> AddTicketNote(ISender sender, int id, AddTicketNoteCommand command)
    {
        var noteId = await sender.Send(command with { TicketId = id });
        return Results.Created($"/api/support/tickets/{id}/notes/{noteId}", new { id = noteId });
    }
}

