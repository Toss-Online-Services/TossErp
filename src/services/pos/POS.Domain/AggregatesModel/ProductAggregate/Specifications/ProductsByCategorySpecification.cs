namespace POS.Domain.AggregatesModel.ProductAggregate.Specifications;

/// <summary>
/// Specification for finding products by category
/// </summary>
public class ProductsByCategorySpecification : Specification<Product>
{
    private readonly Guid _categoryId;

    public ProductsByCategorySpecification(Guid categoryId)
    {
        _categoryId = categoryId;
    }

    public override Expression<Func<Product, bool>> ToExpression()
    {
        return product => product.Category.Id == _categoryId;
    }
} 
