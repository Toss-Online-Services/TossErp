using Microsoft.EntityFrameworkCore;
using TossErp.Sales.Domain.Entities;

namespace TossErp.Sales.Infrastructure.Persistence;

/// <summary>
/// Entity Framework DbContext for Sales service
/// </summary>
public class SalesDbContext : DbContext
{
    public SalesDbContext(DbContextOptions<SalesDbContext> options) : base(options)
    {
    }

    public DbSet<Sale> Sales { get; set; }
    public DbSet<Till> Tills { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Sale entity
        modelBuilder.Entity<Sale>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ReceiptNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CustomerName).HasMaxLength(200);
            entity.Property(e => e.DiscountReason).HasMaxLength(500);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.TenantId).IsRequired().HasMaxLength(50);

            // Configure SaleItems as owned entity
            entity.OwnsMany(e => e.Items, item =>
            {
                item.WithOwner().HasForeignKey("SaleId");
                item.Property(i => i.ItemName).IsRequired().HasMaxLength(200);
                item.Property(i => i.ItemSku).IsRequired().HasMaxLength(50);
                item.Property(i => i.TenantId).IsRequired().HasMaxLength(50);
            });

            // Configure Payments as owned entity
            entity.OwnsMany(e => e.Payments, payment =>
            {
                payment.WithOwner().HasForeignKey("SaleId");
                payment.Property(p => p.Reference).HasMaxLength(100);
                payment.Property(p => p.TenantId).IsRequired().HasMaxLength(50);
            });

            // Indexes
            entity.HasIndex(e => e.ReceiptNumber).IsUnique();
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.TillId);
        });

        // Configure Till entity
        modelBuilder.Entity<Till>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.TenantId).IsRequired().HasMaxLength(50);

            // Indexes
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => e.Status);
        });
    }
}
