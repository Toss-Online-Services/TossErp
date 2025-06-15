using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS.Domain.AggregatesModel.StaffAggregate;

namespace POS.Infrastructure.EntityConfigurations;

public class StaffEntityTypeConfiguration : IEntityTypeConfiguration<Staff>
{
    public void Configure(EntityTypeBuilder<Staff> builder)
    {
        builder.ToTable("Staff", "POS");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();

        builder.Property(x => x.StoreId).IsRequired();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Email).HasMaxLength(100);
        builder.Property(x => x.Phone).HasMaxLength(20);
        builder.Property(x => x.Position).HasMaxLength(100);
        builder.Property(x => x.Department).HasMaxLength(100);
        builder.Property(x => x.EmployeeId).HasMaxLength(50);
        builder.Property(x => x.TaxId).HasMaxLength(50);
        builder.Property(x => x.Notes).HasMaxLength(500);
        builder.Property(x => x.Status).HasMaxLength(50).IsRequired();
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.IsSynced).IsRequired();
        builder.Property(x => x.SyncedAt);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt);

        builder.HasIndex(x => x.StoreId);
        builder.HasIndex(x => x.Email).IsUnique();
        builder.HasIndex(x => x.Phone).IsUnique();
        builder.HasIndex(x => x.EmployeeId).IsUnique();
        builder.HasIndex(x => x.TaxId).IsUnique();
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.IsActive);
        builder.HasIndex(x => x.IsSynced);

        builder.HasOne(x => x.Store)
            .WithMany()
            .HasForeignKey(x => x.StoreId)
            .OnDelete(DeleteBehavior.Restrict);
    }
} 
