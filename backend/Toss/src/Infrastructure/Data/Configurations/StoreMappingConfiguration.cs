using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Stores;

namespace Toss.Infrastructure.Data.Configurations;

public class StoreMappingConfiguration : IEntityTypeConfiguration<StoreMapping>
{
    public void Configure(EntityTypeBuilder<StoreMapping> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.EntityName)
            .IsRequired()
            .HasMaxLength(400);

        builder.HasOne(x => x.Shop)
            .WithMany()
            .HasForeignKey(x => x.StoreId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.EntityId, x.EntityName });
    }
}

