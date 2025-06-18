using POS.Domain.SeedWork;
using POS.Domain.ValueObjects;

namespace POS.Domain.AggregatesModel.OrderAggregate;

/// <summary>
/// Represents an item in an order
/// </summary>
public class OrderItem : Entity
{
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; }
    public string ProductSku { get; private set; }
    public int Quantity { get; private set; }
    public Money UnitPrice { get; private set; }
    public Money TotalPrice { get; private set; }
    public string? Notes { get; private set; }

    private OrderItem() { ProductName = string.Empty; ProductSku = string.Empty; UnitPrice = new Money(0, "USD"); TotalPrice = new Money(0, "USD"); }

    public OrderItem(Guid productId, string productName, string productSku, int quantity, Money unitPrice, string? notes = null)
    {
        ProductId = Guard.Against.Default(productId, nameof(productId));
        ProductName = Guard.Against.NullOrWhiteSpace(productName, nameof(productName));
        ProductSku = Guard.Against.NullOrWhiteSpace(productSku, nameof(productSku));
        Quantity = Guard.Against.NegativeOrZero(quantity, nameof(quantity));
        UnitPrice = Guard.Against.Null(unitPrice, nameof(unitPrice));
        Notes = notes;
        TotalPrice = unitPrice * quantity;
    }

    public void UpdateQuantity(int newQuantity)
    {
        Guard.Against.NegativeOrZero(newQuantity, nameof(newQuantity));
        Quantity = newQuantity;
        TotalPrice = UnitPrice * newQuantity;
    }

    public void UpdateNotes(string? notes)
    {
        Notes = notes;
    }
} 
