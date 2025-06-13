namespace TossErp.POS.Infrastructure.Data;

public class POSContext : DbContext, IUnitOfWork
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

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        await SaveChangesAsync(cancellationToken);
        return true;
    }
} 
