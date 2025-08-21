using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TossErp.Sales.Domain.Entities;
using TossErp.Sales.Domain.Enums;

namespace TossErp.Sales.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for Till entity
/// </summary>
public class TillConfiguration : IEntityTypeConfiguration<Till>
{
    public void Configure(EntityTypeBuilder<Till> builder)
    {
        builder.ToTable("Tills", "sales");

        // Primary key
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        // Multi-tenancy
        builder.Property<string>("TenantId")
            .HasMaxLength(50)
            .IsRequired();

        // Till properties
        builder.Property(x => x.TillNumber)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Location)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.OpenedAt)
            .IsRequired(false);

        builder.Property(x => x.ClosedAt)
            .IsRequired(false);

        builder.Property(x => x.OpenedBy)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(x => x.ClosedBy)
            .HasMaxLength(100)
            .IsRequired(false);

        // Starting and ending cash amounts
        builder.Property(x => x.StartingCash)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.EndingCash)
            .HasColumnType("decimal(18,2)")
            .IsRequired(false);

        // Audit fields
        builder.Property<DateTime>("CreatedDate")
            .IsRequired();

        builder.Property<DateTime?>("UpdatedDate")
            .IsRequired(false);

        builder.Property<string>("CreatedBy")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property<string>("UpdatedBy")
            .HasMaxLength(100)
            .IsRequired(false);

        // Unique constraints
        builder.HasIndex("TenantId", "TillNumber")
            .HasDatabaseName("IX_Tills_TenantId_TillNumber")
            .IsUnique();

        // Indexes
        builder.HasIndex(x => x.Status)
            .HasDatabaseName("IX_Tills_Status");

        builder.HasIndex(x => x.OpenedAt)
            .HasDatabaseName("IX_Tills_OpenedAt");

        builder.HasIndex("TenantId")
            .HasDatabaseName("IX_Tills_TenantId");
    }
}
