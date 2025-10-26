using Toss.Application.ArtificialIntelligence.Queries.AskAI;
using Toss.Application.ArtificialIntelligence.Queries.GetAISuggestions;

namespace Toss.Web.Endpoints;

public class AICopilot : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.MapPost("ask", AskAI);
        group.MapGet("suggestions", GetAISuggestions);
    }

    public async Task<IResult> AskAI(ISender sender, AskAIQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> GetAISuggestions(ISender sender, [AsParameters] GetAISuggestionsQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }
}

