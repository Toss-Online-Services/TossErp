namespace Toss.Domain.Entities.Orders;

/// <summary>
/// Represents an order (extends beyond simple sales for more complex scenarios)
/// </summary>
public class Order : BaseAuditableEntity
{
    public Order()
    {
        OrderGuid = Guid.NewGuid();
        OrderStatus = OrderStatus.Pending;
        PaymentStatus = Domain.Enums.PaymentStatus.Pending;
        ShippingStatus = ShippingStatus.NotYetShipped;
        CustomerTaxDisplayType = TaxDisplayType.IncludingTax;
        OrderItems = new List<OrderItem>();
        OrderNotes = new List<OrderNote>();
    }

    /// <summary>
    /// Gets or sets the order GUID
    /// </summary>
    public Guid OrderGuid { get; set; }

    /// <summary>
    /// Gets or sets the customer ID
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the billing address ID
    /// </summary>
    public int? BillingAddressId { get; set; }
    public Address? BillingAddress { get; set; }

    /// <summary>
    /// Gets or sets the shipping address ID
    /// </summary>
    public int? ShippingAddressId { get; set; }
    public Address? ShippingAddress { get; set; }

    /// <summary>
    /// Gets or sets the order status
    /// </summary>
    public OrderStatus OrderStatus { get; set; }

    /// <summary>
    /// Gets or sets the shipping status
    /// </summary>
    public ShippingStatus ShippingStatus { get; set; }

    /// <summary>
    /// Gets or sets the payment status
    /// </summary>
    public Domain.Enums.PaymentStatus PaymentStatus { get; set; }

    /// <summary>
    /// Gets or sets the payment method system name
    /// </summary>
    public string? PaymentMethodSystemName { get; set; }

    /// <summary>
    /// Gets or sets the customer tax display type
    /// </summary>
    public TaxDisplayType CustomerTaxDisplayType { get; set; }

    /// <summary>
    /// Gets or sets the customer IP address
    /// </summary>
    public string? CustomerIp { get; set; }

    /// <summary>
    /// Gets or sets the order subtotal (excl tax)
    /// </summary>
    public decimal OrderSubtotalExclTax { get; set; }

    /// <summary>
    /// Gets or sets the order subtotal (incl tax)
    /// </summary>
    public decimal OrderSubtotalInclTax { get; set; }

    /// <summary>
    /// Gets or sets the order tax
    /// </summary>
    public decimal OrderTax { get; set; }

    /// <summary>
    /// Gets or sets the order total
    /// </summary>
    public decimal OrderTotal { get; set; }

    /// <summary>
    /// Gets or sets the refunded amount
    /// </summary>
    public decimal RefundedAmount { get; set; }

    /// <summary>
    /// Gets or sets the paid date and time
    /// </summary>
    public DateTime? PaidDateUtc { get; set; }

    /// <summary>
    /// Gets or sets the shipping method
    /// </summary>
    public string? ShippingMethod { get; set; }

    /// <summary>
    /// Gets or sets the customer currency code
    /// </summary>
    public string? CustomerCurrencyCode { get; set; }

    /// <summary>
    /// Gets or sets the customer language ID
    /// </summary>
    public int? CustomerLanguageId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity has been deleted
    /// </summary>
    public bool Deleted { get; set; }

    /// <summary>
    /// Gets or sets the order items
    /// </summary>
    public ICollection<OrderItem> OrderItems { get; set; }

    /// <summary>
    /// Gets or sets the order notes
    /// </summary>
    public ICollection<OrderNote> OrderNotes { get; set; }
}

