using MediatR;
using TossErp.Stock.Application.Common.DTOs;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Aggregates.WarehouseAggregate;
using Microsoft.Extensions.Logging;

namespace TossErp.Stock.Application.Warehouses.Commands.DeleteWarehouse;

public class DeleteWarehouseCommandHandler : IRequestHandler<DeleteWarehouseCommand>
{
    private readonly IRepository<WarehouseAggregate> _repository;

    public DeleteWarehouseCommandHandler(IRepository<WarehouseAggregate> repository)
    {
        _repository = repository;
    }

    public async Task Handle(DeleteWarehouseCommand request, CancellationToken cancellationToken)
    {
        var warehouse = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (warehouse == null)
        {
            throw new TossErp.Stock.Domain.Exceptions.NotFoundException($"Warehouse with ID {request.Id} not found");
        }

        await _repository.DeleteAsync(warehouse, cancellationToken);
    }
} 
