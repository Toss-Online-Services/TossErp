using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS.Domain.AggregatesModel.SyncAggregate;

namespace TossErp.POS.Infrastructure.EntityConfigurations;

public class ClientRequestEntityTypeConfiguration : IEntityTypeConfiguration<ClientRequest>
{
    public void Configure(EntityTypeBuilder<ClientRequest> builder)
    {
        builder.ToTable("client_requests", "POS");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasConversion<string>();

        builder.Property(c => c.StoreId).HasConversion<string>().IsRequired();

        builder.Property(c => c.RequestType).HasMaxLength(50).IsRequired();
        builder.Property(c => c.Status).HasMaxLength(20).IsRequired();
        builder.Property(c => c.Message).HasMaxLength(500);
        builder.Property(c => c.Details).HasMaxLength(2000);

        builder.Property(c => c.RequestDate).IsRequired();
        builder.Property(c => c.Success).IsRequired();

        builder.HasIndex(c => c.StoreId);
        builder.HasIndex(c => c.RequestType);
        builder.HasIndex(c => c.Status);
        builder.HasIndex(c => c.RequestDate);
        builder.HasIndex(c => c.Success);

        builder.HasOne(c => c.Store)
            .WithMany()
            .HasForeignKey(c => c.StoreId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
