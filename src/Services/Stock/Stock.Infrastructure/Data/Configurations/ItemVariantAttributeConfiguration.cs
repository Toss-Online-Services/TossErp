using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TossErp.Stock.Domain.Aggregates.ItemAggregate.Entities;

namespace TossErp.Stock.Infrastructure.Data.Configurations;

public class ItemVariantAttributeConfiguration : IEntityTypeConfiguration<ItemVariantAttribute>
{
    public void Configure(EntityTypeBuilder<ItemVariantAttribute> builder)
    {
        builder.ToTable("ItemVariantAttributes");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        // Properties
        builder.Property(x => x.Attribute).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Value).HasMaxLength(500).IsRequired();
        builder.Property(x => x.IsDisabled).IsRequired();

        // Note: ItemVariant is an owned entity, so we can't create a direct relationship
        // The relationship will be handled at the application level
        // builder.HasOne(x => x.ItemVariant)
        //     .WithMany()
        //     .HasForeignKey("ItemVariantId")
        //     .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(x => x.Attribute);
        builder.HasIndex(x => x.IsDisabled);
    }
}
