using MediatR;
using Microsoft.Extensions.Logging;
using TossErp.Stock.Application.Common.DTOs;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Application.Common.Mappings;

namespace TossErp.Stock.Application.Warehouses.Queries.GetWarehousesByBin;

public class GetWarehousesByBinQueryHandler : IRequestHandler<GetWarehousesByBinQuery, List<WarehouseDto>>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly ILogger<GetWarehousesByBinQueryHandler> _logger;

    public GetWarehousesByBinQueryHandler(
        IWarehouseRepository warehouseRepository,
        ILogger<GetWarehousesByBinQueryHandler> logger)
    {
        _warehouseRepository = warehouseRepository;
        _logger = logger;
    }

    public async Task<List<WarehouseDto>> Handle(GetWarehousesByBinQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting warehouses containing bin: {BinId}", request.BinId);

        var warehouses = await _warehouseRepository.GetByBinAsync(request.BinId, cancellationToken);
        
        return warehouses.Select(w => w.ToDto()).ToList();
    }
} 
