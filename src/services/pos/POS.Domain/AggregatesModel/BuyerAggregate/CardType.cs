using POS.Domain.Exceptions;
using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.BuyerAggregate;

public class CardType : AggregateRoot
{
    public string Name { get; private set; }
    public string Icon { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public string? StoreId { get; private set; }

    protected CardType()
    {
        Name = string.Empty;
        Icon = string.Empty;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        DomainEvents = new List<DomainEvent>();
    }

    public CardType(string name, string icon, string? storeId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Name cannot be empty");
        if (string.IsNullOrWhiteSpace(icon))
            throw new DomainException("Icon cannot be empty");

        Name = name;
        Icon = icon;
        StoreId = storeId;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        DomainEvents = new List<DomainEvent>();
    }

    public void Update(string name, string? icon = null, string? storeId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Name cannot be empty");

        Name = name;
        if (icon != null)
            Icon = icon;
        if (storeId != null)
            StoreId = storeId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new DomainException("Card type is already inactive");

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        if (IsActive)
            throw new DomainException("Card type is already active");

        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public static readonly CardType Visa = new("Visa", "/images/cards/visa.png") { DomainEvents = new List<DomainEvent>() };
    public static readonly CardType MasterCard = new("MasterCard", "/images/cards/mastercard.png") { DomainEvents = new List<DomainEvent>() };
    public static readonly CardType AmericanExpress = new("American Express", "/images/cards/amex.png") { DomainEvents = new List<DomainEvent>() };
    public static readonly CardType Discover = new("Discover", "/images/cards/discover.png") { DomainEvents = new List<DomainEvent>() };
}

// CardType is generic and can be reused for POS domain. Consider extending for POS-specific payment types in the future.
