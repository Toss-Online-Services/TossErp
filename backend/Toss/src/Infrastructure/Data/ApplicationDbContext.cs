using System.Reflection;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities;
using Toss.Domain.Entities.Accounting;
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
using Toss.Domain.Entities.Onboarding;
using Toss.Domain.Entities.Manufacturing;
using Toss.Infrastructure.Identity;
using Toss.Domain.Entities.Businesses;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Toss.Infrastructure.Data;

/// <summary>
/// The main database context for the TOSS ERP application.
/// Inherits from IdentityDbContext to support ASP.NET Core Identity.
/// Implements IApplicationDbContext for clean architecture and testability.
/// </summary>
/// <remarks>
/// This context manages all entity sets for the application including:
/// - Core entities (Stores, Addresses)
/// - Inventory and catalog management
/// - Sales and purchasing
/// - Logistics and shipping
/// - CRM and customer data
/// - Security and localization
/// - AI and automation features
/// </remarks>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    private readonly IBusinessContext _businessContext;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IBusinessContext businessContext) : base(options)
    {
        _businessContext = businessContext;
    }

    /// <summary>
    /// Applies global EF Core conventions for this context.
    /// Currently enforces decimal precision suitable for money fields across the schema.
    /// </summary>
    /// <param name="configurationBuilder">The convention builder.</param>
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<decimal>()
            .HavePrecision(18, 2);

        configurationBuilder
            .Properties<decimal?>()
            .HavePrecision(18, 2);
    }

    // Core entities
    /// <summary>
    /// Stores/Shops in the system. Each store represents a physical or virtual shop location.
    /// </summary>
    public DbSet<Store> Stores => Set<Store>();
    
    /// <summary>
    /// Physical and billing addresses used throughout the system.
    /// </summary>
    public DbSet<Address> Addresses => Set<Address>();

    // Inventory entities
    /// <summary>
    /// Products available for sale in the system.
    /// </summary>
    public DbSet<Product> Products => Set<Product>();
    
    /// <summary>
    /// Hierarchical categories for organizing products.
    /// </summary>
    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
    
    /// <summary>
    /// Current stock levels for products across different stores/warehouses.
    /// </summary>
    public DbSet<StockLevel> StockLevels => Set<StockLevel>();
    
    /// <summary>
    /// Historical record of all stock movements (receipts, transfers, adjustments).
    /// </summary>
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
    
    /// <summary>
    /// Alerts for low stock levels or other inventory-related notifications.
    /// </summary>
    public DbSet<StockAlert> StockAlerts => Set<StockAlert>();

    // Sales entities
    /// <summary>
    /// Completed sales transactions in the POS system.
    /// </summary>
    public DbSet<Sale> Sales => Set<Sale>();
    
    /// <summary>
    /// Individual line items for each sale.
    /// </summary>
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();
    
    /// <summary>
    /// Sales-related documents (receipts, invoices, etc.).
    /// </summary>
    public DbSet<SalesDocument> SalesDocuments => Set<SalesDocument>();
    
    /// <summary>
    /// Shopping cart items for online orders before checkout.
    /// </summary>
    public DbSet<ShoppingCartItem> ShoppingCartItems => Set<ShoppingCartItem>();

    // Supplier entities (removed - now using Vendors)

    // Purchasing entities
    /// <summary>
    /// Purchase orders sent to vendors for restocking inventory.
    /// </summary>
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    
    /// <summary>
    /// Individual line items for each purchase order.
    /// </summary>
    public DbSet<PurchaseOrderItem> PurchaseOrderItems => Set<PurchaseOrderItem>();
    
    /// <summary>
    /// Receipts documenting goods received from purchase orders.
    /// </summary>
    public DbSet<PurchaseReceipt> PurchaseReceipts => Set<PurchaseReceipt>();
    
    /// <summary>
    /// Purchase-related documents (invoices, delivery notes, etc.).
    /// </summary>
    public DbSet<PurchaseDocument> PurchaseDocuments => Set<PurchaseDocument>();
    
    /// <summary>
    /// Lines for purchase documents (supplier invoices).
    /// </summary>
    public DbSet<PurchaseDocumentLine> PurchaseDocumentLines => Set<PurchaseDocumentLine>();
    
    /// <summary>
    /// Purchase requests that can be converted to purchase orders.
    /// </summary>
    public DbSet<PurchaseRequest> PurchaseRequests => Set<PurchaseRequest>();
    
    /// <summary>
    /// Individual line items for each purchase request.
    /// </summary>
    public DbSet<PurchaseRequestLine> PurchaseRequestLines => Set<PurchaseRequestLine>();

    // Group Buying entities
    /// <summary>
    /// Pools for group buying initiatives where multiple customers combine orders.
    /// </summary>
    public DbSet<GroupBuyPool> GroupBuyPools => Set<GroupBuyPool>();
    
    /// <summary>
    /// Individual customer participation in group buying pools.
    /// </summary>
    public DbSet<PoolParticipation> PoolParticipations => Set<PoolParticipation>();
    
    /// <summary>
    /// Aggregated purchase orders created from group buying pools.
    /// </summary>
    public DbSet<AggregatedPurchaseOrder> AggregatedPurchaseOrders => Set<AggregatedPurchaseOrder>();

    // Logistics entities
    /// <summary>
    /// Drivers available for delivery services.
    /// </summary>
    public DbSet<Driver> Drivers => Set<Driver>();
    
    /// <summary>
    /// Shared delivery runs combining multiple deliveries for efficiency.
    /// </summary>
    public DbSet<SharedDeliveryRun> SharedDeliveryRuns => Set<SharedDeliveryRun>();
    
    /// <summary>
    /// Individual stops on a delivery run.
    /// </summary>
    public DbSet<DeliveryStop> DeliveryStops => Set<DeliveryStop>();
    
    /// <summary>
    /// Proof of delivery documentation (signatures, photos, etc.).
    /// </summary>
    public DbSet<ProofOfDelivery> ProofOfDeliveries => Set<ProofOfDelivery>();

    // CRM entities
    /// <summary>
    /// Customer master data for CRM and sales tracking.
    /// </summary>
    public DbSet<Customer> Customers => Set<Customer>();
    
    /// <summary>
    /// Historical record of customer purchases for analytics and recommendations.
    /// </summary>
    public DbSet<CustomerPurchase> CustomerPurchases => Set<CustomerPurchase>();
    
    /// <summary>
    /// Customer interactions (calls, emails, meetings) for relationship management.
    /// </summary>
    public DbSet<CustomerInteraction> CustomerInteractions => Set<CustomerInteraction>();

    // Payment entities
    /// <summary>
    /// Payment transactions processed through various payment methods.
    /// </summary>
    public DbSet<Payment> Payments => Set<Payment>();
    
    /// <summary>
    /// Payment links for online and remote payment collection.
    /// </summary>
    public DbSet<PayLink> PayLinks => Set<PayLink>();

    // Accounting entities
    /// <summary>
    /// Accounting accounts (Cash or Bank) for tracking money in/out.
    /// </summary>
    public DbSet<Account> Accounts => Set<Account>();
    
    /// <summary>
    /// Cashbook entries recording money in or out of accounts.
    /// </summary>
    public DbSet<CashbookEntry> CashbookEntries => Set<CashbookEntry>();
    
    /// <summary>
    /// Accounts payable ledger entries for vendors.
    /// </summary>
    public DbSet<VendorLedgerEntry> VendorLedgerEntries => Set<VendorLedgerEntry>();

    // AI entities
    /// <summary>
    /// AI system settings and configuration for artificial intelligence features.
    /// </summary>
    public DbSet<AISettings> AISettings => Set<AISettings>();
    
    /// <summary>
    /// AI conversation threads for customer support and assistance.
    /// </summary>
    public DbSet<AIConversation> AIConversations => Set<AIConversation>();
    
    /// <summary>
    /// Individual messages within AI conversation threads.
    /// </summary>
    public DbSet<AIMessage> AIMessages => Set<AIMessage>();

    // Localization entities
    /// <summary>
    /// Supported languages in the system for multi-language support.
    /// </summary>
    public DbSet<Language> Languages => Set<Language>();
    
    /// <summary>
    /// Localized string resources for translations.
    /// </summary>
    public DbSet<LocaleStringResource> LocaleStringResources => Set<LocaleStringResource>();
    
    /// <summary>
    /// Localized property values for entity-level translations.
    /// </summary>
    public DbSet<LocalizedProperty> LocalizedProperties => Set<LocalizedProperty>();

    // Directory entities
    /// <summary>
    /// Countries supported in the system for address and tax purposes.
    /// </summary>
    public DbSet<Country> Countries => Set<Country>();
    
    /// <summary>
    /// States/provinces within countries for more granular location data.
    /// </summary>
    public DbSet<StateProvince> StateProvinces => Set<StateProvince>();
    
    /// <summary>
    /// Currencies supported for multi-currency transactions.
    /// </summary>
    public DbSet<Currency> Currencies => Set<Currency>();
    
    /// <summary>
    /// Weight measurement units (kg, lb, oz, etc.).
    /// </summary>
    public DbSet<MeasureWeight> MeasureWeights => Set<MeasureWeight>();
    
    /// <summary>
    /// Dimension measurement units (m, cm, in, ft, etc.).
    /// </summary>
    public DbSet<MeasureDimension> MeasureDimensions => Set<MeasureDimension>();

    // Tax entities
    /// <summary>
    /// Tax categories for grouping products with similar tax rates.
    /// </summary>
    public DbSet<TaxCategory> TaxCategories => Set<TaxCategory>();
    
    /// <summary>
    /// Tax rates applied based on location, category, and other factors.
    /// </summary>
    public DbSet<TaxRate> TaxRates => Set<TaxRate>();

    // Security entities
    /// <summary>
    /// System permissions that can be assigned to roles.
    /// </summary>
    public DbSet<PermissionRecord> PermissionRecords => Set<PermissionRecord>();
    
    /// <summary>
    /// Mappings between permissions and roles for access control.
    /// </summary>
    public DbSet<PermissionRoleMapping> PermissionRoleMappings => Set<PermissionRoleMapping>();
    
    /// <summary>
    /// Access Control List records for entity-level security.
    /// </summary>
    public DbSet<AclRecord> AclRecords => Set<AclRecord>();

    // Onboarding entities
    /// <summary>
    /// Tracks onboarding completion status for different user roles (Retailer, Supplier, Driver).
    /// </summary>
    public DbSet<OnboardingStatus> OnboardingStatuses => Set<OnboardingStatus>();

    // Store entities
    /// <summary>
    /// Store mappings for multi-store entity access control.
    /// </summary>
    public DbSet<StoreMapping> StoreMappings => Set<StoreMapping>();

    // Manufacturing entities
    /// <summary>
    /// Bill of Materials (BOM) definitions for finished products.
    /// </summary>
    public DbSet<BillOfMaterials> BillOfMaterials => Set<BillOfMaterials>();
    
    /// <summary>
    /// Components within a Bill of Materials.
    /// </summary>
    public DbSet<BillOfMaterialsComponent> BillOfMaterialsComponents => Set<BillOfMaterialsComponent>();
    
    /// <summary>
    /// Production orders for manufacturing finished goods.
    /// </summary>
    public DbSet<ProductionOrder> ProductionOrders => Set<ProductionOrder>();
    
    /// <summary>
    /// Raw material consumption records for production orders.
    /// </summary>
    public DbSet<ProductionOrderConsumption> ProductionOrderConsumptions => Set<ProductionOrderConsumption>();
    
    /// <summary>
    /// Finished goods production records for production orders.
    /// </summary>
    public DbSet<ProductionOrderProduction> ProductionOrderProductions => Set<ProductionOrderProduction>();

    // Catalog entities
    /// <summary>
    /// Product attributes for configurable products (size, color, etc.).
    /// </summary>
    public DbSet<ProductAttribute> ProductAttributes => Set<ProductAttribute>();
    
    /// <summary>
    /// Specific values for product attributes (e.g., "Red", "Large").
    /// </summary>
    public DbSet<ProductAttributeValue> ProductAttributeValues => Set<ProductAttributeValue>();
    
    /// <summary>
    /// Customer reviews and ratings for products.
    /// </summary>
    public DbSet<ProductReview> ProductReviews => Set<ProductReview>();
    
    /// <summary>
    /// Tags for categorizing and searching products.
    /// </summary>
    public DbSet<ProductTag> ProductTags => Set<ProductTag>();
    
    /// <summary>
    /// Many-to-many mapping between products and tags.
    /// </summary>
    public DbSet<ProductProductTagMapping> ProductProductTagMappings => Set<ProductProductTagMapping>();

    // Vendor entities
    /// <summary>
    /// Vendors/suppliers who provide products to the business.
    /// </summary>
    public DbSet<Vendor> Vendors => Set<Vendor>();
    
    /// <summary>
    /// Notes and communications related to vendors.
    /// </summary>
    public DbSet<VendorNote> VendorNotes => Set<VendorNote>();
    
    /// <summary>
    /// Products available from each vendor (vendor catalog).
    /// </summary>
    public DbSet<VendorProduct> VendorProducts => Set<VendorProduct>();
    
    /// <summary>
    /// Pricing information for products from vendors.
    /// </summary>
    public DbSet<VendorPricing> VendorPricings => Set<VendorPricing>();

    // Order entities
    /// <summary>
    /// Customer orders placed through the system.
    /// </summary>
    public DbSet<Order> Orders => Set<Order>();
    
    /// <summary>
    /// Individual line items for each order.
    /// </summary>
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    
    /// <summary>
    /// Notes and comments attached to orders.
    /// </summary>
    public DbSet<OrderNote> OrderNotes => Set<OrderNote>();

    // Shipping entities
    /// <summary>
    /// Available shipping methods (Standard, Express, etc.).
    /// </summary>
    public DbSet<ShippingMethod> ShippingMethods => Set<ShippingMethod>();
    
    /// <summary>
    /// Shipments created to fulfill orders.
    /// </summary>
    public DbSet<Shipment> Shipments => Set<Shipment>();
    
    /// <summary>
    /// Individual items included in each shipment.
    /// </summary>
    public DbSet<ShipmentItem> ShipmentItems => Set<ShipmentItem>();

    /// <summary>
    /// Refresh tokens issued for sliding JWT sessions.
    /// </summary>
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    /// <summary>
    /// Business (tenant) aggregate root.
    /// </summary>
    public DbSet<Business> Businesses => Set<Business>();

    /// <summary>
    /// Mapping between users and businesses with per-business roles.
    /// </summary>
    public DbSet<UserBusiness> UserBusinesses => Set<UserBusiness>();

    /// <summary>
    /// Configures the model using Fluent API by applying all entity configurations from the assembly.
    /// </summary>
    /// <param name="builder">The ModelBuilder used to construct the model.</param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Apply all IEntityTypeConfiguration implementations from this assembly
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        ApplyBusinessFilters(builder);
    }

    private void ApplyBusinessFilters(ModelBuilder builder)
    {
        if (_businessContext is null)
        {
            return;
        }

        builder.Entity<Store>()
            .HasQueryFilter(store => !_businessContext.HasBusiness || store.BusinessId == _businessContext.CurrentBusinessId);

        builder.Entity<Product>()
            .HasQueryFilter(product => !_businessContext.HasBusiness || product.BusinessId == _businessContext.CurrentBusinessId);

        builder.Entity<Vendor>()
            .HasQueryFilter(vendor => !_businessContext.HasBusiness || vendor.BusinessId == _businessContext.CurrentBusinessId);

        builder.Entity<Driver>()
            .HasQueryFilter(driver => !_businessContext.HasBusiness || driver.BusinessId == _businessContext.CurrentBusinessId);

        builder.Entity<Customer>()
            .HasQueryFilter(customer => !_businessContext.HasBusiness || customer.BusinessId == _businessContext.CurrentBusinessId);

        builder.Entity<CustomerInteraction>()
            .HasQueryFilter(interaction => !_businessContext.HasBusiness || interaction.BusinessId == _businessContext.CurrentBusinessId);

        builder.Entity<StockLevel>()
            .HasQueryFilter(level => !_businessContext.HasBusiness || level.Shop!.BusinessId == _businessContext.CurrentBusinessId);

        builder.Entity<StockMovement>()
            .HasQueryFilter(movement => !_businessContext.HasBusiness || movement.Shop!.BusinessId == _businessContext.CurrentBusinessId);

        builder.Entity<StockAlert>()
            .HasQueryFilter(alert => !_businessContext.HasBusiness || alert.Shop!.BusinessId == _businessContext.CurrentBusinessId);

        builder.Entity<Sale>()
            .HasQueryFilter(sale => !_businessContext.HasBusiness || sale.Shop!.BusinessId == _businessContext.CurrentBusinessId);

        builder.Entity<CustomerPurchase>()
            .HasQueryFilter(purchase => !_businessContext.HasBusiness || purchase.Customer.BusinessId == _businessContext.CurrentBusinessId);

        builder.Entity<SaleItem>()
            .HasQueryFilter(item => !_businessContext.HasBusiness || item.Sale.Shop!.BusinessId == _businessContext.CurrentBusinessId);

        builder.Entity<SalesDocument>()
            .HasQueryFilter(document =>
                !_businessContext.HasBusiness ||
                ((document.Shop != null ? document.Shop.BusinessId : document.Sale.Shop.BusinessId) == _businessContext.CurrentBusinessId));

        builder.Entity<ShoppingCartItem>()
            .HasQueryFilter(item => !_businessContext.HasBusiness || item.Shop!.BusinessId == _businessContext.CurrentBusinessId);

        builder.Entity<Payment>()
            .HasQueryFilter(payment => !_businessContext.HasBusiness || payment.Shop!.BusinessId == _businessContext.CurrentBusinessId);

        builder.Entity<PayLink>()
            .HasQueryFilter(link => !_businessContext.HasBusiness || link.Shop!.BusinessId == _businessContext.CurrentBusinessId);

        builder.Entity<PurchaseOrder>()
            .HasQueryFilter(order => !_businessContext.HasBusiness || order.Shop!.BusinessId == _businessContext.CurrentBusinessId);

        builder.Entity<PurchaseRequest>()
            .HasQueryFilter(pr => !_businessContext.HasBusiness || pr.Shop!.BusinessId == _businessContext.CurrentBusinessId);

        builder.Entity<PurchaseDocument>()
            .HasQueryFilter(doc =>
                !_businessContext.HasBusiness ||
                ((doc.Shop != null ? doc.Shop.BusinessId : doc.PurchaseOrder.Shop!.BusinessId) == _businessContext.CurrentBusinessId));

        builder.Entity<PurchaseDocumentLine>()
            .HasQueryFilter(line =>
                !_businessContext.HasBusiness ||
                (
                    line.PurchaseDocument.Shop != null
                        ? line.PurchaseDocument.Shop.BusinessId == _businessContext.CurrentBusinessId
                        : line.PurchaseDocument.PurchaseOrder.Shop!.BusinessId == _businessContext.CurrentBusinessId
                ));

        builder.Entity<PurchaseReceipt>()
            .HasQueryFilter(receipt =>
                !_businessContext.HasBusiness ||
                receipt.PurchaseOrder.Shop!.BusinessId == _businessContext.CurrentBusinessId);

        builder.Entity<DeliveryStop>()
            .HasQueryFilter(stop => !_businessContext.HasBusiness || stop.Shop!.BusinessId == _businessContext.CurrentBusinessId);

        builder.Entity<AISettings>()
            .HasQueryFilter(settings => !_businessContext.HasBusiness || settings.Shop!.BusinessId == _businessContext.CurrentBusinessId);

        builder.Entity<AIConversation>()
            .HasQueryFilter(conversation => !_businessContext.HasBusiness || conversation.Shop!.BusinessId == _businessContext.CurrentBusinessId);

        builder.Entity<PoolParticipation>()
            .HasQueryFilter(participation => !_businessContext.HasBusiness || participation.Shop!.BusinessId == _businessContext.CurrentBusinessId);

        builder.Entity<GroupBuyPool>()
            .HasQueryFilter(pool => !_businessContext.HasBusiness || pool.InitiatorShop!.BusinessId == _businessContext.CurrentBusinessId);

        builder.Entity<Account>()
            .HasQueryFilter(account => !_businessContext.HasBusiness || account.BusinessId == _businessContext.CurrentBusinessId);

        builder.Entity<CashbookEntry>()
            .HasQueryFilter(entry => !_businessContext.HasBusiness || entry.BusinessId == _businessContext.CurrentBusinessId);

        builder.Entity<VendorLedgerEntry>()
            .HasQueryFilter(entry => !_businessContext.HasBusiness || entry.BusinessId == _businessContext.CurrentBusinessId);

        builder.Entity<BillOfMaterials>()
            .HasQueryFilter(bom => !_businessContext.HasBusiness || bom.BusinessId == _businessContext.CurrentBusinessId);

        builder.Entity<BillOfMaterialsComponent>()
            .HasQueryFilter(component => !_businessContext.HasBusiness || component.BusinessId == _businessContext.CurrentBusinessId);

        builder.Entity<ProductionOrder>()
            .HasQueryFilter(order => !_businessContext.HasBusiness || order.BusinessId == _businessContext.CurrentBusinessId);

        builder.Entity<ProductionOrderConsumption>()
            .HasQueryFilter(consumption => !_businessContext.HasBusiness || consumption.BusinessId == _businessContext.CurrentBusinessId);

        builder.Entity<ProductionOrderProduction>()
            .HasQueryFilter(production => !_businessContext.HasBusiness || production.BusinessId == _businessContext.CurrentBusinessId);
    }
}
