using System.ComponentModel.DataAnnotations;
using MediatR;

namespace TossErp.Stock.Application.Commands.CreateItem;

public class CreateItemCommand : IRequest<Guid>
{
    [Required]
    public Guid TenantId { get; set; }

    [Required]
    [StringLength(50)]
    public string SKU { get; set; } = null!;

    [StringLength(100)]
    public string? Barcode { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = null!;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    [StringLength(100)]
    public string Category { get; set; } = null!;

    [Required]
    [StringLength(20)]
    public string Unit { get; set; } = null!;

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Selling price must be greater than zero")]
    public decimal SellingPrice { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Cost price cannot be negative")]
    public decimal? CostPrice { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Reorder level cannot be negative")]
    public decimal ReorderLevel { get; set; } = 0;

    [Range(0, double.MaxValue, ErrorMessage = "Reorder quantity cannot be negative")]
    public decimal ReorderQty { get; set; } = 0;
}
