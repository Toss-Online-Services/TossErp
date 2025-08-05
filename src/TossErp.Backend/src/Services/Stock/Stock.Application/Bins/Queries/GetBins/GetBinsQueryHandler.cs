using MediatR;
using TossErp.Stock.Application.Common.DTOs;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Aggregates.WarehouseAggregate.Entities;
using Microsoft.Extensions.Logging;

namespace TossErp.Stock.Application.Bins.Queries.GetBins;

public class GetBinsQueryHandler : IRequestHandler<GetBinsQuery, List<BinDto>>
{
    private readonly IRepository<Bin> _repository;

    public GetBinsQueryHandler(IRepository<Bin> repository)
    {
        _repository = repository;
    }

    public async Task<List<BinDto>> Handle(GetBinsQuery request, CancellationToken cancellationToken)
    {
        var bins = await _repository.GetAllAsync(cancellationToken);
        return bins.Select(bin => new BinDto
        {
            Id = bin.Id,
            BinCode = bin.BinCode.Value,
            WarehouseCode = string.Empty, // Not available in Bin entity
            ItemCode = string.Empty, // Not available in Bin entity
            Quantity = 0, // Not available in Bin entity
            IsActive = bin.IsActive,
            Location = bin.Description ?? string.Empty, // Use Description as Location
            Created = DateTime.UtcNow, // Use current time since entity doesn't expose Created
            CreatedBy = string.Empty, // Use empty string since entity doesn't expose CreatedBy
            LastModified = DateTime.UtcNow, // Use current time since entity doesn't expose LastModified
            LastModifiedBy = string.Empty // Use empty string since entity doesn't expose LastModifiedBy
        }).ToList();
    }
} 
