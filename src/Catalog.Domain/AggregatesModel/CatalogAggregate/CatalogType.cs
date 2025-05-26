using Catalog.Domain.SeedWork;

namespace Catalog.Domain.AggregatesModel.CatalogAggregate;

public class CatalogType : Entity
{
    public string Type { get; private set; }

    protected CatalogType() { }

    public CatalogType(string type)
    {
        Type = type;
    }

    public void UpdateType(string type)
    {
        Type = type;
    }
} 
