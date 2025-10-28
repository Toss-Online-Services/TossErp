using System.Reflection;
using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities;
using Toss.Domain.Entities.ArtificialIntelligence;
using Toss.Domain.Entities.Orders;
using Toss.Domain.Entities.Catalog;
using Toss.Domain.Entities.CRM;
using Toss.Domain.Entities.Directory;
using Toss.Domain.Entities.GroupBuying;
using Toss.Domain.Entities.Localization;
using Toss.Domain.Entities.Logistics;
using Toss.Domain.Entities.Payments;
using Toss.Domain.Entities.Sales;
using Toss.Domain.Entities.Security;
using Toss.Domain.Entities.Shipping;
using Toss.Domain.Entities.Stores;
using Toss.Domain.Entities.Vendors;
using Toss.Domain.Entities.Tax;
using Toss.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Toss.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    // Core entities
    public DbSet<Store> Shops => Set<Store>();
    public DbSet<Store> Stores => Set<Store>(); // Alias for Shops
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
    public DbSet<ShoppingCartItem> ShoppingCartItems => Set<ShoppingCartItem>();

    // Supplier entities (removed - now using Vendors)

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

    // AI entities
    public DbSet<AISettings> AISettings => Set<AISettings>();
    public DbSet<AIConversation> AIConversations => Set<AIConversation>();
    public DbSet<AIMessage> AIMessages => Set<AIMessage>();

    // Localization entities
    public DbSet<Language> Languages => Set<Language>();
    public DbSet<LocaleStringResource> LocaleStringResources => Set<LocaleStringResource>();
    public DbSet<LocalizedProperty> LocalizedProperties => Set<LocalizedProperty>();

    // Directory entities
    public DbSet<Country> Countries => Set<Country>();
    public DbSet<StateProvince> StateProvinces => Set<StateProvince>();
    public DbSet<Currency> Currencies => Set<Currency>();
    public DbSet<MeasureWeight> MeasureWeights => Set<MeasureWeight>();
    public DbSet<MeasureDimension> MeasureDimensions => Set<MeasureDimension>();

    // Tax entities
    public DbSet<TaxCategory> TaxCategories => Set<TaxCategory>();
    public DbSet<TaxRate> TaxRates => Set<TaxRate>();

    // Security entities
    public DbSet<PermissionRecord> PermissionRecords => Set<PermissionRecord>();
    public DbSet<PermissionRoleMapping> PermissionRoleMappings => Set<PermissionRoleMapping>();
    public DbSet<AclRecord> AclRecords => Set<AclRecord>();

    // Store entities (Shop is already defined above as core entity)
    public DbSet<StoreMapping> StoreMappings => Set<StoreMapping>();

    // Catalog entities
    public DbSet<ProductAttribute> ProductAttributes => Set<ProductAttribute>();
    public DbSet<ProductAttributeValue> ProductAttributeValues => Set<ProductAttributeValue>();
    public DbSet<ProductReview> ProductReviews => Set<ProductReview>();
    public DbSet<ProductTag> ProductTags => Set<ProductTag>();
    public DbSet<ProductProductTagMapping> ProductProductTagMappings => Set<ProductProductTagMapping>();

    // Vendor entities (merged from Suppliers)
    public DbSet<Vendor> Vendors => Set<Vendor>();
    public DbSet<VendorNote> VendorNotes => Set<VendorNote>();
    public DbSet<VendorProduct> VendorProducts => Set<VendorProduct>();
    public DbSet<VendorPricing> VendorPricings => Set<VendorPricing>();

    // Order entities
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<OrderNote> OrderNotes => Set<OrderNote>();

    // Shipping entities
    public DbSet<ShippingMethod> ShippingMethods => Set<ShippingMethod>();
    public DbSet<Shipment> Shipments => Set<Shipment>();
    public DbSet<ShipmentItem> ShipmentItems => Set<ShipmentItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
