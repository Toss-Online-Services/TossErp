using MediatR;
using TossErp.Procurement.Application.Common.DTOs;
using TossErp.Procurement.Application.Common.Interfaces;
using TossErp.Procurement.Domain.Common;

namespace TossErp.Procurement.Application.Queries.GetSupplierById;

/// <summary>
/// Handler for GetSupplierByIdQuery
/// </summary>
public class GetSupplierByIdQueryHandler : IRequestHandler<GetSupplierByIdQuery, SupplierDto?>
{
    private readonly ISupplierRepository _supplierRepository;

    public GetSupplierByIdQueryHandler(ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task<SupplierDto?> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (supplier == null)
            return null;

        return MapToDto(supplier);
    }

    private static SupplierDto MapToDto(Domain.Entities.Supplier supplier)
    {
        return new SupplierDto
        {
            Id = supplier.Id,
            Name = supplier.Name,
            Code = supplier.Code,
            ContactPerson = supplier.ContactPerson,
            Email = supplier.Email,
            Phone = supplier.Phone,
            Address = supplier.Address,
            City = supplier.City,
            PostalCode = supplier.PostalCode,
            Country = supplier.Country,
            TaxNumber = supplier.TaxNumber,
            RegistrationNumber = supplier.RegistrationNumber,
            Status = supplier.Status,
            Notes = supplier.Notes,
            CreditLimit = supplier.CreditLimit,
            PaymentTermsDays = supplier.PaymentTermsDays,
            LeadTimeDays = supplier.LeadTimeDays,
            CreatedAt = supplier.CreatedAt,
            UpdatedAt = supplier.UpdatedAt,
            CreatedBy = supplier.CreatedBy,
            UpdatedBy = supplier.UpdatedBy,
            TenantId = supplier.TenantId
        };
    }
}
