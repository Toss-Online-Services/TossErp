using Toss.Application.Audit.Commands.LogAuditEvent;

namespace Toss.Web.Endpoints;

public class Audit : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.MapPost("log", LogAuditEvent)
            .WithName("LogAuditEvent")
            .AllowAnonymous(); // Allow anonymous for logging failed auth attempts
    }

    public async Task<IResult> LogAuditEvent(ISender sender, LogAuditEventCommand command)
    {
        var auditId = await sender.Send(command);
        return Results.Ok(new { id = auditId, message = "Audit event logged successfully" });
    }
}


