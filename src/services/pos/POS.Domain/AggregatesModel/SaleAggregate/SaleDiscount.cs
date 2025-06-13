#nullable enable
namespace eShop.POS.Domain.AggregatesModel.SaleAggregate;
public class SaleDiscount : ValueObject
{
    public decimal Amount { get; private set; }
    public string Reason { get; private set; }
    public DiscountType Type { get; private set; }
    public string StaffId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    protected SaleDiscount() {
        Reason = string.Empty;
        StaffId = string.Empty;
    }

    public SaleDiscount(decimal amount, string reason, DiscountType type, string staffId)
    {
        if (amount <= 0)
            throw new POSDomainException("Discount amount must be greater than zero.");
        if (string.IsNullOrEmpty(reason))
            throw new POSDomainException("Discount reason is required.");

        Amount = amount;
        Reason = reason;
        Type = type;
        StaffId = staffId;
        CreatedAt = DateTime.UtcNow;
    }

    public void SetStaffId(string staffId)
    {
        StaffId = staffId;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Reason;
        yield return Type;
        yield return StaffId;
        yield return CreatedAt;
    }
} 
