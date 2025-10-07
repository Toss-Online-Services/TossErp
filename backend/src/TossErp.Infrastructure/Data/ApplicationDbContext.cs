using Microsoft.EntityFrameworkCore;
using TossErp.Domain.Common;
using TossErp.Domain.Entities.Sales;
using TossErp.Domain.Entities.Inventory;
using TossErp.Domain.Entities.Finance;
using TossErp.Domain.Entities.Procurement;
using TossErp.Domain.Entities.HR;
using TossErp.Domain.Entities.Auth;
using TossErp.Domain.Entities.Manufacturing;
using TossErp.Domain.Entities.SupplyChain;
using TossErp.Domain.Entities.Projects;
using TossErp.Domain.Entities.WMS;
using TossErp.Domain.Entities.Marketing;
using TossErp.Domain.Entities.Ecommerce;
using TossErp.Domain.Entities.Collaboration;

namespace TossErp.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    // Sales
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Customer> Customers => Set<Customer>();
    
    // Inventory
    public DbSet<Product> Products => Set<Product>();
    public DbSet<StockLevel> StockLevels => Set<StockLevel>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    
    // Finance
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();
    public DbSet<JournalEntryLine> JournalEntryLines => Set<JournalEntryLine>();
    
    // Procurement
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<PurchaseOrderItem> PurchaseOrderItems => Set<PurchaseOrderItem>();
    
    // HR
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();
    public DbSet<AttendanceRecord> AttendanceRecords => Set<AttendanceRecord>();
    
    // Auth
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    
    // Manufacturing
    public DbSet<BillOfMaterials> BillsOfMaterials => Set<BillOfMaterials>();
    public DbSet<BomItem> BomItems => Set<BomItem>();
    public DbSet<BomOperation> BomOperations => Set<BomOperation>();
    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();
    public DbSet<WorkOrderOperation> WorkOrderOperations => Set<WorkOrderOperation>();
    public DbSet<WorkOrderMaterial> WorkOrderMaterials => Set<WorkOrderMaterial>();
    public DbSet<ProductionEntry> ProductionEntries => Set<ProductionEntry>();
    
    // Supply Chain
    public DbSet<Shipment> Shipments => Set<Shipment>();
    public DbSet<ShipmentItem> ShipmentItems => Set<ShipmentItem>();
    public DbSet<ShipmentTracking> ShipmentTrackingHistory => Set<ShipmentTracking>();
    public DbSet<Carrier> Carriers => Set<Carrier>();
    
    // Projects
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectTask> ProjectTasks => Set<ProjectTask>();
    
    // WMS (Advanced Warehouse Management)
    public DbSet<BinLocation> BinLocations => Set<BinLocation>();
    
    // Marketing
    public DbSet<Campaign> Campaigns => Set<Campaign>();
    
    // Ecommerce
    public DbSet<OnlineOrder> OnlineOrders => Set<OnlineOrder>();
    
    // Collaboration (ERP III Features)
    public DbSet<BuyingGroup> BuyingGroups => Set<BuyingGroup>();
    public DbSet<GroupMember> GroupMembers => Set<GroupMember>();
    public DbSet<GroupPurchaseOrder> GroupPurchaseOrders => Set<GroupPurchaseOrder>();
    public DbSet<GroupPurchaseOrderItem> GroupPurchaseOrderItems => Set<GroupPurchaseOrderItem>();
    public DbSet<MemberOrderAllocation> MemberOrderAllocations => Set<MemberOrderAllocation>();
    public DbSet<DeliveryPool> DeliveryPools => Set<DeliveryPool>();
    public DbSet<DeliveryPoolParticipant> DeliveryPoolParticipants => Set<DeliveryPoolParticipant>();
    public DbSet<PoolStop> PoolStops => Set<PoolStop>();
    public DbSet<SharedAsset> SharedAssets => Set<SharedAsset>();
    public DbSet<AssetRental> AssetRentals => Set<AssetRental>();
    public DbSet<CreditPool> CreditPools => Set<CreditPool>();
    public DbSet<CreditPoolMember> CreditPoolMembers => Set<CreditPoolMember>();
    public DbSet<CreditAllocation> CreditAllocations => Set<CreditAllocation>();
    public DbSet<BusinessDirectory> BusinessDirectory => Set<BusinessDirectory>();
    public DbSet<CommunityEvent> CommunityEvents => Set<CommunityEvent>();
    public DbSet<EventRegistration> EventRegistrations => Set<EventRegistration>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply all configurations from the assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        
        // Global query filters for soft delete
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "e");
                var property = System.Linq.Expressions.Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
                var filter = System.Linq.Expressions.Expression.Lambda(
                    System.Linq.Expressions.Expression.Equal(property, System.Linq.Expressions.Expression.Constant(false)),
                    parameter);
                
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
            }
        }
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Handle timestamps and soft delete
        var entries = ChangeTracker.Entries<BaseEntity>();
        
        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                    
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
                    
                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAt = DateTime.UtcNow;
                    break;
            }
        }
        
        return await base.SaveChangesAsync(cancellationToken);
    }
}

