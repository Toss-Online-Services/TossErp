using MediatR;
using Microsoft.Extensions.Logging;
using TossErp.Stock.Application.Common.DTOs;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Application.Common.Mappings;

namespace TossErp.Stock.Application.Warehouses.Queries.GetWarehousesByUtilization;

public class GetWarehousesByUtilizationQueryHandler : IRequestHandler<GetWarehousesByUtilizationQuery, List<WarehouseDto>>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly ILogger<GetWarehousesByUtilizationQueryHandler> _logger;

    public GetWarehousesByUtilizationQueryHandler(
        IWarehouseRepository warehouseRepository,
        ILogger<GetWarehousesByUtilizationQueryHandler> logger)
    {
        _warehouseRepository = warehouseRepository;
        _logger = logger;
    }

    public async Task<List<WarehouseDto>> Handle(GetWarehousesByUtilizationQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting warehouses by utilization range: {MinUtilization} - {MaxUtilization}", 
            request.MinUtilization, request.MaxUtilization);

        var warehouses = await _warehouseRepository.GetByUtilizationRangeAsync(
            request.MinUtilization, request.MaxUtilization, cancellationToken);
        
        return warehouses.Select(w => w.ToDto()).ToList();
    }
} 
