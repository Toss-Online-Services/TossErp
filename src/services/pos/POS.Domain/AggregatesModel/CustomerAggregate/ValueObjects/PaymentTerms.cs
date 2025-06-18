using POS.Domain.Common;

namespace POS.Domain.AggregatesModel.CustomerAggregate.ValueObjects;

public record PaymentTerms
{
    public int Days { get; }
    public string Description { get; }

    public PaymentTerms(int days, string description)
    {
        if (days < 0)
            throw new DomainException("Payment terms days cannot be negative");
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Payment terms description cannot be empty");

        Days = days;
        Description = description;
    }

    public static PaymentTerms Create(int days, string description) => 
        new(days, description);

    public static PaymentTerms Net30 => 
        new(30, "Net 30");

    public static PaymentTerms Net60 => 
        new(60, "Net 60");

    public static PaymentTerms Net90 => 
        new(90, "Net 90");

    public bool IsOverdue(DateTime purchaseDate) => 
        (DateTime.UtcNow - purchaseDate).TotalDays > Days;
} 
