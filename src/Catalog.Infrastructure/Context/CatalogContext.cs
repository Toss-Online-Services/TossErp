using Catalog.Domain.Entities;
using Catalog.Domain.SeedWork;
using Catalog.Infrastructure.Context.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Pgvector.EntityFrameworkCore;
using System.Data;

namespace Catalog.Infrastructure.Context;

public class CatalogContext : DbContext, IUnitOfWork
{
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<ProductPicture> ProductPictures { get; set; } = null!;
    public DbSet<ProductReview> ProductReviews { get; set; } = null!;
    public DbSet<ProductTag> ProductTags { get; set; } = null!;
    public DbSet<CatalogBrand> CatalogBrands { get; set; } = null!;
    public DbSet<CatalogType> CatalogTypes { get; set; } = null!;
    public DbSet<ProductAttribute> ProductAttributes { get; set; } = null!;
    public DbSet<ProductAttributeMapping> ProductAttributeMappings { get; set; } = null!;
    public DbSet<ProductAttributeValue> ProductAttributeValues { get; set; } = null!;

    private IDbContextTransaction? _currentTransaction;

    public CatalogContext(DbContextOptions<CatalogContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        modelBuilder.ApplyConfiguration(new ProductPictureConfiguration());
        modelBuilder.ApplyConfiguration(new ProductReviewConfiguration());
        modelBuilder.ApplyConfiguration(new ProductTagConfiguration());
        modelBuilder.ApplyConfiguration(new CatalogBrandConfiguration());
        modelBuilder.ApplyConfiguration(new CatalogTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ProductAttributeConfiguration());
        modelBuilder.ApplyConfiguration(new ProductAttributeMappingConfiguration());
        modelBuilder.ApplyConfiguration(new ProductAttributeValueConfiguration());

        // Configure vector operations
        modelBuilder.HasPostgresExtension("vector");
    }

    public IDbContextTransaction GetCurrentTransaction() => _currentTransaction!;

    public bool HasActiveTransaction => _currentTransaction != null;

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        await base.SaveChangesAsync(cancellationToken);
        return true;
    }

    public Task<IDbContextTransaction> BeginTransactionAsync()
    {
        _currentTransaction = Database.BeginTransaction();
        return Task.FromResult(_currentTransaction);
    }

    public Task CommitTransactionAsync(IDbContextTransaction transaction)
    {
        transaction.Commit();
        return Task.CompletedTask;
    }

    public void RollbackTransaction()
    {
        _currentTransaction?.Rollback();
    }
} 
