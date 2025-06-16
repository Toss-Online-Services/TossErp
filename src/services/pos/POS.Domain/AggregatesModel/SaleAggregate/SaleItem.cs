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
    public string? ProductName { get; private set; }
    public string? Barcode { get; private set; }
    public string? Variant { get; private set; }
    public decimal TaxRate { get; private set; }
    public decimal SubTotal { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public decimal Discount { get; private set; }
    public DateTime UpdatedAt { get; private set; }

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
        UpdatedAt = DateTime.UtcNow;
        SubTotal = Total;
        TotalAmount = Total;
    }

    public void UpdateQuantity(int quantity)
    {
        Quantity = quantity;
        Total = quantity * UnitPrice;
        SubTotal = Total;
        TotalAmount = Total + TaxAmount - Discount;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateUnitPrice(decimal unitPrice)
    {
        UnitPrice = unitPrice;
        Total = Quantity * unitPrice;
        SubTotal = Total;
        TotalAmount = Total + TaxAmount - Discount;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateTaxRate(decimal taxRate)
    {
        TaxRate = taxRate;
        TaxAmount = SubTotal * (taxRate / 100);
        TotalAmount = SubTotal + TaxAmount - Discount;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDiscount(decimal discount)
    {
        Discount = discount;
        TotalAmount = SubTotal + TaxAmount - discount;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateProductDetails(string productName, string? barcode, string? variant)
    {
        ProductName = productName;
        Barcode = barcode;
        Variant = variant;
        UpdatedAt = DateTime.UtcNow;
    }
} 
