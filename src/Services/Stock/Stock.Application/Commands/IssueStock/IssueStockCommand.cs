using System.ComponentModel.DataAnnotations;
using MediatR;

namespace TossErp.Stock.Application.Commands.IssueStock;

public class IssueStockCommand : IRequest<bool>
{
    [Required]
    public Guid TenantId { get; set; }

    [Required]
    public Guid ItemId { get; set; }

    [Required]
    public Guid WarehouseId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than zero")]
    public decimal Quantity { get; set; }

    [Required]
    [StringLength(20)]
    public string Unit { get; set; } = null!;

    [StringLength(50)]
    public string? RefType { get; set; }

    [StringLength(100)]
    public string? RefId { get; set; }

    [StringLength(200)]
    public string? Reason { get; set; }

    [Required]
    [StringLength(100)]
    public string CreatedBy { get; set; } = null!;
}
