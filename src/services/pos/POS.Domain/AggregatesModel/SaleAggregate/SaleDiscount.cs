using POS.Domain.SeedWork;
using POS.Domain.Enums;
using POS.Domain.Common;
using POS.Domain.Exceptions;

namespace POS.Domain.AggregatesModel.SaleAggregate;

public class SaleDiscount : Entity
{
    public string? Code { get; private set; }
    public string? StoreId { get; private set; }
    public string? Status { get; private set; }
    public DiscountType DiscountType { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string? Description { get; private set; }

    private SaleDiscount()
    {
        DiscountType = DiscountType.Percentage;
    }

    public SaleDiscount(string? code, string? storeId, POS.Domain.Enums.DiscountType type, decimal amount, DateTime startDate, DateTime endDate)
    {
        Code = code;
        StoreId = storeId;
        DiscountType = type;
        Amount = amount;
        StartDate = startDate;
        EndDate = endDate;
        Status = "Active";
        CreatedAt = DateTime.UtcNow;
    }

    public SaleDiscount(POS.Domain.Enums.DiscountType type, decimal amount, string? description = null)
    {
        if (amount <= 0)
            throw new DomainException("Discount amount must be greater than zero");

        DiscountType = type;
        Amount = amount;
        Description = description;
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

    public void UpdateAmount(decimal amount)
    {
        if (amount <= 0)
            throw new DomainException("Discount amount must be greater than zero");
        Amount = amount;
    }

    public void UpdateDescription(string? description)
    {
        Description = description;
    }
}
