#nullable enable
using eShop.POS.Domain.SeedWork;

namespace eShop.POS.Domain.AggregatesModel.SaleAggregate;

public class SaleItem : ValueObject
{
    public string ProductId { get; private set; }
    public string ProductName { get; private set; }
    public decimal Price { get; private set; }
    public int Quantity { get; private set; }
    public decimal Total => Price * Quantity;

    protected SaleItem()
    {
        ProductId = string.Empty;
        ProductName = string.Empty;
    }

    public SaleItem(string productId, string productName, decimal price, int quantity)
    {
        ProductId = productId;
        ProductName = productName;
        Price = price;
        Quantity = quantity;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ProductId;
        yield return ProductName;
        yield return Price;
        yield return Quantity;
    }
} 
