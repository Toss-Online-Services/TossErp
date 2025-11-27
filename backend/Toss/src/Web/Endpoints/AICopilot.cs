using Toss.Application.ArtificialIntelligence.Commands.GenerateMetaTags;
using Toss.Application.ArtificialIntelligence.Commands.UpdateAISettings;
using Toss.Application.ArtificialIntelligence.Queries.AskAI;
using Toss.Application.ArtificialIntelligence.Queries.GetAISettings;
using Toss.Application.ArtificialIntelligence.Queries.GetAISuggestions;
using Toss.Domain.Constants;

namespace Toss.Web.Endpoints;

public class AICopilot : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.RequireAuthorization(Policies.RequireOwnerOrManager);
        group.MapPost("ask", AskAI);
        group.MapGet("suggestions", GetAISuggestions);
        group.MapPost("meta-tags", GenerateMetaTags);
        group.MapGet("settings/{shopId}", GetAISettings);
        group.MapPut("settings", UpdateAISettings);
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

    public async Task<IResult> GenerateMetaTags(ISender sender, GenerateMetaTagsCommand command)
    {
        var result = await sender.Send(command);
        return Results.Ok(result);
    }

    public async Task<IResult> GetAISettings(ISender sender, int shopId)
    {
        var query = new GetAISettingsQuery { ShopId = shopId };
        var result = await sender.Send(query);
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    public async Task<IResult> UpdateAISettings(ISender sender, UpdateAISettingsCommand command)
    {
        await sender.Send(command);
        return Results.Ok();
    }
}

