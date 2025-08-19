using System.Text.RegularExpressions;

namespace TossErp.Procurement.Domain.ValueObjects;

/// <summary>
/// Purchase order number value object with validation
/// </summary>
public sealed record PurchaseOrderNumber
{
    private static readonly Regex PurchaseOrderNumberPattern = new(@"^PO-\d{4}-\d{3}$", RegexOptions.Compiled);
    private static long _sequence = 0;
    
    public string Value { get; }

    public PurchaseOrderNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Purchase order number cannot be null or empty", nameof(value));

        if (!PurchaseOrderNumberPattern.IsMatch(value))
            throw new ArgumentException($"Invalid purchase order number format: {value}. Expected format: PO-YYYY-######", nameof(value));

        Value = value.ToUpperInvariant();
    }

    /// <summary>
    /// Generate a new purchase order number
    /// </summary>
    /// <param name="year">Year for the PO</param>
    /// <param name="sequenceNumber">Sequential number</param>
    /// <returns>New purchase order number</returns>
    public static PurchaseOrderNumber Generate(int year, long sequenceNumber)
    {
        if (year < 2000 || year > 2100)
            throw new ArgumentException("Year must be between 2000 and 2100", nameof(year));

        if (sequenceNumber <= 0)
            throw new ArgumentException("Sequence number must be positive", nameof(sequenceNumber));

        var purchaseOrderNumber = $"PO-{year}-{sequenceNumber:D3}";
        return new PurchaseOrderNumber(purchaseOrderNumber);
    }

    /// <summary>
    /// Generate a new purchase order number using current year and a default sequence.
    /// </summary>
    public static PurchaseOrderNumber Generate()
    {
        var next = Interlocked.Increment(ref _sequence);
        return Generate(DateTime.UtcNow.Year, next);
    }

    public static implicit operator string(PurchaseOrderNumber purchaseOrderNumber) => purchaseOrderNumber.Value;

    public override string ToString() => Value;
}
