using TossErp.Domain.SeedWork;

namespace TossErp.Domain.AggregatesModel.SaleAggregate;

public class SaleItem : Entity
{
    public Guid SaleId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal? Discount { get; private set; }
    public decimal TotalPrice { get; private set; }

    protected SaleItem() 
    {
        ProductName = string.Empty;
    }

    public SaleItem(Guid saleId, Guid productId, string productName, int quantity, decimal unitPrice, decimal? discount = null)
    {
        Id = Guid.NewGuid();
        SaleId = saleId;
        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
        Discount = discount ?? 0;
        CalculateTotalPrice();
    }

    public void UpdateQuantity(int quantity)
    {
        Quantity = quantity;
        CalculateTotalPrice();
    }

    public void UpdateUnitPrice(decimal unitPrice)
    {
        UnitPrice = unitPrice;
        CalculateTotalPrice();
    }

    public void UpdateDiscount(decimal discount)
    {
        Discount = discount;
        CalculateTotalPrice();
    }

    private void CalculateTotalPrice()
    {
        var subtotal = Quantity * UnitPrice;
        TotalPrice = subtotal - (Discount ?? 0);
    }
} 
