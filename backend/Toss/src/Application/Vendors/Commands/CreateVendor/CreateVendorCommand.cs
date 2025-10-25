using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Vendors;

namespace Toss.Application.Vendors.Commands.CreateVendor;

public record CreateVendorCommand : IRequest<int>
{
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool Active { get; init; } = true;
    public int DisplayOrder { get; init; }
}

public class CreateVendorCommandHandler : IRequestHandler<CreateVendorCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateVendorCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateVendorCommand request, CancellationToken cancellationToken)
    {
        var vendor = new Vendor
        {
            Name = request.Name,
            Email = request.Email,
            Description = request.Description,
            Active = request.Active,
            DisplayOrder = request.DisplayOrder
        };

        _context.Vendors.Add(vendor);
        await _context.SaveChangesAsync(cancellationToken);

        return vendor.Id;
    }
}

