#nullable enable

using MediatR;
using Microsoft.EntityFrameworkCore;
using POS.Domain.AggregatesModel.StoreAggregate;
using POS.Domain.AggregatesModel.CustomerAggregate;
using POS.Domain.AggregatesModel.OrderAggregate;
using POS.Domain.AggregatesModel.ProductAggregate;
using POS.Domain.AggregatesModel.PaymentAggregate;
using POS.Domain.AggregatesModel.SaleAggregate;
using POS.Domain.AggregatesModel.SyncAggregate;
using POS.Domain.AggregatesModel.InventoryAggregate;
using POS.Domain.AggregatesModel.StaffAggregate;
using POS.Domain.SeedWork;
using POS.Infrastructure.EntityConfigurations;
using POS.Infrastructure.Idempotency;
using ClientRequest = POS.Infrastructure.Idempotency.ClientRequest;
using PaymentAggregate = POS.Domain.AggregatesModel.PaymentAggregate;

namespace POS.Infrastructure;

public class POSContext : DbContext, IUnitOfWork
{
    public const string DEFAULT_SCHEMA = "pos";

    private readonly IMediator _mediator;

    public DbSet<Store> Stores { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductCategory> Categories { get; set; }
    public DbSet<PaymentAggregate.Payment> Payments { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<SyncLog> SyncLogs { get; set; }
    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<Staff> Staff { get; set; }
    public DbSet<ClientRequest> ClientRequests { get; set; }

    public POSContext(DbContextOptions<POSContext> options, IMediator mediator) : base(options)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CustomerEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new InventoryEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new OrderEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new OrderItemEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new PaymentEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new PaymentEventEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new PaymentSplitEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ProductEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new SaleEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new StaffEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new StoreEntityTypeConfiguration());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _mediator.DispatchDomainEventsAsync(this);
        return await base.SaveChangesAsync(cancellationToken);
    }
} 
