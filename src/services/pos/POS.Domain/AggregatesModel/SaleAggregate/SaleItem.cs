using eShop.POS.Domain.SeedWork;

namespace eShop.POS.Domain.AggregatesModel.SaleAggregate;

public class SaleItem : ValueObject
{
    public string ProductId { get; private set; } = string.Empty;
    public string ProductName { get; private set; } = string.Empty;
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public decimal Discount { get; private set; }
    public string? PictureUrl { get; private set; }

    public SaleItem() { }

    public SaleItem(string productId, string productName, decimal unitPrice, int quantity, decimal discount = 0, string? pictureUrl = null)
    {
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
        Discount = discount;
        PictureUrl = pictureUrl;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ProductId;
        yield return ProductName;
        yield return UnitPrice;
        yield return Quantity;
        yield return Discount;
        if (PictureUrl != null)
            yield return PictureUrl;
    }
} 
