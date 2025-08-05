using TossErp.Stock.Application.Bins.Commands.CreateBin;
using TossErp.Stock.Application.Bins.Commands.UpdateBin;
using TossErp.Stock.Application.Bins.Commands.DeleteBin;
using TossErp.Stock.Application.Bins.Queries.GetBins;
using TossErp.Stock.Application.Bins.Queries.GetBinById;
using TossErp.Stock.Application.Common.DTOs;

namespace TossErp.Stock.API.Endpoints;

public class Bins : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(nameof(Bins));
        group.MapGet(GetBins);
        group.MapGet(GetBin, "{id}");
        group.MapPost(CreateBin);
        group.MapPut(UpdateBin, "{id}");
        group.MapDelete(DeleteBin, "{id}");
    }

    public async Task<List<BinDto>> GetBins(ISender sender, [AsParameters] GetBinsQuery query)
    {
        return await sender.Send(query);
    }

    public async Task<BinDto> GetBin(ISender sender, Guid id)
    {
        return await sender.Send(new GetBinByIdQuery(id));
    }

    public async Task<BinDto> CreateBin(ISender sender, CreateBinCommand command)
    {
        return await sender.Send(command);
    }

    public async Task<IResult> UpdateBin(ISender sender, Guid id, UpdateBinCommand command)
    {
        if (id != command.Id) return Results.BadRequest();
        await sender.Send(command);
        return Results.NoContent();
    }

    public async Task<IResult> DeleteBin(ISender sender, Guid id)
    {
        await sender.Send(new DeleteBinCommand(id));
        return Results.NoContent();
    }
} 
