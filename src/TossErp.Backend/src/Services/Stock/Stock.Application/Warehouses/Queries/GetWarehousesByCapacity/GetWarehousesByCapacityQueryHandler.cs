using MediatR;
using Microsoft.Extensions.Logging;
using TossErp.Stock.Application.Common.DTOs;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Application.Common.Mappings;

namespace TossErp.Stock.Application.Warehouses.Queries.GetWarehousesByCapacity;

public class GetWarehousesByCapacityQueryHandler : IRequestHandler<GetWarehousesByCapacityQuery, List<WarehouseDto>>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly ILogger<GetWarehousesByCapacityQueryHandler> _logger;

    public GetWarehousesByCapacityQueryHandler(
        IWarehouseRepository warehouseRepository,
        ILogger<GetWarehousesByCapacityQueryHandler> logger)
    {
        _warehouseRepository = warehouseRepository;
        _logger = logger;
    }

    public async Task<List<WarehouseDto>> Handle(GetWarehousesByCapacityQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting warehouses by capacity range: {MinCapacity} - {MaxCapacity}", 
            request.MinCapacity, request.MaxCapacity);

        var warehouses = await _warehouseRepository.GetByCapacityRangeAsync(
            request.MinCapacity, request.MaxCapacity, cancellationToken);
        
        return warehouses.Select(w => w.ToDto()).ToList();
    }
} 
