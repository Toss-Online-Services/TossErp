namespace Toss.Domain.Entities.Orders;

/// <summary>
/// Represents an order note
/// </summary>
public class OrderNote : BaseEntity
{
    public OrderNote()
    {
        Note = string.Empty;
        DisplayToCustomer = false;
        CreatedOnUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets or sets the order ID
    /// </summary>
    public int OrderId { get; set; }
    public Order? Order { get; set; }

    /// <summary>
    /// Gets or sets the note
    /// </summary>
    public string Note { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a customer can see a note
    /// </summary>
    public bool DisplayToCustomer { get; set; }

    /// <summary>
    /// Gets or sets the date and time of order note creation
    /// </summary>
    public DateTime CreatedOnUtc { get; set; }
}

