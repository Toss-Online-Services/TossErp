using TossErp.Domain.SeedWork;

namespace TossErp.Domain.AggregatesModel.ProductAggregate;

public class StockMovement : Entity
{
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public string MovementType { get; private set; } = string.Empty; // "in", "out", "adjustment"
    public string Reason { get; private set; } = string.Empty;
    public int PreviousStock { get; private set; }
    public int NewStock { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid? CreatedBy { get; private set; }
    public string? ReferenceNumber { get; private set; } // For linking to sales, purchases, etc.

    protected StockMovement() 
    {
        MovementType = string.Empty;
        Reason = string.Empty;
        CreatedAt = DateTime.UtcNow;
    }

    public StockMovement(Guid productId, int quantity, string movementType, string reason, 
                        int previousStock, int newStock, Guid? createdBy = null, string? referenceNumber = null)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        Quantity = quantity;
        MovementType = movementType;
        Reason = reason;
        PreviousStock = previousStock;
        NewStock = newStock;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        ReferenceNumber = referenceNumber;
    }
} 
