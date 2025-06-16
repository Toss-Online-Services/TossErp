using POS.Domain.SeedWork;

namespace POS.Domain.Common.ValueObjects;

public class TaxInformation : ValueObject
{
    public string TaxNumber { get; private set; }
    public string TaxType { get; private set; }
    public decimal TaxRate { get; private set; }

    public TaxInformation(string taxNumber, string taxType, decimal taxRate)
    {
        if (string.IsNullOrWhiteSpace(taxNumber))
            throw new ArgumentException("Tax number cannot be empty", nameof(taxNumber));
        if (string.IsNullOrWhiteSpace(taxType))
            throw new ArgumentException("Tax type cannot be empty", nameof(taxType));
        if (taxRate < 0 || taxRate > 100)
            throw new ArgumentException("Tax rate must be between 0 and 100", nameof(taxRate));

        TaxNumber = taxNumber;
        TaxType = taxType;
        TaxRate = taxRate;
    }

    public decimal CalculateTaxAmount(decimal amount)
    {
        return amount * (TaxRate / 100);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return TaxNumber;
        yield return TaxType;
        yield return TaxRate;
    }

    public override string ToString()
    {
        return $"{TaxType} ({TaxRate}%): {TaxNumber}";
    }
} 
