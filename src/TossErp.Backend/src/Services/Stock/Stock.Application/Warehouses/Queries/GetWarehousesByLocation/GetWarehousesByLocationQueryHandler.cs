using MediatR;
using Microsoft.Extensions.Logging;
using TossErp.Stock.Application.Common.DTOs;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Application.Common.Mappings;

namespace TossErp.Stock.Application.Warehouses.Queries.GetWarehousesByLocation;

public class GetWarehousesByLocationQueryHandler : IRequestHandler<GetWarehousesByLocationQuery, List<WarehouseDto>>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly ILogger<GetWarehousesByLocationQueryHandler> _logger;

    public GetWarehousesByLocationQueryHandler(
        IWarehouseRepository warehouseRepository,
        ILogger<GetWarehousesByLocationQueryHandler> logger)
    {
        _warehouseRepository = warehouseRepository;
        _logger = logger;
    }

    public async Task<List<WarehouseDto>> Handle(GetWarehousesByLocationQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting warehouses by location: {Location}", request.Location);

        var warehouses = await _warehouseRepository.GetByLocationAsync(request.Location, cancellationToken);
        
        return warehouses.Select(w => w.ToDto()).ToList();
    }
} 
