using MediatR;
using TossErp.Stock.Application.Common.DTOs;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Aggregates.WarehouseAggregate.Entities;
using Microsoft.Extensions.Logging;

namespace TossErp.Stock.Application.Bins.Commands.UpdateBin;

public class UpdateBinCommandHandler : IRequestHandler<UpdateBinCommand, Guid>
{
    private readonly IRepository<Bin> _repository;

    public UpdateBinCommandHandler(IRepository<Bin> repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(UpdateBinCommand request, CancellationToken cancellationToken)
    {
        var bin = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (bin == null)
            throw new TossErp.Stock.Domain.Exceptions.NotFoundException(nameof(Bin), request.Id);

        // Only update IsActive for now
        if (request.IsActive && !bin.IsActive)
            bin.Activate();
        else if (!request.IsActive && bin.IsActive)
            bin.Deactivate();

        await _repository.UpdateAsync(bin, cancellationToken);
        return bin.Id;
    }
} 
