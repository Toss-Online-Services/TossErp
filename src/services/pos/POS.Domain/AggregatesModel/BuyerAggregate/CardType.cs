using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.BuyerAggregate;

public class CardType : Entity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    protected CardType()
    {
        Name = string.Empty;
        Description = string.Empty;
        CreatedAt = DateTime.UtcNow;
    }

    public CardType(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Name cannot be empty");
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Description cannot be empty");

        Name = name;
        Description = description;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Name cannot be empty");
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Description cannot be empty");

        Name = name;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public static readonly CardType Visa = new(nameof(Visa), "Visa card");
    public static readonly CardType MasterCard = new(nameof(MasterCard), "MasterCard");
    public static readonly CardType AmericanExpress = new(nameof(AmericanExpress), "American Express");
    public static readonly CardType Discover = new(nameof(Discover), "Discover");
}

// CardType is generic and can be reused for POS domain. Consider extending for POS-specific payment types in the future.
