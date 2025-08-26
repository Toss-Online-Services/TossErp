using TossErp.CRM.Domain.SeedWork;
using System.ComponentModel.DataAnnotations.Schema;

namespace TossErp.CRM.Domain.ValueObjects;

/// <summary>
/// Represents the value and probability of an opportunity
/// Encapsulates estimated value, probability, and calculated weighted value
/// </summary>
[ComplexType]
public class OpportunityValue : ValueObject
{
    public Money EstimatedValue { get; private set; }
    public decimal Probability { get; private set; } // Percentage (0-100)

    // Calculated properties
    [NotMapped]
    public Money WeightedValue => new Money(EstimatedValue.Amount * (Probability / 100), EstimatedValue.Currency);

    [NotMapped]
    public bool IsHighValue => EstimatedValue.Amount >= 50000;

    [NotMapped]
    public bool IsLowProbability => Probability < 25;

    [NotMapped]
    public bool IsHighProbability => Probability >= 75;

    private OpportunityValue() { } // EF Core

    public OpportunityValue(Money estimatedValue, decimal probability)
    {
        EstimatedValue = estimatedValue ?? throw new ArgumentNullException(nameof(estimatedValue));
        
        if (probability < 0 || probability > 100)
            throw new ArgumentOutOfRangeException(nameof(probability), "Probability must be between 0 and 100");
        
        Probability = probability;
    }

    public static OpportunityValue Create(decimal amount, string currency, decimal probability)
    {
        return new OpportunityValue(new Money(amount, currency), probability);
    }

    public OpportunityValue UpdateProbability(decimal newProbability)
    {
        if (newProbability < 0 || newProbability > 100)
            throw new ArgumentOutOfRangeException(nameof(newProbability), "Probability must be between 0 and 100");

        return new OpportunityValue(EstimatedValue, newProbability);
    }

    public OpportunityValue UpdateValue(Money newValue)
    {
        return new OpportunityValue(newValue ?? throw new ArgumentNullException(nameof(newValue)), Probability);
    }

    public OpportunityValue UpdateBoth(Money newValue, decimal newProbability)
    {
        if (newProbability < 0 || newProbability > 100)
            throw new ArgumentOutOfRangeException(nameof(newProbability), "Probability must be between 0 and 100");

        return new OpportunityValue(newValue ?? throw new ArgumentNullException(nameof(newValue)), newProbability);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return EstimatedValue;
        yield return Probability;
    }

    public override string ToString()
    {
        return $"{EstimatedValue} ({Probability:F1}% probability, weighted: {WeightedValue})";
    }

    // Standard probability mappings for different opportunity stages
    public static class StandardProbabilities
    {
        public const decimal Prospecting = 10;
        public const decimal Qualification = 25;
        public const decimal NeedsAnalysis = 40;
        public const decimal Proposal = 60;
        public const decimal Negotiation = 80;
        public const decimal ClosedWon = 100;
        public const decimal ClosedLost = 0;
    }

    // Value tier classifications
    public enum ValueTier
    {
        Small,      // < $10K
        Medium,     // $10K - $50K
        Large,      // $50K - $250K
        Enterprise  // > $250K
    }

    [NotMapped]
    public ValueTier Tier
    {
        get
        {
            var amount = EstimatedValue.Amount;
            return amount switch
            {
                < 10000 => ValueTier.Small,
                < 50000 => ValueTier.Medium,
                < 250000 => ValueTier.Large,
                _ => ValueTier.Enterprise
            };
        }
    }

    // Risk assessment based on value and probability
    [NotMapped]
    public string RiskLevel
    {
        get
        {
            if (IsHighValue && IsLowProbability)
                return "High Risk/High Reward";
            if (IsHighValue && IsHighProbability)
                return "Low Risk/High Reward";
            if (!IsHighValue && IsLowProbability)
                return "High Risk/Low Reward";
            return "Moderate Risk/Moderate Reward";
        }
    }

    // Forecasting helpers
    public Money GetConservativeValue() => new Money(EstimatedValue.Amount * 0.8m, EstimatedValue.Currency);
    public Money GetOptimisticValue() => new Money(EstimatedValue.Amount * 1.2m, EstimatedValue.Currency);
    
    // Value formatting for different scenarios
    public string ToShortString() => $"{EstimatedValue.ToShortString()} ({Probability:F0}%)";
    public string ToDetailedString() => $"{EstimatedValue} at {Probability:F1}% probability (weighted: {WeightedValue})";
}
