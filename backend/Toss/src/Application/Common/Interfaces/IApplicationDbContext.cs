using Toss.Domain.Entities;
using Toss.Domain.Entities.Buying;
using Toss.Domain.Entities.CRM;
using Toss.Domain.Entities.GroupBuying;
using Toss.Domain.Entities.Inventory;
using Toss.Domain.Entities.Logistics;
using Toss.Domain.Entities.Payments;
using Toss.Domain.Entities.Sales;

namespace Toss.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    // Core entities
    DbSet<Shop> Shops { get; }
    DbSet<Address> Addresses { get; }

    // Inventory entities
    DbSet<Product> Products { get; }
    DbSet<ProductCategory> ProductCategories { get; }
    DbSet<StockLevel> StockLevels { get; }
    DbSet<StockMovement> StockMovements { get; }
    DbSet<StockAlert> StockAlerts { get; }

    // Sales entities
    DbSet<Sale> Sales { get; }
    DbSet<SaleItem> SaleItems { get; }
    DbSet<Receipt> Receipts { get; }
    DbSet<Invoice> Invoices { get; }

    // Supplier entities
    DbSet<Supplier> Suppliers { get; }
    DbSet<SupplierProduct> SupplierProducts { get; }
    DbSet<SupplierPricing> SupplierPricings { get; }

    // Buying entities
    DbSet<PurchaseOrder> PurchaseOrders { get; }
    DbSet<PurchaseOrderItem> PurchaseOrderItems { get; }
    DbSet<PurchaseReceipt> PurchaseReceipts { get; }

    // Group Buying entities
    DbSet<GroupBuyPool> GroupBuyPools { get; }
    DbSet<PoolParticipation> PoolParticipations { get; }
    DbSet<AggregatedPurchaseOrder> AggregatedPurchaseOrders { get; }

    // Logistics entities
    DbSet<Driver> Drivers { get; }
    DbSet<SharedDeliveryRun> SharedDeliveryRuns { get; }
    DbSet<DeliveryStop> DeliveryStops { get; }
    DbSet<ProofOfDelivery> ProofOfDeliveries { get; }

    // CRM entities
    DbSet<Customer> Customers { get; }
    DbSet<CustomerPurchase> CustomerPurchases { get; }
    DbSet<CustomerInteraction> CustomerInteractions { get; }

    // Payment entities
    DbSet<Payment> Payments { get; }
    DbSet<PayLink> PayLinks { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
