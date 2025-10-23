using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Suppliers;
using Toss.Domain.ValueObjects;

namespace Toss.Application.Suppliers.Commands.CreateSupplier;

public record CreateSupplierCommand : IRequest<int>
{
    public string Name { get; init; } = string.Empty;
    public string? CompanyRegNumber { get; init; }
    public string? VATNumber { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Email { get; init; }
    public string? Website { get; init; }
    public int? PaymentTermsDays { get; init; }
    public decimal? CreditLimit { get; init; }
    public string? Notes { get; init; }
}

public class CreateSupplierCommandHandler : IRequestHandler<CreateSupplierCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateSupplierCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = new Supplier
        {
            Name = request.Name,
            CompanyRegNumber = request.CompanyRegNumber,
            VATNumber = request.VATNumber,
            ContactPhone = !string.IsNullOrWhiteSpace(request.PhoneNumber) 
                ? new PhoneNumber(request.PhoneNumber) 
                : null,
            Email = request.Email,
            Website = request.Website,
            PaymentTermsDays = request.PaymentTermsDays ?? 30,
            CreditLimit = request.CreditLimit,
            IsActive = true,
            Notes = request.Notes
        };

        _context.Suppliers.Add(supplier);
        await _context.SaveChangesAsync(cancellationToken);

        return supplier.Id;
    }
}

