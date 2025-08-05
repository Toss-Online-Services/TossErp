using MediatR;
using Microsoft.Extensions.Logging;
using TossErp.Stock.Application.Common.DTOs;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Application.Common.Mappings;

namespace TossErp.Stock.Application.Warehouses.Queries.GetWarehousesByValue;

public class GetWarehousesByValueQueryHandler : IRequestHandler<GetWarehousesByValueQuery, List<WarehouseDto>>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly ILogger<GetWarehousesByValueQueryHandler> _logger;

    public GetWarehousesByValueQueryHandler(
        IWarehouseRepository warehouseRepository,
        ILogger<GetWarehousesByValueQueryHandler> logger)
    {
        _warehouseRepository = warehouseRepository;
        _logger = logger;
    }

    public async Task<List<WarehouseDto>> Handle(GetWarehousesByValueQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting warehouses by value range: {MinValue} - {MaxValue}", 
            request.MinValue, request.MaxValue);

        var warehouses = await _warehouseRepository.GetByValueRangeAsync(
            request.MinValue, request.MaxValue, cancellationToken);
        
        return warehouses.Select(w => w.ToDto()).ToList();
    }
} 
