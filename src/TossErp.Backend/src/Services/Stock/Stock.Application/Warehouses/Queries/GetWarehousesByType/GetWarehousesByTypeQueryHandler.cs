using MediatR;
using Microsoft.Extensions.Logging;
using TossErp.Stock.Application.Common.DTOs;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Application.Common.Mappings;

namespace TossErp.Stock.Application.Warehouses.Queries.GetWarehousesByType;

public class GetWarehousesByTypeQueryHandler : IRequestHandler<GetWarehousesByTypeQuery, List<WarehouseDto>>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly ILogger<GetWarehousesByTypeQueryHandler> _logger;

    public GetWarehousesByTypeQueryHandler(
        IWarehouseRepository warehouseRepository,
        ILogger<GetWarehousesByTypeQueryHandler> logger)
    {
        _warehouseRepository = warehouseRepository;
        _logger = logger;
    }

    public async Task<List<WarehouseDto>> Handle(GetWarehousesByTypeQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting warehouses by type: {Type}", request.Type);

        var warehouses = await _warehouseRepository.GetByTypeAsync(request.Type, cancellationToken);
        
        return warehouses.Select(w => w.ToDto()).ToList();
    }
} 
