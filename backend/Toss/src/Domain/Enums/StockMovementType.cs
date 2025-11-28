namespace Toss.Domain.Enums;

public enum StockMovementType
{
    Sale = 0,
    Purchase = 1,
    Adjustment = 2,
    Return = 3,
    Transfer = 4,
    Damage = 5,
    Expiry = 6,
    Consume = 7,  // Manufacturing: consumption of raw materials
    Produce = 8   // Manufacturing: production of finished goods
}

