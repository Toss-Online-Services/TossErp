using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Suppliers;

namespace Toss.Application.Suppliers.Commands.UpdateSupplierPricing;

public record UpdateSupplierPricingCommand : IRequest<int>
{
    public int SupplierProductId { get; init; }
    public decimal NewBasePrice { get; init; }
    public int? NewLeadTimeDays { get; init; }
    public int? NewMinOrderQuantity { get; init; }
    public DateTime EffectiveDate { get; init; } = DateTime.UtcNow;
}

public class UpdateSupplierPricingCommandHandler : IRequestHandler<UpdateSupplierPricingCommand, int>
{
    private readonly IApplicationDbContext _context;

    public UpdateSupplierPricingCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(UpdateSupplierPricingCommand request, CancellationToken cancellationToken)
    {
        var supplierProduct = await _context.SupplierProducts
            .FindAsync(new object[] { request.SupplierProductId }, cancellationToken);

        if (supplierProduct == null)
            throw new NotFoundException(nameof(SupplierProduct), request.SupplierProductId.ToString());

        // Create pricing history record
        var pricingRecord = new SupplierPricing
        {
            SupplierProductId = request.SupplierProductId,
            BasePrice = request.NewBasePrice,
            EffectiveDate = request.EffectiveDate,
            CreatedDate = DateTime.UtcNow
        };

        _context.SupplierPricings.Add(pricingRecord);

        // Update current pricing
        supplierProduct.BasePrice = request.NewBasePrice;
        
        if (request.NewLeadTimeDays.HasValue)
            supplierProduct.LeadTimeDays = request.NewLeadTimeDays.Value;

        if (request.NewMinOrderQuantity.HasValue)
            supplierProduct.MinOrderQuantity = request.NewMinOrderQuantity.Value;

        await _context.SaveChangesAsync(cancellationToken);

        return pricingRecord.Id;
    }
}

