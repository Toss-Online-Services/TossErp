namespace TossErp.Stock.Application.Common.Interfaces;

/// <summary>
/// Service for tax calculations
/// </summary>
public interface ITaxCalculationService
{
    /// <summary>
    /// Calculates tax for a given amount and tax rate
    /// </summary>
    /// <param name="amount">The amount to calculate tax for</param>
    /// <param name="taxRate">The tax rate as a percentage (e.g., 10.0 for 10%)</param>
    /// <returns>Tax calculation result</returns>
    Task<TaxCalculationResult> CalculateTaxAsync(decimal amount, decimal taxRate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculates tax for a given amount based on location and tax category
    /// </summary>
    /// <param name="amount">The amount to calculate tax for</param>
    /// <param name="location">The location for tax calculation</param>
    /// <param name="taxCategory">The tax category</param>
    /// <returns>Tax calculation result</returns>
    Task<TaxCalculationResult> CalculateTaxByLocationAsync(decimal amount, string location, string taxCategory, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets applicable tax rates for a location and tax category
    /// </summary>
    /// <param name="location">The location</param>
    /// <param name="taxCategory">The tax category</param>
    /// <returns>List of applicable tax rates</returns>
    Task<IEnumerable<TaxRate>> GetTaxRatesAsync(string location, string taxCategory, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a tax rate
    /// </summary>
    /// <param name="taxRate">The tax rate to validate</param>
    /// <returns>True if the tax rate is valid</returns>
    Task<bool> ValidateTaxRateAsync(decimal taxRate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets tax categories available for a location
    /// </summary>
    /// <param name="location">The location</param>
    /// <returns>List of available tax categories</returns>
    Task<IEnumerable<string>> GetTaxCategoriesAsync(string location, CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of a tax calculation
/// </summary>
public record TaxCalculationResult
{
    /// <summary>
    /// The original amount
    /// </summary>
    public decimal OriginalAmount { get; init; }

    /// <summary>
    /// The tax amount
    /// </summary>
    public decimal TaxAmount { get; init; }

    /// <summary>
    /// The total amount including tax
    /// </summary>
    public decimal TotalAmount { get; init; }

    /// <summary>
    /// The tax rate used
    /// </summary>
    public decimal TaxRate { get; init; }

    /// <summary>
    /// The location used for calculation
    /// </summary>
    public string? Location { get; init; }

    /// <summary>
    /// The tax category used
    /// </summary>
    public string? TaxCategory { get; init; }

    /// <summary>
    /// Currency code
    /// </summary>
    public string Currency { get; init; } = "USD";

    /// <summary>
    /// Whether the calculation was successful
    /// </summary>
    public bool IsValid { get; init; }

    /// <summary>
    /// Error message if calculation failed
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Additional metadata from the calculation
    /// </summary>
    public Dictionary<string, string> Metadata { get; init; } = new();
}

/// <summary>
/// Tax rate information
/// </summary>
public record TaxRate
{
    /// <summary>
    /// The tax rate percentage
    /// </summary>
    public decimal Rate { get; init; }

    /// <summary>
    /// The tax category
    /// </summary>
    public string Category { get; init; } = string.Empty;

    /// <summary>
    /// The location this rate applies to
    /// </summary>
    public string Location { get; init; } = string.Empty;

    /// <summary>
    /// Description of the tax rate
    /// </summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Whether this tax rate is active
    /// </summary>
    public bool IsActive { get; init; }

    /// <summary>
    /// Effective date from which this rate applies
    /// </summary>
    public DateTime EffectiveFrom { get; init; }

    /// <summary>
    /// Effective date until which this rate applies (null if indefinite)
    /// </summary>
    public DateTime? EffectiveTo { get; init; }
}
