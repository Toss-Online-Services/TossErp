namespace Toss.Domain.Entities.Inventory;

public class StockAlert : BaseAuditableEntity
{
    public int ShopId { get; set; }
    public Shop Shop { get; set; } = null!;
    
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    
    public int CurrentStock { get; set; }
    public int MinimumStock { get; set; }
    public bool IsAcknowledged { get; set; }
    public DateTimeOffset? AcknowledgedAt { get; set; }
    public string? AcknowledgedBy { get; set; }
}

