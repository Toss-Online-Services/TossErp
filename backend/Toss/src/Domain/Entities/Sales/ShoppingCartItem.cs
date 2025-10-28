using Toss.Domain.Common;

namespace Toss.Domain.Entities.Sales;

/// <summary>
/// Represents an item in a shopping cart (POS session)
/// </summary>
public class ShoppingCartItem : BaseEntity
{
    /// <summary>
    /// Gets or sets the shop/store ID where this cart belongs
    /// </summary>
    public int ShopId { get; set; }

    /// <summary>
    /// Gets or sets the customer ID (null for walk-in customers at POS)
    /// </summary>
    public int? CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the product ID
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the quantity
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the price at the time of adding to cart (snapshot for price changes)
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets or sets the tax rate percentage
    /// </summary>
    public decimal TaxRate { get; set; }

    /// <summary>
    /// Gets or sets the discount amount applied to this item
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// Gets or sets the session ID for POS (to group items before checkout)
    /// </summary>
    public string SessionId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether this cart is active (not checked out yet)
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the attributes (e.g., size, color) as JSON
    /// </summary>
    public string? Attributes { get; set; }

    // Navigation properties
    public virtual Store? Shop { get; set; }
    public virtual Customer? Customer { get; set; }
    public virtual Product? Product { get; set; }

    /// <summary>
    /// Calculates the total price for this cart item
    /// </summary>
    public decimal GetTotal() => (UnitPrice * Quantity) - DiscountAmount + ((UnitPrice * Quantity - DiscountAmount) * TaxRate / 100);
}

