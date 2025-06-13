using Microsoft.EntityFrameworkCore;
using eShop.POS.Domain.AggregatesModel.ProductAggregate;
using eShop.POS.Domain.AggregatesModel.StaffAggregate;
using eShop.POS.Domain.AggregatesModel.StoreAggregate;
using eShop.POS.Domain.AggregatesModel.SaleAggregate;
using eShop.POS.Domain.AggregatesModel.BuyerAggregate;
using eShop.POS.Domain.AggregatesModel.SyncLogAggregate;
using eShop.POS.Infrastructure.Idempotency;

namespace eShop.POS.Infrastructure.Data;

public class POSContext : DbContext
{
    public POSContext(DbContextOptions<POSContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Staff> Staff { get; set; }
    public DbSet<Store> Stores { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<Buyer> Buyers { get; set; }
    public DbSet<PaymentMethod> PaymentMethods { get; set; }
    public DbSet<CardType> CardTypes { get; set; }
    public DbSet<SyncLog> SyncLogs { get; set; }
    public DbSet<ClientRequest> ClientRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(POSContext).Assembly);
    }
} 
