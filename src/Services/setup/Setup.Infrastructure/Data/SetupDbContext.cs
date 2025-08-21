using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.AspNetCore.Http;
using Setup.Domain.Aggregates.TenantAggregate;
using Setup.Domain.Aggregates.OrganizationAggregate;
using Setup.Domain.Aggregates.ApplicationConfigurationAggregate;
using Setup.Domain.SeedWork;
using Setup.Domain.ValueObjects;
using Setup.Infrastructure.Data.Configurations;
using System.Reflection;
using System.Linq.Expressions;

namespace Setup.Infrastructure.Data;

public class SetupDbContext : DbContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private string? _currentTenantId;

    public SetupDbContext(DbContextOptions<SetupDbContext> options, 
                         IHttpContextAccessor httpContextAccessor) 
        : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    // Tenant Aggregate
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<IntegrationConfiguration> IntegrationConfigurations => Set<IntegrationConfiguration>();
    public DbSet<AuditConfiguration> AuditConfigurations => Set<AuditConfiguration>();
    public DbSet<StorageMetrics> StorageMetrics => Set<StorageMetrics>();
    public DbSet<ComplianceFramework> ComplianceFrameworks => Set<ComplianceFramework>();

    // Organization Aggregate
    public DbSet<Organization> Organizations => Set<Organization>();

    // Application Configuration Aggregate
    public DbSet<ApplicationConfiguration> ApplicationConfigurations => Set<ApplicationConfiguration>();
    public DbSet<ModuleConfiguration> ModuleConfigurations => Set<ModuleConfiguration>();
    public DbSet<FeatureFlag> FeatureFlags => Set<FeatureFlag>();
    public DbSet<NotificationTemplate> NotificationTemplates => Set<NotificationTemplate>();
    public DbSet<ApiKeyConfiguration> ApiKeyConfigurations => Set<ApiKeyConfiguration>();
    public DbSet<RateLimitRule> RateLimitRules => Set<RateLimitRule>();
    public DbSet<BackupConfiguration> BackupConfigurations => Set<BackupConfiguration>();

    // Complex Types for EF Core 9
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Configure complex types for value objects
        configurationBuilder.ComplexType<SubscriptionPlan>();
        configurationBuilder.ComplexType<BillingCycle>();
        configurationBuilder.ComplexType<UsageQuota>();
        configurationBuilder.ComplexType<SecurityPolicy>();
        configurationBuilder.ComplexType<ComplianceRequirement>();
        configurationBuilder.ComplexType<ContactInfo>();
        configurationBuilder.ComplexType<Address>();
        configurationBuilder.ComplexType<ConfigurationValue>();
        configurationBuilder.ComplexType<NotificationSettings>();
        configurationBuilder.ComplexType<RateLimit>();
        configurationBuilder.ComplexType<BackupRetention>();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(connectionString =>
            {
                connectionString.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null);
            });
        }

        // EF Core 9 optimizations
        optionsBuilder.EnableSensitiveDataLogging(false);
        optionsBuilder.EnableDetailedErrors(false);
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Configure global query filters for multi-tenancy
        ConfigureGlobalQueryFilters(modelBuilder);

        // Configure shadow properties for audit fields
        ConfigureShadowProperties(modelBuilder);

        // Configure indexes for performance
        ConfigureIndexes(modelBuilder);
    }

    private void ConfigureGlobalQueryFilters(ModelBuilder modelBuilder)
    {
        // Multi-tenant entities get global query filter
        var tenantEntities = new[]
        {
            typeof(UserProfile),
            typeof(IntegrationConfiguration),
            typeof(AuditConfiguration),
            typeof(StorageMetrics),
            typeof(ComplianceFramework),
            typeof(Organization),
            typeof(ApplicationConfiguration),
            typeof(ModuleConfiguration),
            typeof(FeatureFlag),
            typeof(NotificationTemplate),
            typeof(ApiKeyConfiguration),
            typeof(RateLimitRule),
            typeof(BackupConfiguration)
        };

        foreach (var entityType in tenantEntities)
        {
            var entityBuilder = modelBuilder.Entity(entityType);
            
            // Add tenant ID property if not exists
            if (!entityBuilder.Metadata.GetProperties().Any(p => p.Name == "TenantId"))
            {
                entityBuilder.Property<string>("TenantId")
                    .HasMaxLength(50)
                    .IsRequired();
            }

            // Configure global query filter
            var parameter = Expression.Parameter(entityType, "e");
            var tenantIdProperty = Expression.Property(parameter, "TenantId");
            var currentTenantId = Expression.Constant(GetCurrentTenantId());
            var filterExpression = Expression.Equal(tenantIdProperty, currentTenantId);
            var lambda = Expression.Lambda(filterExpression, parameter);
            
            entityBuilder.HasQueryFilter(lambda);

            // Add index on TenantId for performance
            entityBuilder.HasIndex("TenantId")
                .HasDatabaseName($"IX_{entityType.Name}_TenantId")
                .HasFillFactor(85);
        }
    }

    private void ConfigureShadowProperties(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // Add audit fields as shadow properties
            entityType.AddProperty("CreatedAt", typeof(DateTime))
                .SetDefaultValueSql("GETUTCDATE()");
            
            entityType.AddProperty("CreatedBy", typeof(string))
                .SetMaxLength(100);
            
            entityType.AddProperty("ModifiedAt", typeof(DateTime?));
            
            entityType.AddProperty("ModifiedBy", typeof(string))
                .SetMaxLength(100);
            
            entityType.AddProperty("IsDeleted", typeof(bool))
                .SetDefaultValue(false);
            
            entityType.AddProperty("DeletedAt", typeof(DateTime?));
            
            entityType.AddProperty("DeletedBy", typeof(string))
                .SetMaxLength(100);

            // Configure soft delete filter
            var parameter = Expression.Parameter(entityType.ClrType, "e");
            var isDeletedProperty = Expression.Property(parameter, "IsDeleted");
            var notDeleted = Expression.Equal(isDeletedProperty, Expression.Constant(false));
            var lambda = Expression.Lambda(notDeleted, parameter);
            
            entityType.SetQueryFilter(lambda);
        }
    }

    private void ConfigureIndexes(ModelBuilder modelBuilder)
    {
        // Performance indexes with EF Core 9 fill factors
        modelBuilder.Entity<Tenant>()
            .HasIndex(t => t.Code)
            .IsUnique()
            .HasFillFactor(90);

        modelBuilder.Entity<Tenant>()
            .HasIndex(t => t.Status)
            .HasFillFactor(85);

        modelBuilder.Entity<UserProfile>()
            .HasIndex(u => u.Email)
            .IsUnique()
            .HasFillFactor(90);

        modelBuilder.Entity<UserProfile>()
            .HasIndex(u => u.IsActive)
            .HasFillFactor(85);

        modelBuilder.Entity<Organization>()
            .HasIndex(o => o.Code)
            .IsUnique()
            .HasFillFactor(90);

        modelBuilder.Entity<FeatureFlag>()
            .HasIndex(f => new { f.Key, f.IsEnabled })
            .HasFillFactor(85);

        modelBuilder.Entity<ApiKeyConfiguration>()
            .HasIndex(a => a.KeyHash)
            .IsUnique()
            .HasFillFactor(90);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditInformation();
        ApplyTenantId();
        return await base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        ApplyAuditInformation();
        ApplyTenantId();
        return base.SaveChanges();
    }

    private void ApplyAuditInformation()
    {
        var currentUser = GetCurrentUserId();
        var currentTime = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Property("CreatedAt").CurrentValue = currentTime;
                    entry.Property("CreatedBy").CurrentValue = currentUser;
                    entry.Property("IsDeleted").CurrentValue = false;
                    break;

                case EntityState.Modified:
                    entry.Property("ModifiedAt").CurrentValue = currentTime;
                    entry.Property("ModifiedBy").CurrentValue = currentUser;
                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Property("IsDeleted").CurrentValue = true;
                    entry.Property("DeletedAt").CurrentValue = currentTime;
                    entry.Property("DeletedBy").CurrentValue = currentUser;
                    break;
            }
        }
    }

    private void ApplyTenantId()
    {
        var currentTenantId = GetCurrentTenantId();
        if (string.IsNullOrEmpty(currentTenantId)) return;

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Added)
            {
                var tenantIdProperty = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "TenantId");
                if (tenantIdProperty != null && tenantIdProperty.CurrentValue == null)
                {
                    tenantIdProperty.CurrentValue = currentTenantId;
                }
            }
        }
    }

    private string? GetCurrentTenantId()
    {
        if (!string.IsNullOrEmpty(_currentTenantId))
            return _currentTenantId;

        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.Items.ContainsKey("TenantId") == true)
        {
            _currentTenantId = httpContext.Items["TenantId"]?.ToString();
        }
        else if (httpContext?.Request.Headers.ContainsKey("X-Tenant-Id") == true)
        {
            _currentTenantId = httpContext.Request.Headers["X-Tenant-Id"].FirstOrDefault();
        }

        return _currentTenantId;
    }

    private string? GetCurrentUserId()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        return httpContext?.User?.FindFirst("sub")?.Value 
               ?? httpContext?.User?.FindFirst("userId")?.Value
               ?? "system";
    }

    // Method to set tenant context manually (for background services)
    public void SetTenantContext(string tenantId)
    {
        _currentTenantId = tenantId;
    }

    // Method to bypass tenant filtering (for system operations)
    public IQueryable<T> GetEntityWithoutTenantFilter<T>() where T : class
    {
        return Set<T>().IgnoreQueryFilters();
    }
}
