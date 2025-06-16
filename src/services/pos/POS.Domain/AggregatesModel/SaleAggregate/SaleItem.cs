using POS.Domain.SeedWork;
using POS.Domain.AggregatesModel.ProductAggregate;

namespace POS.Domain.AggregatesModel.SaleAggregate;

public class SaleItem : Entity
{
    public int SaleId { get; private set; }
    public int ProductId { get; private set; }
    public string? StoreId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Total { get; private set; }
    public string? Category { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private SaleItem() { }

    public SaleItem(int saleId, int productId, string? storeId, int quantity, decimal unitPrice, string? category)
    {
        SaleId = saleId;
        ProductId = productId;
        StoreId = storeId;
        Quantity = quantity;
        UnitPrice = unitPrice;
        Category = category;
        Total = quantity * unitPrice;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateQuantity(int quantity)
    {
        Quantity = quantity;
        Total = quantity * UnitPrice;
    }

    public void UpdateUnitPrice(decimal unitPrice)
    {
        UnitPrice = unitPrice;
        Total = Quantity * unitPrice;
    }
} 
