namespace eShop.POS.Domain.AggregatesModel.BuyerAggregate;

public sealed class CardType
{
    public int Id { get; init; }
    public required string Name { get; init; }
}

// CardType is generic and can be reused for POS domain. Consider extending for POS-specific payment types in the future.
