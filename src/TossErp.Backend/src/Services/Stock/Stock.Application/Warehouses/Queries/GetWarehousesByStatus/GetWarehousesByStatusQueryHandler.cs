using MediatR;
using Microsoft.Extensions.Logging;
using TossErp.Stock.Application.Common.DTOs;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Application.Common.Mappings;

namespace TossErp.Stock.Application.Warehouses.Queries.GetWarehousesByStatus;

public class GetWarehousesByStatusQueryHandler : IRequestHandler<GetWarehousesByStatusQuery, List<WarehouseDto>>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly ILogger<GetWarehousesByStatusQueryHandler> _logger;

    public GetWarehousesByStatusQueryHandler(
        IWarehouseRepository warehouseRepository,
        ILogger<GetWarehousesByStatusQueryHandler> logger)
    {
        _warehouseRepository = warehouseRepository;
        _logger = logger;
    }

    public async Task<List<WarehouseDto>> Handle(GetWarehousesByStatusQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting warehouses by status: {Status}", request.Status);

        var warehouses = await _warehouseRepository.GetByStatusAsync(request.Status, cancellationToken);
        
        return warehouses.Select(w => w.ToDto()).ToList();
    }
} 
