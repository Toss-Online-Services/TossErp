using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TossErp.Stock.Domain.Entities;

namespace TossErp.Stock.Infrastructure.Data.Configurations;

public class ItemGroupConfiguration : IEntityTypeConfiguration<ItemGroup>
{
    public void Configure(EntityTypeBuilder<ItemGroup> builder)
    {
        builder.ToTable("ItemGroups");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Create unique index on Code
        builder.HasIndex(x => x.Code)
            .IsUnique();

        // Create index on Name for search performance
        builder.HasIndex(x => x.Name);

        // Create index on IsActive for filtering
        builder.HasIndex(x => x.IsActive);

        // Configure audit fields from BaseAuditableEntity
        builder.Property(x => x.Created)
            .IsRequired();

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(100);

        builder.Property(x => x.LastModified)
            .IsRequired();

        builder.Property(x => x.LastModifiedBy)
            .HasMaxLength(100);
    }
}

