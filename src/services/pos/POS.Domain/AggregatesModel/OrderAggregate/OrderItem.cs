using POS.Domain.ValueObjects;

namespace POS.Domain.AggregatesModel.OrderAggregate;

/// <summary>
/// Represents an item in an order
/// </summary>
public class OrderItem : Entity
{
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public Money UnitPrice { get; private set; }
    public Money TotalPrice { get; private set; }

    private OrderItem() { } // For EF Core

    public OrderItem(Guid productId, int quantity, Money unitPrice)
    {
        ProductId = Guard.Against.Default(productId, nameof(productId));
        Quantity = Guard.Against.NegativeOrZero(quantity, nameof(quantity));
        UnitPrice = Guard.Against.Null(unitPrice, nameof(unitPrice));
        CalculateTotalPrice();
    }

    public void UpdateQuantity(int quantity)
    {
        Quantity = Guard.Against.NegativeOrZero(quantity, nameof(quantity));
        CalculateTotalPrice();
    }

    public void UpdateUnitPrice(Money unitPrice)
    {
        UnitPrice = Guard.Against.Null(unitPrice, nameof(unitPrice));
        CalculateTotalPrice();
    }

    private void CalculateTotalPrice()
    {
        TotalPrice = Money.FromDecimal(UnitPrice.Amount * Quantity, UnitPrice.Currency);
    }
} 
