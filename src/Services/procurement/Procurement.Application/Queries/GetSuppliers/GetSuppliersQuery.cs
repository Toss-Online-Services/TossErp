using MediatR;
using TossErp.Procurement.Application.Common.DTOs;
using TossErp.Procurement.Domain.Enums;

namespace TossErp.Procurement.Application.Queries.GetSuppliers;

/// <summary>
/// Query to get suppliers with optional filtering
/// </summary>
public class GetSuppliersQuery : IRequest<IEnumerable<SupplierSummaryDto>>
{
    public SupplierStatus? Status { get; set; }
    public string? Name { get; set; }
    public bool ActiveOnly { get; set; } = false;
}
