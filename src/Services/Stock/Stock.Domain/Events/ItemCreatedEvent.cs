using TossErp.Stock.Domain.SeedWork;

namespace TossErp.Stock.Domain.Events;

public class ItemCreatedEvent : DomainEvent
{
    public Guid ItemId { get; }
    public Guid TenantId { get; }
    public string SKU { get; }
    public string Name { get; }
    public string Category { get; }

    public ItemCreatedEvent(Guid itemId, Guid tenantId, string sku, string name, string category)
    {
        ItemId = itemId;
        TenantId = tenantId;
        SKU = sku;
        Name = name;
        Category = category;
    }
}
