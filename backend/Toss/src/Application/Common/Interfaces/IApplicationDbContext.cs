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
using Toss.Domain.Entities.Onboarding;
using Toss.Domain.Entities.Payments;
using Toss.Domain.Entities.Sales;
using Toss.Domain.Entities.Security;
using Toss.Domain.Entities.Shipping;
using Toss.Domain.Entities.Stores;
using Toss.Domain.Entities.Vendors;
using Toss.Domain.Entities.Tax;
using Toss.Domain.Entities.Manufacturing;
using Toss.Domain.Entities.Tasks;
using Toss.Domain.Entities.Projects;
using Toss.Domain.Entities.Audit;
using Toss.Domain.Entities.Notifications;
using Toss.Domain.Entities.Businesses;
using Toss.Domain.Entities.Assets;
using Toss.Domain.Entities.Quality;
using Toss.Domain.Entities.HR;
using Toss.Domain.Entities.Support;
using Toss.Domain.Entities.Collaborations;
using Toss.Domain.Entities.Analytics;

namespace Toss.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    // Core entities
    /// <summary>
    /// Stores/Shops in the system. Each store represents a physical or virtual shop location.
    /// </summary>
    DbSet<Store> Stores { get; }
    
    /// <summary>
    /// Physical and billing addresses used throughout the system.
    /// </summary>
    DbSet<Address> Addresses { get; }

    // AI entities
    DbSet<AISettings> AISettings { get; }
    DbSet<AIConversation> AIConversations { get; }
    DbSet<AIMessage> AIMessages { get; }

    // Localization entities
    DbSet<Language> Languages { get; }
    DbSet<LocaleStringResource> LocaleStringResources { get; }
    DbSet<LocalizedProperty> LocalizedProperties { get; }

    // Directory entities
    DbSet<Country> Countries { get; }
    DbSet<StateProvince> StateProvinces { get; }
    DbSet<Currency> Currencies { get; }
    DbSet<MeasureWeight> MeasureWeights { get; }
    DbSet<MeasureDimension> MeasureDimensions { get; }

    // Tax entities
    DbSet<TaxCategory> TaxCategories { get; }
    DbSet<TaxRate> TaxRates { get; }

    // Security entities
    DbSet<PermissionRecord> PermissionRecords { get; }
    DbSet<PermissionRoleMapping> PermissionRoleMappings { get; }
    DbSet<AclRecord> AclRecords { get; }

    // Store entities (Shop is already defined above as core entity)
    DbSet<StoreMapping> StoreMappings { get; }

    // Business entities
    DbSet<Business> Businesses { get; }
    DbSet<UserBusiness> UserBusinesses { get; }
    DbSet<BusinessSettings> BusinessSettings { get; }

    // Catalog entities
    DbSet<ProductAttribute> ProductAttributes { get; }
    DbSet<ProductAttributeValue> ProductAttributeValues { get; }
    DbSet<ProductReview> ProductReviews { get; }
    DbSet<ProductTag> ProductTags { get; }
    DbSet<ProductProductTagMapping> ProductProductTagMappings { get; }

    // Vendor entities (merged from Suppliers)
    DbSet<Vendor> Vendors { get; }
    DbSet<VendorNote> VendorNotes { get; }
    DbSet<VendorProduct> VendorProducts { get; }
    DbSet<VendorPricing> VendorPricings { get; }

    // Order entities
    DbSet<Order> Orders { get; }
    DbSet<OrderItem> OrderItems { get; }
    DbSet<OrderNote> OrderNotes { get; }

    // Shipping entities
    DbSet<ShippingMethod> ShippingMethods { get; }
    DbSet<Shipment> Shipments { get; }
    DbSet<ShipmentItem> ShipmentItems { get; }

    // Inventory entities
    DbSet<Product> Products { get; }
    DbSet<ProductCategory> ProductCategories { get; }
    DbSet<StockLevel> StockLevels { get; }
    DbSet<StockMovement> StockMovements { get; }
    DbSet<StockAlert> StockAlerts { get; }

    // Sales entities
    DbSet<Sale> Sales { get; }
    DbSet<SaleItem> SaleItems { get; }
    DbSet<SalesDocument> SalesDocuments { get; }
    DbSet<ShoppingCartItem> ShoppingCartItems { get; }

    // Supplier entities (removed - now using Vendors)

    // Buying entities
    DbSet<PurchaseOrder> PurchaseOrders { get; }
    DbSet<PurchaseOrderItem> PurchaseOrderItems { get; }
    DbSet<PurchaseReceipt> PurchaseReceipts { get; }
    DbSet<PurchaseDocument> PurchaseDocuments { get; }
    DbSet<PurchaseDocumentLine> PurchaseDocumentLines { get; }
    DbSet<PurchaseRequest> PurchaseRequests { get; }
    DbSet<PurchaseRequestLine> PurchaseRequestLines { get; }

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

    // Accounting entities
    DbSet<Account> Accounts { get; }
    DbSet<CashbookEntry> CashbookEntries { get; }
    DbSet<VendorLedgerEntry> VendorLedgerEntries { get; }

    // Manufacturing entities
    DbSet<BillOfMaterials> BillOfMaterials { get; }
    DbSet<BillOfMaterialsComponent> BillOfMaterialsComponents { get; }
    DbSet<ProductionOrder> ProductionOrders { get; }
    DbSet<ProductionOrderConsumption> ProductionOrderConsumptions { get; }
    DbSet<ProductionOrderProduction> ProductionOrderProductions { get; }

    // Task entities
    DbSet<TaskItem> TaskItems { get; }

    // Project entities
    DbSet<Project> Projects { get; }
    DbSet<ProjectTask> ProjectTasks { get; }
    DbSet<ProjectMaterial> ProjectMaterials { get; }
    DbSet<LabourEntry> LabourEntries { get; }

    // Audit entities
    DbSet<AuditEntry> AuditEntries { get; }

    // Notification entities
    DbSet<Notification> Notifications { get; }
    DbSet<Comment> Comments { get; }
    DbSet<NotificationPreference> NotificationPreferences { get; }

    // Asset entities
    DbSet<Asset> Assets { get; }
    DbSet<AssetMaintenanceLog> AssetMaintenanceLogs { get; }

    // Quality entities
    DbSet<QualityChecklist> QualityChecklists { get; }
    DbSet<ChecklistItem> ChecklistItems { get; }
    DbSet<ChecklistRun> ChecklistRuns { get; }
    DbSet<ChecklistRunItem> ChecklistRunItems { get; }
    DbSet<Incident> Incidents { get; }
    DbSet<ActionItem> ActionItems { get; }

    // HR entities
    DbSet<Employee> Employees { get; }
    DbSet<Attendance> Attendances { get; }
    DbSet<PayrollRun> PayrollRuns { get; }

    // Support entities
    DbSet<Ticket> Tickets { get; }
    DbSet<TicketNote> TicketNotes { get; }

    // Collaboration entities
    DbSet<CollabLink> CollabLinks { get; }

    // Analytics entities
    DbSet<BusinessEvent> BusinessEvents { get; }

    // Onboarding entities
    DbSet<OnboardingStatus> OnboardingStatuses { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
