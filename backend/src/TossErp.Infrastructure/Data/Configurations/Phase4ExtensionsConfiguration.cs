using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TossErp.Domain.Entities.Projects;
using TossErp.Domain.Entities.WMS;
using TossErp.Domain.Entities.Marketing;
using TossErp.Domain.Entities.Ecommerce;

namespace TossErp.Infrastructure.Data.Configurations;

// Projects
public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects");
        
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.ProjectNumber)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.HasIndex(p => p.ProjectNumber)
            .IsUnique();
        
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(p => p.Status)
            .HasConversion<string>()
            .HasMaxLength(50);
        
        builder.Property(p => p.Type)
            .HasConversion<string>()
            .HasMaxLength(50);
        
        builder.Property(p => p.Budget)
            .HasPrecision(18, 2);
        
        builder.Property(p => p.ActualCost)
            .HasPrecision(18, 2);
        
        builder.Property(p => p.PercentComplete)
            .HasPrecision(5, 2);
        
        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => p.StartDate);
        
        // Relationships
        builder.HasMany(p => p.Tasks)
            .WithOne(t => t.Project)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class ProjectTaskConfiguration : IEntityTypeConfiguration<ProjectTask>
{
    public void Configure(EntityTypeBuilder<ProjectTask> builder)
    {
        builder.ToTable("ProjectTasks");
        
        builder.HasKey(t => t.Id);
        
        builder.Property(t => t.TaskNumber)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(t => t.Status)
            .HasConversion<string>()
            .HasMaxLength(50);
        
        builder.Property(t => t.EstimatedHours)
            .HasPrecision(10, 2);
        
        builder.Property(t => t.ActualHours)
            .HasPrecision(10, 2);
        
        builder.Property(t => t.PercentComplete)
            .HasPrecision(5, 2);
        
        builder.HasIndex(t => t.ProjectId);
        builder.HasIndex(t => t.Status);
        builder.HasIndex(t => t.AssignedToId);
    }
}

// WMS
public class BinLocationConfiguration : IEntityTypeConfiguration<BinLocation>
{
    public void Configure(EntityTypeBuilder<BinLocation> builder)
    {
        builder.ToTable("BinLocations");
        
        builder.HasKey(b => b.Id);
        
        builder.Property(b => b.BinCode)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.HasIndex(b => new { b.WarehouseId, b.BinCode })
            .IsUnique();
        
        builder.Property(b => b.Barcode)
            .HasMaxLength(100);
        
        builder.Property(b => b.Type)
            .HasConversion<string>()
            .HasMaxLength(50);
        
        builder.Property(b => b.MaxWeight)
            .HasPrecision(18, 2);
        
        builder.Property(b => b.MaxVolume)
            .HasPrecision(18, 4);
        
        builder.Property(b => b.CurrentWeight)
            .HasPrecision(18, 2);
        
        builder.Property(b => b.CurrentVolume)
            .HasPrecision(18, 4);
        
        builder.Property(b => b.TemperatureMin)
            .HasPrecision(5, 2);
        
        builder.Property(b => b.TemperatureMax)
            .HasPrecision(5, 2);
        
        builder.HasIndex(b => b.WarehouseId);
        builder.HasIndex(b => b.Zone);
        builder.HasIndex(b => b.IsActive);
    }
}

// Marketing
public class CampaignConfiguration : IEntityTypeConfiguration<Campaign>
{
    public void Configure(EntityTypeBuilder<Campaign> builder)
    {
        builder.ToTable("Campaigns");
        
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.CampaignNumber)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.HasIndex(c => c.CampaignNumber)
            .IsUnique();
        
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(c => c.Type)
            .HasConversion<string>()
            .HasMaxLength(50);
        
        builder.Property(c => c.Status)
            .HasConversion<string>()
            .HasMaxLength(50);
        
        builder.Property(c => c.Budget)
            .HasPrecision(18, 2);
        
        builder.Property(c => c.ActualSpend)
            .HasPrecision(18, 2);
        
        builder.Property(c => c.Revenue)
            .HasPrecision(18, 2);
        
        builder.HasIndex(c => c.Status);
        builder.HasIndex(c => c.StartDate);
    }
}

// Ecommerce
public class OnlineOrderConfiguration : IEntityTypeConfiguration<OnlineOrder>
{
    public void Configure(EntityTypeBuilder<OnlineOrder> builder)
    {
        builder.ToTable("OnlineOrders");
        
        builder.HasKey(o => o.Id);
        
        builder.Property(o => o.OrderNumber)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.HasIndex(o => o.OrderNumber)
            .IsUnique();
        
        builder.Property(o => o.PlatformOrderId)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.HasIndex(o => o.PlatformOrderId);
        
        builder.Property(o => o.Platform)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(o => o.Status)
            .HasConversion<string>()
            .HasMaxLength(50);
        
        builder.Property(o => o.CustomerName)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(o => o.Subtotal)
            .HasPrecision(18, 2);
        
        builder.Property(o => o.ShippingCost)
            .HasPrecision(18, 2);
        
        builder.Property(o => o.TaxAmount)
            .HasPrecision(18, 2);
        
        builder.Property(o => o.DiscountAmount)
            .HasPrecision(18, 2);
        
        builder.Property(o => o.TotalAmount)
            .HasPrecision(18, 2);
        
        builder.HasIndex(o => o.Status);
        builder.HasIndex(o => o.OrderDate);
        builder.HasIndex(o => o.IsSynced);
    }
}
