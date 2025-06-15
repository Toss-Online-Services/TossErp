using System;
using System.Threading;
using System.Threading.Tasks;
using POS.Domain.AggregatesModel.StaffAggregate;
using POS.Domain.AggregatesModel.StoreAggregate;
using POS.Domain.AggregatesModel.SaleAggregate;
using POS.Domain.AggregatesModel.ProductAggregate;
using POS.Domain.AggregatesModel.BuyerAggregate;
using POS.Domain.Common;
using POS.Domain.AggregatesModel.SyncAggregate;
using Microsoft.EntityFrameworkCore.Storage;

namespace TossErp.POS.Infrastructure.Data;

public class POSContext : DbContext, IUnitOfWork
{
    private IDbContextTransaction? _currentTransaction;

    public POSContext(DbContextOptions<POSContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Staff> Staff { get; set; }
    public DbSet<Store> Stores { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<SaleItem> SaleItems { get; set; }
    public DbSet<SaleDiscount> SaleDiscounts { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Buyer> Buyers { get; set; }
    public DbSet<PaymentMethod> PaymentMethods { get; set; }
    public DbSet<CardType> CardTypes { get; set; }
    public DbSet<SyncLog> SyncLogs { get; set; }
    public DbSet<ClientRequest> ClientRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(POSContext).Assembly);

        // Configure value objects
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasNoKey();
            entity.Property(a => a.Street).HasMaxLength(200).IsRequired();
            entity.Property(a => a.City).HasMaxLength(100).IsRequired();
            entity.Property(a => a.State).HasMaxLength(100).IsRequired();
            entity.Property(a => a.Country).HasMaxLength(100).IsRequired();
            entity.Property(a => a.ZipCode).HasMaxLength(20).IsRequired();
        });
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        await SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            return;
        }

        _currentTransaction = await Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);

            if (_currentTransaction != null)
            {
                await _currentTransaction.CommitAsync(cancellationToken);
            }
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.RollbackAsync(cancellationToken);
            }
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    public override void Dispose()
    {
        _currentTransaction?.Dispose();
        base.Dispose();
    }
} 
