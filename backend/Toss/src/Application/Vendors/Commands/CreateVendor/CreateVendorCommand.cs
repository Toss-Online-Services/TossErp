using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Vendors;
using Toss.Domain.ValueObjects;

namespace Toss.Application.Vendors.Commands.CreateVendor;

public record CreateVendorCommand : IRequest<int>
{
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? CompanyRegNumber { get; init; }
    public string? VATNumber { get; init; }
    public string? ContactPerson { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Website { get; init; }
    public int? PaymentTermsDays { get; init; }
    public decimal? CreditLimit { get; init; }
    public string? AdminComment { get; init; }
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
            CompanyRegNumber = request.CompanyRegNumber,
            VATNumber = request.VATNumber,
            ContactPerson = request.ContactPerson,
            ContactPhone = !string.IsNullOrWhiteSpace(request.PhoneNumber) 
                ? new PhoneNumber(request.PhoneNumber) 
                : null,
            Website = request.Website,
            Active = request.Active,
            CreditLimit = request.CreditLimit,
            PaymentTermsDays = request.PaymentTermsDays ?? 30,
            DisplayOrder = request.DisplayOrder,
            AdminComment = request.AdminComment,
            Deleted = false
        };

        _context.Vendors.Add(vendor);
        await _context.SaveChangesAsync(cancellationToken);

        return vendor.Id;
    }
}
