namespace TossErp.Accounting.Domain.Enums;

/// <summary>
/// Methods for valuing inventory
/// </summary>
public enum ValuationMethod
{
    /// <summary>
    /// First In, First Out - oldest inventory is sold first
    /// </summary>
    FIFO = 1,
    
    /// <summary>
    /// Last In, First Out - newest inventory is sold first
    /// </summary>
    LIFO = 2,
    
    /// <summary>
    /// Weighted Average Cost - average cost of all inventory
    /// </summary>
    WeightedAverage = 3,
    
    /// <summary>
    /// Specific Identification - each item is tracked individually
    /// </summary>
    SpecificIdentification = 4,
    
    /// <summary>
    /// Standard Cost - predetermined cost for inventory
    /// </summary>
    StandardCost = 5
}

