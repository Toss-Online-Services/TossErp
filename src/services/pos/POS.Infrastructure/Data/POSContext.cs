using Microsoft.EntityFrameworkCore;
using eShop.POS.Domain.AggregatesModel.ProductAggregate;
using eShop.POS.Domain.AggregatesModel.StaffAggregate;
using eShop.POS.Domain.AggregatesModel.StoreAggregate;

namespace eShop.POS.Infrastructure.Data;

public class POSContext : DbContext
{
    public POSContext(DbContextOptions<POSContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Staff> Staff { get; set; }
    public DbSet<Store> Stores { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(POSContext).Assembly);
    }
} 
