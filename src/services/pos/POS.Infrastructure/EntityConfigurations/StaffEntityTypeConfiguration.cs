using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS.Domain.AggregatesModel.StaffAggregate;

namespace POS.Infrastructure.EntityConfigurations;

public class StaffEntityTypeConfiguration : IEntityTypeConfiguration<Staff>
{
    public void Configure(EntityTypeBuilder<Staff> builder)
    {
        builder.ToTable("Staff", POSContext.DEFAULT_SCHEMA);

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .UseHiLo("staffseq", POSContext.DEFAULT_SCHEMA);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Phone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(s => s.Role)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.PIN)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(s => s.StoreId)
            .IsRequired();

        builder.Property(s => s.IsActive)
            .IsRequired();

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.LastModifiedAt);

        // Configure value objects
        builder.OwnsOne(s => s.Schedule, schedule =>
        {
            schedule.Property(s => s.Day).IsRequired();
            schedule.Property(s => s.StartTime).IsRequired();
            schedule.Property(s => s.EndTime).IsRequired();
            schedule.Property(s => s.IsWorking).IsRequired();
        });

        builder.OwnsOne(s => s.Permissions, permissions =>
        {
            permissions.Property(p => p.CanManageInventory).IsRequired();
            permissions.Property(p => p.CanManageStaff).IsRequired();
            permissions.Property(p => p.CanManageProducts).IsRequired();
            permissions.Property(p => p.CanProcessRefunds).IsRequired();
            permissions.Property(p => p.CanViewReports).IsRequired();
            permissions.Property(p => p.CanManageSettings).IsRequired();
        });

        // Configure indexes
        builder.HasIndex(s => s.Email)
            .IsUnique();

        builder.HasIndex(s => s.Phone)
            .IsUnique();

        builder.HasIndex(s => s.PIN)
            .IsUnique();

        builder.HasIndex(s => s.StoreId);

        builder.HasIndex(s => s.Role);

        builder.HasIndex(s => s.IsActive);

        builder.HasIndex(s => s.CreatedAt);

        builder.HasIndex(s => s.Name);
    }
} 
