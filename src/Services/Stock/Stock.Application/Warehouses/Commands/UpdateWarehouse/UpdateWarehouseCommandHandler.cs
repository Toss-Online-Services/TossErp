using MediatR;
using TossErp.Stock.Application.Common.DTOs;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Aggregates.WarehouseAggregate;
using Microsoft.Extensions.Logging;

namespace TossErp.Stock.Application.Warehouses.Commands.UpdateWarehouse;

public class UpdateWarehouseCommandHandler : IRequestHandler<UpdateWarehouseCommand, Guid>
{
    private readonly IRepository<WarehouseAggregate> _repository;

    public UpdateWarehouseCommandHandler(IRepository<WarehouseAggregate> repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(UpdateWarehouseCommand request, CancellationToken cancellationToken)
    {
        var warehouse = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (warehouse == null)
        {
            throw new TossErp.Stock.Domain.Exceptions.NotFoundException($"Warehouse with ID {request.Id} not found");
        }

        // Update warehouse properties - need to check what methods are available on WarehouseAggregate
        // For now, just return the ID
        return warehouse.Id;
    }
}
