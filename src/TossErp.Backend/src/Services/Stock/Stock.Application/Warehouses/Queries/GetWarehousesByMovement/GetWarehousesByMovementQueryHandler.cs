using MediatR;
using Microsoft.Extensions.Logging;
using TossErp.Stock.Application.Common.DTOs;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Application.Common.Mappings;

namespace TossErp.Stock.Application.Warehouses.Queries.GetWarehousesByMovement;

public class GetWarehousesByMovementQueryHandler : IRequestHandler<GetWarehousesByMovementQuery, List<WarehouseDto>>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly ILogger<GetWarehousesByMovementQueryHandler> _logger;

    public GetWarehousesByMovementQueryHandler(
        IWarehouseRepository warehouseRepository,
        ILogger<GetWarehousesByMovementQueryHandler> logger)
    {
        _warehouseRepository = warehouseRepository;
        _logger = logger;
    }

    public async Task<List<WarehouseDto>> Handle(GetWarehousesByMovementQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting warehouses involved in movement: {MovementId}", request.MovementId);

        var warehouses = await _warehouseRepository.GetByMovementAsync(request.MovementId, cancellationToken);
        
        return warehouses.Select(w => w.ToDto()).ToList();
    }
} 
