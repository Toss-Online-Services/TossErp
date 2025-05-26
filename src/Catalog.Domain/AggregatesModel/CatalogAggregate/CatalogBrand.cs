using Catalog.Domain.SeedWork;

namespace Catalog.Domain.AggregatesModel.CatalogAggregate;

public class CatalogBrand : Entity
{
    public string Brand { get; private set; }

    protected CatalogBrand() { }

    public CatalogBrand(string brand)
    {
        Brand = brand;
    }

    public void UpdateBrand(string brand)
    {
        Brand = brand;
    }
} 
