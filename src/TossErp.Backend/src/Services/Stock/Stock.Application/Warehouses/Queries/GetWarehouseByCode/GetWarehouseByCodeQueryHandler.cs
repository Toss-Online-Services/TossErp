using MediatR;
using Microsoft.Extensions.Logging;
using TossErp.Stock.Application.Common.DTOs;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Application.Common.Mappings;

namespace TossErp.Stock.Application.Warehouses.Queries.GetWarehouseByCode;

public class GetWarehouseByCodeQueryHandler : IRequestHandler<GetWarehouseByCodeQuery, WarehouseDto?>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly ILogger<GetWarehouseByCodeQueryHandler> _logger;

    public GetWarehouseByCodeQueryHandler(
        IWarehouseRepository warehouseRepository,
        ILogger<GetWarehouseByCodeQueryHandler> logger)
    {
        _warehouseRepository = warehouseRepository;
        _logger = logger;
    }

    public async Task<WarehouseDto?> Handle(GetWarehouseByCodeQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting warehouse by code: {Code}", request.Code);

        var warehouse = await _warehouseRepository.GetByCodeAsync(request.Code, cancellationToken);
        
        return warehouse?.ToDto();
    }
} 
