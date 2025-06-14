namespace TossErp.POS.Domain.AggregatesModel.BuyerAggregate;

public class CardType
{
    public int Id { get; private set; }
    public string Name { get; private set; }

    protected CardType() { }

    public CardType(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public static readonly CardType Visa = new(1, nameof(Visa));
    public static readonly CardType MasterCard = new(2, nameof(MasterCard));
    public static readonly CardType AmericanExpress = new(3, nameof(AmericanExpress));
    public static readonly CardType Discover = new(4, nameof(Discover));
}

// CardType is generic and can be reused for POS domain. Consider extending for POS-specific payment types in the future.
