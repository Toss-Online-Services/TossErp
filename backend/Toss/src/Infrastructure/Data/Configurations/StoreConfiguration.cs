using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Stores;

namespace Toss.Infrastructure.Data.Configurations;

public class StoreConfiguration : IEntityTypeConfiguration<Store>
{
    public void Configure(EntityTypeBuilder<Store> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(400);

        builder.Property(x => x.Url)
            .IsRequired()
            .HasMaxLength(400);

        builder.Property(x => x.Hosts)
            .HasMaxLength(1000);

        builder.Property(x => x.CompanyName)
            .HasMaxLength(1000);

        builder.Property(x => x.CompanyAddress)
            .HasMaxLength(1000);

        builder.Property(x => x.CompanyPhoneNumber)
            .HasMaxLength(50);

        builder.Property(x => x.CompanyVat)
            .HasMaxLength(50);

        builder.HasMany(x => x.StoreMappings)
            .WithOne(x => x.Store)
            .HasForeignKey(x => x.StoreId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

