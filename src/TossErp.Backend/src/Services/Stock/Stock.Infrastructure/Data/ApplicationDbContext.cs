using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TossErp.Stock.Domain.Entities;
using TossErp.Stock.Infrastructure.Identity;
using TossErp.Stock.Domain.ValueObjects;
using TossErp.Stock.Domain.Aggregates.ItemAggregate;
using TossErp.Stock.Domain.Aggregates.ItemAggregate.Entities;
using TossErp.Stock.Domain.Aggregates.WarehouseAggregate;
using TossErp.Stock.Domain.Aggregates.WarehouseAggregate.Entities;
using TossErp.Stock.Domain.Aggregates.StockEntryAggregate;
using TossErp.Stock.Domain.Aggregates.StockEntryAggregate.Entities;
using TossErp.Stock.Application.Common.Interfaces;

namespace TossErp.Stock.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    // Stock Domain Entities
    public DbSet<ItemAggregate> Items => Set<ItemAggregate>();
    public DbSet<ItemVariant> ItemVariants => Set<ItemVariant>();
    public DbSet<ItemVariantAttribute> ItemVariantAttributes => Set<ItemVariantAttribute>();
    public DbSet<ItemPrice> ItemPrices => Set<ItemPrice>();
    public DbSet<ItemSupplier> ItemSuppliers => Set<ItemSupplier>();
    public DbSet<ItemCustomer> ItemCustomers => Set<ItemCustomer>();
    public DbSet<ItemBarcode> ItemBarcodes => Set<ItemBarcode>();
    public DbSet<ItemTax> ItemTaxes => Set<ItemTax>();
    public DbSet<ItemReorder> ItemReorders => Set<ItemReorder>();
    public DbSet<ItemAttribute> ItemAttributes => Set<ItemAttribute>();
    public DbSet<ItemAlternative> ItemAlternatives => Set<ItemAlternative>();
    public DbSet<ItemManufacturer> ItemManufacturers => Set<ItemManufacturer>();
    public DbSet<ItemWebsiteSpecification> ItemWebsiteSpecifications => Set<ItemWebsiteSpecification>();
    public DbSet<ItemQualityInspectionParameter> ItemQualityInspectionParameters => Set<ItemQualityInspectionParameter>();
    public DbSet<UOMConversionDetail> UOMConversionDetails => Set<UOMConversionDetail>();

    public DbSet<WarehouseAggregate> Warehouses => Set<WarehouseAggregate>();
    public DbSet<Bin> Bins => Set<Bin>();

    public DbSet<StockEntryAggregate> StockEntries => Set<StockEntryAggregate>();
    public DbSet<StockEntryDetail> StockEntryDetails => Set<StockEntryDetail>();
    public DbSet<StockEntryAdditionalCost> StockEntryAdditionalCosts => Set<StockEntryAdditionalCost>();

    public DbSet<Batch> Batches => Set<Batch>();
    public DbSet<SerialNo> SerialNos => Set<SerialNo>();
    public DbSet<SerialNo> SerialNumbers => Set<SerialNo>(); // Interface compatibility

    public DbSet<StockLedgerEntry> StockLedgerEntries => Set<StockLedgerEntry>();
    public DbSet<StockEntryType> StockEntryTypes => Set<StockEntryType>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Configure value objects as owned types to prevent EF Core from treating them as entities
        builder.Owned<Quantity>();
        builder.Owned<Rate>();
        builder.Owned<ItemCode>();
        builder.Owned<WarehouseCode>();
        builder.Owned<BinCode>();
        builder.Owned<Colour>();
        builder.Owned<SerialNumber>();
        builder.Owned<StockEntryNo>();
        
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
