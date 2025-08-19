using MediatR;
using TossErp.Procurement.Application.Common.DTOs;

namespace TossErp.Procurement.Application.Queries.GetSupplierById;

/// <summary>
/// Query to get a specific supplier by ID
/// </summary>
public class GetSupplierByIdQuery : IRequest<SupplierDto?>
{
    public Guid Id { get; set; }
}
