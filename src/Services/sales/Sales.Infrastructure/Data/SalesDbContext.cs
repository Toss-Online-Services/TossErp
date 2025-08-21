using Microsoft.EntityFrameworkCore;
using TossErp.Sales.Domain.Entities;
using TossErp.Sales.Domain.ValueObjects;
using TossErp.Sales.Infrastructure.Persistence.Configurations;

namespace TossErp.Sales.Infrastructure.Data;

/// <summary>
/// Database context for the Sales module
/// </summary>
public class SalesDbContext : DbContext
{
    public SalesDbContext(DbContextOptions<SalesDbContext> options) : base(options)
    {
    }

    public DbSet<Sale> Sales { get; set; } = null!;
    public DbSet<SaleItem> SaleItems { get; set; } = null!;
    public DbSet<Payment> Payments { get; set; } = null!;
    public DbSet<Till> Tills { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        modelBuilder.ApplyConfiguration(new SaleConfiguration());
        modelBuilder.ApplyConfiguration(new SaleItemConfiguration());
        modelBuilder.ApplyConfiguration(new PaymentConfiguration());
        modelBuilder.ApplyConfiguration(new TillConfiguration());

        // Global query filters for multi-tenancy
        modelBuilder.Entity<Sale>().HasQueryFilter(x => EF.Property<string>(x, "TenantId") == "current_tenant");
        modelBuilder.Entity<SaleItem>().HasQueryFilter(x => EF.Property<string>(x, "TenantId") == "current_tenant");
        modelBuilder.Entity<Payment>().HasQueryFilter(x => EF.Property<string>(x, "TenantId") == "current_tenant");
        modelBuilder.Entity<Till>().HasQueryFilter(x => EF.Property<string>(x, "TenantId") == "current_tenant");

        // Set schema
        modelBuilder.HasDefaultSchema("sales");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Database=toss_sales;Username=postgres;Password=postgres");
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Update audit fields
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is Entity<Guid> && (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (Entity<Guid>)entry.Entity;
            
            if (entry.State == EntityState.Added)
            {
                entity.SetCreatedDate(DateTime.UtcNow);
                if (string.IsNullOrEmpty(entity.CreatedBy))
                    entity.SetCreatedBy("system");
            }
            else if (entry.State == EntityState.Modified)
            {
                entity.SetUpdatedDate(DateTime.UtcNow);
                if (string.IsNullOrEmpty(entity.UpdatedBy))
                    entity.SetUpdatedBy("system");
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
