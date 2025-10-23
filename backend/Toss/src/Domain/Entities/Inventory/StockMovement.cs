namespace Toss.Domain.Entities.Inventory;

public class StockMovement : BaseAuditableEntity
{
    public int ShopId { get; set; }
    public Shop Shop { get; set; } = null!;
    
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    
    public StockMovementType MovementType { get; set; }
    public int QuantityBefore { get; set; }
    public int QuantityChange { get; set; }
    public int QuantityAfter { get; set; }
    
    // Reference to source transaction
    public string? ReferenceType { get; set; } // "Sale", "Purchase", "Adjustment"
    public int? ReferenceId { get; set; }
    
    public string? Notes { get; set; }
    public DateTimeOffset MovementDate { get; set; }
}

