using MediatR;
using Microsoft.Extensions.Logging;
using TossErp.Stock.Application.Common.DTOs;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Application.Common.Mappings;

namespace TossErp.Stock.Application.Warehouses.Queries.GetWarehousesByItem;

public class GetWarehousesByItemQueryHandler : IRequestHandler<GetWarehousesByItemQuery, List<WarehouseDto>>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly ILogger<GetWarehousesByItemQueryHandler> _logger;

    public GetWarehousesByItemQueryHandler(
        IWarehouseRepository warehouseRepository,
        ILogger<GetWarehousesByItemQueryHandler> logger)
    {
        _warehouseRepository = warehouseRepository;
        _logger = logger;
    }

    public async Task<List<WarehouseDto>> Handle(GetWarehousesByItemQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting warehouses containing item: {ItemId}", request.ItemId);

        var warehouses = await _warehouseRepository.GetByItemAsync(request.ItemId, cancellationToken);
        
        return warehouses.Select(w => w.ToDto()).ToList();
    }
} 
