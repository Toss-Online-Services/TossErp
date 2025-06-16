using POS.Domain.SeedWork;
using POS.Domain.Enums;

namespace POS.Domain.AggregatesModel.SaleAggregate;

public class SaleDiscount : Entity
{
    public string? Code { get; private set; }
    public string? StoreId { get; private set; }
    public string? Status { get; private set; }
    public DiscountType Type { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private SaleDiscount() { }

    public SaleDiscount(string? code, string? storeId, DiscountType type, decimal amount, DateTime startDate, DateTime endDate)
    {
        Code = code;
        StoreId = storeId;
        Type = type;
        Amount = amount;
        StartDate = startDate;
        EndDate = endDate;
        Status = "Active";
        CreatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        Status = "Inactive";
    }

    public bool IsValid()
    {
        var now = DateTime.UtcNow;
        return Status == "Active" && now >= StartDate && now <= EndDate;
    }
}
