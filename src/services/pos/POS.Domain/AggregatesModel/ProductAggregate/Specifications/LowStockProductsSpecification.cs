namespace POS.Domain.AggregatesModel.ProductAggregate.Specifications;

/// <summary>
/// Specification for finding products with low stock
/// </summary>
public class LowStockProductsSpecification : Specification<Product>
{
    private readonly int _threshold;

    public LowStockProductsSpecification(int threshold = 10)
    {
        _threshold = Guard.Against.NegativeOrZero(threshold, nameof(threshold));
    }

    public override Expression<Func<Product, bool>> ToExpression()
    {
        return product => product.StockQuantity <= _threshold && product.IsActive;
    }
} 
