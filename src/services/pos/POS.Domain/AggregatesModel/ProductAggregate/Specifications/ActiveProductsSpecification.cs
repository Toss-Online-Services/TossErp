namespace POS.Domain.AggregatesModel.ProductAggregate.Specifications;

/// <summary>
/// Specification for finding active products
/// </summary>
public class ActiveProductsSpecification : Specification<Product>
{
    public override Expression<Func<Product, bool>> ToExpression()
    {
        return product => product.IsActive;
    }
} 
