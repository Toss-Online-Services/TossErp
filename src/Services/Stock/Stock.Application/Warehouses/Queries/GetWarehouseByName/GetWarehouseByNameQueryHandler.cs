using MediatR;
using Microsoft.Extensions.Logging;
using TossErp.Stock.Application.Common.DTOs;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Application.Common.Mappings;

namespace TossErp.Stock.Application.Warehouses.Queries.GetWarehouseByName;

public class GetWarehouseByNameQueryHandler : IRequestHandler<GetWarehouseByNameQuery, WarehouseDto?>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly ILogger<GetWarehouseByNameQueryHandler> _logger;

    public GetWarehouseByNameQueryHandler(
        IWarehouseRepository warehouseRepository,
        ILogger<GetWarehouseByNameQueryHandler> logger)
    {
        _warehouseRepository = warehouseRepository;
        _logger = logger;
    }

    public async Task<WarehouseDto?> Handle(GetWarehouseByNameQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting warehouse by name: {Name}", request.Name);

        var warehouse = await _warehouseRepository.GetByNameAsync(request.Name, cancellationToken);
        
        return warehouse?.ToDto();
    }
} 
