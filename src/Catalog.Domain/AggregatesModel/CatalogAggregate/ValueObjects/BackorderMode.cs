namespace Catalog.Domain.AggregatesModel.CatalogAggregate.ValueObjects;

public enum BackorderMode
{
    NoBackorders = 0,
    AllowQtyBelow0 = 1,
    AllowQtyBelow0AndNotifyCustomer = 2
} 
