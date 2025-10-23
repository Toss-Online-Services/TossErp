using Toss.Application.AICopilot.Queries.AskAI;

namespace Toss.Web.Endpoints;

public class AICopilot : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.MapPost("ask", AskAI);
    }

    public async Task<IResult> AskAI(ISender sender, AskAIQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }
}

