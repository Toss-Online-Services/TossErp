using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Vendors;

namespace Toss.Application.Vendors.Commands.UpdateVendorPricing;

public record UpdateVendorPricingCommand : IRequest<int>
{
    public int VendorProductId { get; init; }
    public decimal NewBasePrice { get; init; }
    public int? NewLeadTimeDays { get; init; }
    public int? NewMinOrderQuantity { get; init; }
    public DateTime EffectiveDate { get; init; } = DateTime.UtcNow;
}

public class UpdateVendorPricingCommandHandler : IRequestHandler<UpdateVendorPricingCommand, int>
{
    private readonly IApplicationDbContext _context;

    public UpdateVendorPricingCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(UpdateVendorPricingCommand request, CancellationToken cancellationToken)
    {
        var vendorProduct = await _context.VendorProducts
            .FindAsync(new object[] { request.VendorProductId }, cancellationToken);

        if (vendorProduct == null)
            throw new NotFoundException(nameof(VendorProduct), request.VendorProductId.ToString());

        // Create pricing history record
        var pricingRecord = new VendorPricing
        {
            VendorProductId = request.VendorProductId,
            UnitPrice = request.NewBasePrice,
            ValidFrom = request.EffectiveDate
        };

        _context.VendorPricings.Add(pricingRecord);

        // Update current pricing
        vendorProduct.BasePrice = request.NewBasePrice;
        
        if (request.NewLeadTimeDays.HasValue)
            vendorProduct.LeadTimeDays = request.NewLeadTimeDays.Value;

        if (request.NewMinOrderQuantity.HasValue)
            vendorProduct.MinOrderQuantity = request.NewMinOrderQuantity.Value;

        await _context.SaveChangesAsync(cancellationToken);

        return pricingRecord.Id;
    }
}

