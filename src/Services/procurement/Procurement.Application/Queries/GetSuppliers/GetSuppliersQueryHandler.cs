using MediatR;
using TossErp.Procurement.Application.Common.DTOs;
using TossErp.Procurement.Domain.Common;

namespace TossErp.Procurement.Application.Queries.GetSuppliers;

/// <summary>
/// Handler for GetSuppliersQuery
/// </summary>
public class GetSuppliersQueryHandler : IRequestHandler<GetSuppliersQuery, IEnumerable<SupplierSummaryDto>>
{
    private readonly ISupplierRepository _supplierRepository;

    public GetSuppliersQueryHandler(ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task<IEnumerable<SupplierSummaryDto>> Handle(GetSuppliersQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Domain.Entities.Supplier> suppliers;

        // Apply filters based on request
        if (request.Status.HasValue)
        {
            suppliers = await _supplierRepository.GetByStatusAsync(request.Status.Value, cancellationToken);
        }
        else if (!string.IsNullOrWhiteSpace(request.Name))
        {
            suppliers = await _supplierRepository.GetByNameAsync(request.Name, cancellationToken);
        }
        else if (request.ActiveOnly)
        {
            suppliers = await _supplierRepository.GetActiveAsync(cancellationToken);
        }
        else
        {
            // Get all suppliers
            suppliers = await _supplierRepository.GetAllAsync(cancellationToken);
        }

        // Map to summary DTOs
        return suppliers.Select(MapToSummaryDto);
    }

    private static SupplierSummaryDto MapToSummaryDto(Domain.Entities.Supplier supplier)
    {
        return new SupplierSummaryDto
        {
            Id = supplier.Id,
            Name = supplier.Name,
            Code = supplier.Code,
            Status = supplier.Status,
            ContactPerson = supplier.ContactPerson,
            Email = supplier.Email,
            Phone = supplier.Phone,
            CreditLimit = supplier.CreditLimit,
            PaymentTermsDays = supplier.PaymentTermsDays
        };
    }
}
