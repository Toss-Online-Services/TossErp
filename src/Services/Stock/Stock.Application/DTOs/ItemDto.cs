using System;

namespace TossErp.Stock.Application.DTOs;

public class ItemDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string SKU { get; set; } = null!;
    public string? Barcode { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string Category { get; set; } = null!;
    public string Unit { get; set; } = null!;
    public decimal SellingPrice { get; set; }
    public decimal? CostPrice { get; set; }
    public decimal ReorderLevel { get; set; }
    public decimal ReorderQty { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
