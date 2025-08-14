namespace TossErp.Stock.Domain.Enums;

public enum MovementType
{
    Receipt,    // Stock received from supplier
    Issue,      // Stock issued for sale/production
    Adjustment, // Manual stock adjustment
    Transfer,   // Transfer between warehouses
    Return,     // Customer return
    Damage,     // Damaged stock write-off
    Expiry      // Expired stock write-off
}
