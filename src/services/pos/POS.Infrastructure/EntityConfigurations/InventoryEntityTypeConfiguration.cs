using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS.Domain.AggregatesModel.InventoryAggregate;

namespace POS.Infrastructure.EntityConfigurations;

public class InventoryEntityTypeConfiguration : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        builder.ToTable("Inventories", POSContext.DEFAULT_SCHEMA);

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .UseHiLo("inventoryseq", POSContext.DEFAULT_SCHEMA);

        builder.Property(i => i.Quantity)
            .IsRequired();

        builder.Property(i => i.MinQuantity)
            .IsRequired();

        builder.Property(i => i.MaxQuantity)
            .IsRequired();

        builder.Property(i => i.CreatedAt)
            .IsRequired();

        builder.Property(i => i.UpdatedAt)
            .IsRequired();

        builder.HasOne(i => i.Product)
            .WithMany()
            .HasForeignKey("ProductId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Store)
            .WithMany()
            .HasForeignKey("StoreId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(i => new { i.ProductId, i.StoreId })
            .IsUnique();
    }
} 
