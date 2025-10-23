using System.Reflection;
using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities;
using Toss.Domain.Entities.Buying;
using Toss.Domain.Entities.CRM;
using Toss.Domain.Entities.GroupBuying;
using Toss.Domain.Entities.Inventory;
using Toss.Domain.Entities.Logistics;
using Toss.Domain.Entities.Payments;
using Toss.Domain.Entities.Sales;
using Toss.Domain.Entities.Suppliers;
using Toss.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Toss.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    // Core entities
    public DbSet<Shop> Shops => Set<Shop>();
    public DbSet<Address> Addresses => Set<Address>();

    // Inventory entities
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
    public DbSet<StockLevel> StockLevels => Set<StockLevel>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
    public DbSet<StockAlert> StockAlerts => Set<StockAlert>();

    // Sales entities
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();
    public DbSet<Receipt> Receipts => Set<Receipt>();
    public DbSet<Invoice> Invoices => Set<Invoice>();

    // Supplier entities
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<SupplierProduct> SupplierProducts => Set<SupplierProduct>();
    public DbSet<SupplierPricing> SupplierPricings => Set<SupplierPricing>();

    // Buying entities
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<PurchaseOrderItem> PurchaseOrderItems => Set<PurchaseOrderItem>();
    public DbSet<PurchaseReceipt> PurchaseReceipts => Set<PurchaseReceipt>();

    // Group Buying entities
    public DbSet<GroupBuyPool> GroupBuyPools => Set<GroupBuyPool>();
    public DbSet<PoolParticipation> PoolParticipations => Set<PoolParticipation>();
    public DbSet<AggregatedPurchaseOrder> AggregatedPurchaseOrders => Set<AggregatedPurchaseOrder>();

    // Logistics entities
    public DbSet<Driver> Drivers => Set<Driver>();
    public DbSet<SharedDeliveryRun> SharedDeliveryRuns => Set<SharedDeliveryRun>();
    public DbSet<DeliveryStop> DeliveryStops => Set<DeliveryStop>();
    public DbSet<ProofOfDelivery> ProofOfDeliveries => Set<ProofOfDelivery>();

    // CRM entities
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<CustomerPurchase> CustomerPurchases => Set<CustomerPurchase>();
    public DbSet<CustomerInteraction> CustomerInteractions => Set<CustomerInteraction>();

    // Payment entities
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<PayLink> PayLinks => Set<PayLink>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
